

using OKRNotification.EF;
using System.Collections.Generic;


namespace OKRNotification.ViewModel.Response
{
    public class NotificationsMessageType
    {
        public List<UnreadNotifications> UnreadNotifications { get; set; }
        public List<NotificationResponse> GetMessage { get; set; }
        public List<NotificationsDetails> GetSystem { get; set; }
        public int TotalUnreadNotifications { get; set; }
    }

    public class UnreadNotifications
    {
        public int CountUnreadNotifications { get; set; }
    }
}
