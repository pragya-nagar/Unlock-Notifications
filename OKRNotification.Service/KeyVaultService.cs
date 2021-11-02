using Microsoft.Extensions.Configuration;
using OKRNotification.Service.Contracts;
using OKRNotification.ViewModel.Response;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OKRNotification.Service
{
    public class KeyVaultService : BaseService, IKeyVaultService
    {
        public KeyVaultService(IServicesAggregator servicesAggregateService) : base(servicesAggregateService)
        {
        }
     
        public async Task<ServiceSettingUrlResponse> GetSettingsAndUrlsAsync()
        {
            if (!IsTokenActive) return null;
            var tenantId = GetTenantId(UserToken);
            ServiceSettingUrlResponse settingsResponse = null;
            var dbSecretApiUrl = Configuration.GetValue<string>("AzureSettings:AzureSecretApiSettingUrl");
            var uri = new Uri(dbSecretApiUrl + tenantId);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserToken);
            var response = await client.GetAsync(uri);
            if (response is { Headers: { } })
            {
                settingsResponse = new ServiceSettingUrlResponse();
                var headers = response.Headers.ToList();
                var unlockLog = headers.FirstOrDefault(x => x.Key == "UnlockLog");
                var okrBaseAddress = headers.FirstOrDefault(x => x.Key == "OkrBaseAddress");
                var unlockTime = headers.FirstOrDefault(x => x.Key == "OkrUnlockTime");
                var frontEndUrl = headers.FirstOrDefault(x => x.Key == "FrontEndUrl");
                var resetPassUrl = headers.FirstOrDefault(x => x.Key == "ResetPassUrl");
                var notificationUrl = headers.FirstOrDefault(x => x.Key == "NotificationBaseAddress");

                settingsResponse.UnlockLog = unlockLog.Key != null ? unlockLog.Value.FirstOrDefault() : string.Empty;
                settingsResponse.OkrBaseAddress = okrBaseAddress.Key != null ? okrBaseAddress.Value.FirstOrDefault() : string.Empty;
                settingsResponse.OkrUnlockTime = unlockTime.Key != null ? unlockTime.Value.FirstOrDefault() : string.Empty;
                settingsResponse.FrontEndUrl = frontEndUrl.Key != null ? frontEndUrl.Value.FirstOrDefault() : string.Empty;
                settingsResponse.ResetPassUrl = resetPassUrl.Key != null ? resetPassUrl.Value.FirstOrDefault() : string.Empty;
                settingsResponse.NotificationBaseAddress = notificationUrl.Key != null ? notificationUrl.Value.FirstOrDefault() : string.Empty;

            }
            return settingsResponse;
        }

        #region Private methods
        private string GetTenantId(string token)
        {
            string tenantId = string.Empty;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            if (jsonToken is JwtSecurityToken principal)
                tenantId = principal.Claims.Single(x => x.Type == "tid").Value;

            return tenantId;
        }
        #endregion
    }
}
