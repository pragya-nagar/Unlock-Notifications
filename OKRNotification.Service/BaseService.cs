using System;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OKRNotification.EF;
using OKRNotification.Service.Contracts;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace OKRNotification.Service
{
    [ExcludeFromCodeCoverage]
    public abstract class BaseService : IBaseService
    {
        public IUnitOfWorkAsync UnitOfWorkAsync { get; set; }
        public IOperationStatus OperationStatus { get; set; }
        public NotificationDbContext NotificationDbContext { get; set; }
        public IConfiguration Configuration { get; set; }
        public IHostingEnvironment HostingEnvironment { get; set; }
        protected ILogger Logger { get; private set; }

        protected IMapper Mapper { get; private set; }

        protected HttpContext HttpContext => new HttpContextAccessor().HttpContext;
        protected string LoggedInUserEmail => HttpContext.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
        protected string UserToken => HttpContext.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(x => x.Type == "token")?.Value;
        protected bool IsTokenActive => (!string.IsNullOrEmpty(LoggedInUserEmail) && !string.IsNullOrEmpty(UserToken));
        private IKeyVaultService _keyVaultService;
        public IKeyVaultService KeyVaultService => _keyVaultService ??= HttpContext.RequestServices.GetRequiredService<IKeyVaultService>();
        public string ConnectionString
        {
            get => NotificationDbContext?.Database.GetDbConnection().ConnectionString;
            set
            {
                if (NotificationDbContext != null)
                    NotificationDbContext.Database.GetDbConnection().ConnectionString = value;
            }
        }
        protected BaseService(IServicesAggregator servicesAggregateService)
        {
            UnitOfWorkAsync = servicesAggregateService.UnitOfWorkAsync;
            NotificationDbContext = UnitOfWorkAsync.DataContext as NotificationDbContext;
            OperationStatus = servicesAggregateService.OperationStatus;
            Configuration = servicesAggregateService.Configuration;
            HostingEnvironment = servicesAggregateService.HostingEnvironment;
            Logger = Log.Logger;
            Mapper = servicesAggregateService.Mapper;

        }

        public HttpClient GetHttpClient(string jwtToken)
        {
            var hasTenant = HttpContext.Request.Headers.TryGetValue("TenantId", out var tenantId);
            if ((!hasTenant && HttpContext.Request.Host.Value.Contains("localhost")))
                tenantId = Configuration.GetValue<string>("TenantId");

            string domain;
            var hasOrigin = HttpContext.Request.Headers.TryGetValue("OriginHost", out var origin);
            if (!hasOrigin && HttpContext.Request.Host.Value.Contains("localhost"))
                domain = Configuration.GetValue<string>("FrontEndUrl");
            else
                domain = string.IsNullOrEmpty(origin) ? string.Empty : origin.ToString();

            //var settings = KeyVaultService.GetSettingsAndUrlsAsync().Result;
            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(Configuration.GetValue<string>("User:BaseUrl"))
            };
            var token = !string.IsNullOrEmpty(jwtToken) ? jwtToken : UserToken;
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("TenantId", tenantId.ToString());
            httpClient.DefaultRequestHeaders.Add("OriginHost", domain);
            return httpClient;
        }


    }
}
