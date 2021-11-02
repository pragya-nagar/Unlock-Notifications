using OKRNotification.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OKRNotification.Service.Contracts
{
    public interface ICommonService
    {
        Task<UserIdentity> GetUserIdentity(string jwtToken);
        EmployeeResult GetAllUserFromUsers(string jwtToken);
        Task<KeyDetailsResponse> GetKeyDetails(long goalKeyId, string token);
    }
}
