using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using OKRNotification.EF;
using OKRNotification.Service;
using OKRNotification.Service.AutoMapper;
using OKRNotification.Service.Contracts;
using OKRNotification.ViewModel.Response;
using OKRNotification.WebCore.Filter;
using OKRNotification.WebCore.Middleware;
using Serilog;
using Serilog.Events;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Polly;

namespace OKRNotification.WebCore
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public static IWebHostEnvironment AppEnvironment { get; private set; }
        public IConfiguration Configuration { get; }
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            AppEnvironment = env;
            Configuration = configuration;
            var envName = env?.EnvironmentName;
            Console.Write("********* Environment :" + envName);
            Log.Logger = new LoggerConfiguration()
               .Enrich.WithProperty("Environment", envName)
               .Enrich.WithMachineName()
               .Enrich.WithProcessId()
               .Enrich.WithThreadId()
               .WriteTo.Console()
               .MinimumLevel.Information()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
               .Enrich.FromLogContext()
               .CreateLogger();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddControllers().AddNewtonsoftJson(opt =>
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddLogging();
            var keyVault = new DatabaseVaultResponse();
            services.AddDbContext<NotificationDbContext>((serviceProvider, options) =>
            {
                var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
                if (httpContextAccessor != null)
                {
                    var httpContext = httpContextAccessor.HttpContext;
                    var tokenDecoded = TokenDecoded(httpContext);
                    //var tid = tokenDecoded.Claims.FirstOrDefault(c => c.Type == "tid");
                    //var tenantId = tid == null ? string.Empty : tid.Value;
                   var hasTenant = httpContext.Request.Headers.TryGetValue("TenantId", out var tenantId);
                    if ((!hasTenant && httpContext.Request.Host.Value.Contains("localhost")))
                        tenantId = Configuration.GetValue<string>("TenantId");

                    if (!string.IsNullOrEmpty(tenantId))
                    {
                        var tenantString = DecryptRijndael(tenantId, Configuration.GetValue<string>("PrivateKey"));
                        var key = tenantString + "-Connection" + Configuration.GetValue<string>("KeyVaultConfig:PostFix");
                        keyVault.ConnectionString = Configuration.GetValue<string>(key);
                        var retryPolicy = Policy.Handle<Exception>().Retry(2, (ex, count, context) =>
                        {
                            (Configuration as IConfigurationRoot)?.Reload();
                            keyVault.ConnectionString = Configuration.GetValue<string>(key);
                        });
                        retryPolicy.Execute(() =>
                        {
                            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).UseSqlServer(keyVault?.ConnectionString);
                        });
                    }
                    else
                    {
                        Console.WriteLine("Invalid tenant is received");
                    }
                }
                //var conn = Configuration.GetConnectionString("ConnectionString");
                //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).UseSqlServer(conn);
            });

            services.AddScoped<IOperationStatus, OperationStatus>();
            services.AddScoped<IDataContextAsync>(opt => opt.GetRequiredService<NotificationDbContext>());
            services.AddScoped<IUnitOfWorkAsync, UnitOfWork>();
            services.AddTransient<IServicesAggregator, ServicesAggregator>();
            services.AddTransient<IKeyVaultService, KeyVaultService>();

            services.AddAutoMapper(Assembly.Load("OKRNotification.Service"));
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddMvc(options => options.Filters.Add(typeof(ExceptionFiltersAttribute)))
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSwaggerGen(c =>
                    {
                        c.OperationFilter<CustomHeaderSwaggerFilter>();
                        c.SwaggerDoc("v1", new OpenApiInfo()
                        {
                            Version = "v1",
                            Title = "Notification API",
                            Description = "Notification API",
                            TermsOfService = new Uri(Configuration.GetSection("TermsAndConditionUrl").Value)
                        });
                    });

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IEmailServiceV2, EmailServiceV2>();
            services.AddTransient<IOkrNotificationsService, OkrNotificationsService>();
            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<TokenManagerMiddleware>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(Configuration.GetSection("SwaggerEndpoint").Value, "Notifications API");
            });
            app.UseRouting();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseMiddleware<TokenManagerMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        #region PRIVATE METHODS
        private async Task<TenantResponse> GetTenantIdAsync(string domain, string email, string jwtToken)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(Configuration.GetValue<string>("TenantService:BaseUrl"))
            };
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.GetAsync($"GetTenant?subDomain=" + domain + "&emailId=" + email);
            if (!response.IsSuccessStatusCode) return null;
            var payload = JsonConvert.DeserializeObject<PayloadCustom<TenantResponse>>(await response.Content.ReadAsStringAsync());
            return payload.Entity;
        }
        private async Task<DatabaseVaultResponse> KeyVaultConfiguration(string tenantId, HttpContext httpContext)
        {
            DatabaseVaultResponse databaseVault = new DatabaseVaultResponse();
            string authorization = httpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorization))
                authorization = httpContext.Request.Headers["Token"];
            var token = string.Empty;
            if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = authorization.Substring("Bearer ".Length).Trim();
            string dbSecretApiUrl = Configuration.GetValue<string>("AzureSettings:AzureSecretApiUrl");
            var uri = new Uri(dbSecretApiUrl + tenantId);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(uri);
            if (response is { Headers: { } })
            {
                var suffixName = Configuration.GetValue<string>("AzureSettings:Suffix");
                var conn = response.Headers.ToList().FirstOrDefault(x => x.Key == "ConnectionString" + suffixName);
                var schema = response.Headers.ToList().FirstOrDefault(x => x.Key == "Schema");
                if (conn.Key != null && schema.Key != null)
                {
                    databaseVault.ConnectionString = conn.Value.FirstOrDefault();
                    databaseVault.CurrentSchema = schema.Value.FirstOrDefault();
                }
            }
            return databaseVault;
        }
        private JwtSecurityToken TokenDecoded(HttpContext context)
        {
            string authorization = context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorization))
                authorization = context.Request.Headers["Token"];

            var token = string.Empty;
            if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = authorization.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(token))
                return null;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;

            return tokenS;
        }

        #endregion

        #region Rijndael Encryption
        private string DecryptRijndael(string cipherinput, string salt)
        {
            if (string.IsNullOrEmpty(cipherinput))
                throw new ArgumentNullException("cipherinput");
            if (!IsBase64String(cipherinput))
                throw new FormatException("The cipherText input parameter is not base64 encoded");
            string text;
            var aesAlg = NewRijndaelManaged(salt);
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            var cipher = Convert.FromBase64String(cipherinput);
            using var msDecrypt = new MemoryStream(cipher);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            text = srDecrypt.ReadToEnd();

            return text;
        }
        private static RijndaelManaged NewRijndaelManaged(string salt)
        {
            string InputKey = "99334E81-342C-4900-86D9-07B7B9FE5EBB";
            if (salt == null) throw new ArgumentNullException("salt");
            var saltBytes = Encoding.ASCII.GetBytes(salt);
            var key = new Rfc2898DeriveBytes(InputKey, saltBytes);

            var aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
            return aesAlg;
        }
        private static bool IsBase64String(string base64String)
        {
            const string Base64Regex = @"^[a-zA-Z0-9\+/]*={0,3}$";
            base64String = base64String.Trim();
            return (base64String.Length % 4 == 0) &&
                   Regex.IsMatch(base64String, Base64Regex, RegexOptions.None);

        }
        #endregion
    }
}
