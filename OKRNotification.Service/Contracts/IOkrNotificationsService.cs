using OKRNotification.EF;
using OKRNotification.ViewModel.Request;
using OKRNotification.ViewModel.Response;
using System.Collections.Generic;

using System.Threading.Tasks;


namespace OKRNotification.Service.Contracts
{
    public interface IOkrNotificationsService
    {
        Task<NotificationsMessageType> GetNotificationsDetails(long employeeId, string jwtToken, List<int> appIds);
        Task<string> DeleteNotifications(long id);
        Task<string> ReadNotificationsForFeedback(long id);
        string ReadAlerts(long to);
        Task<long> SaveNotificationsDetailsAsync(NotificationsRequest notificationsRequest);
        void SaveLog(string pageName, string functionName, string errorDetail);
        Task<NotificationsDetails> UpdateNotificationTextAsync(UpdateNotificationTextRequest updateNotificationTextRequest);
        Task<NotificationsDetails> UpdateNotificationUrlAsync(UpdateNotificationURL updateNotificationURL);
        Task<NotificationsDetails> GetNotificationsById(long id);
        Task<List<NotificationsDetailsResponse>> GetAllNotifications();
    }
}
