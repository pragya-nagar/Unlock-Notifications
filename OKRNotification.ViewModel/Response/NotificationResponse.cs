using System;

namespace OKRNotification.ViewModel.Response
{
    public class NotificationResponse
    {
        public long Id { get; set; }
        public long NotificationsBy { get; set; }
        public long NotificationsTo { get; set; }
        public string NotificationsMessage { get; set; }
        public int AppId { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }
        public long NotificationTypeId { get; set; }
        public long MessageType { get; set; }
        public string Url { get; set; }
        public string ImagePath { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string GoalKeyId { get; set; }
        public long OkrId { get; set; }
        public long KrId { get; set; }
        public int NotificationOnTypeId { get; set; }
    }
}
