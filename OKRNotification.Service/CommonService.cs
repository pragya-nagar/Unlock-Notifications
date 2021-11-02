using System;
using Newtonsoft.Json;
using OKRNotification.EF;
using OKRNotification.Service.Contracts;
using OKRNotification.ViewModel.Response;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace OKRNotification.Service
{
    public class CommonService : BaseService, ICommonService
    {
        public CommonService(IServicesAggregator servicesAggregateService) : base(servicesAggregateService)
        {

        }

        public EmployeeResult GetAllUserFromUsers(string jwtToken)
        {
            var employeeResponse = new EmployeeResult();
            if (jwtToken != "")
            {
                using var httpClient = GetHttpClient(jwtToken);
                using var response = httpClient.GetAsync($"api/User/GetAllusers?pageIndex=1&pageSize=9999").Result;
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = response.Content.ReadAsStringAsync().Result;
                    var user = JsonConvert.DeserializeObject<PayloadCustomList<PageResults<UserResponse>>>(apiResponse);
                    employeeResponse.Results = user.Entity.Records;
                }
            }
            return employeeResponse;
        }


        public async Task<UserIdentity> GetUserIdentity(string jwtToken)
        {
            UserIdentity loginUserDetail = new UserIdentity();
            if (jwtToken != "")
            {
                using var httpClient = GetHttpClient(jwtToken);
                using var response = await httpClient.PostAsync($"api/User/Identity", new StringContent(""));
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<PayloadCustom<UserIdentity>>(apiResponse);
                    loginUserDetail = user.Entity;
                }
            }
            return loginUserDetail;
        }

        public async Task<KeyDetailsResponse> GetKeyDetails(long goalKeyId, string token = null)
        {
            var keyDetails = new KeyDetailsResponse();
            if (token != "")
            {
                using var httpClient = GetHttpClient(token);
                httpClient.BaseAddress = new Uri(Configuration.GetValue<string>("OkrService:BaseUrl"));
                using var response = await httpClient.GetAsync($"api/MyGoals/GoalKeyDetail/" + goalKeyId, HttpCompletionOption.ResponseHeadersRead);
                var payload = JsonConvert.DeserializeObject<PayloadCustom<KeyDetailsResponse>>(await response.Content.ReadAsStringAsync());
                keyDetails = payload.Entity;
            }

            return keyDetails;
        }

    }
}
