using System.Net.Http;
using OKRNotification.EF;

namespace OKRNotification.Service.Contracts
{
    public interface IBaseService
    {
        IUnitOfWorkAsync UnitOfWorkAsync { get; set; }
        IOperationStatus OperationStatus { get; set; }
        NotificationDbContext NotificationDbContext { get; set; }
        string ConnectionString { get; set; }
        HttpClient GetHttpClient(string jwtToken);

    }
}
