using System.Threading.Tasks;
using OKRNotification.ViewModel.Response;

namespace OKRNotification.Service.Contracts
{
    public interface IKeyVaultService
    {
    
        Task<ServiceSettingUrlResponse> GetSettingsAndUrlsAsync();
    }
}
