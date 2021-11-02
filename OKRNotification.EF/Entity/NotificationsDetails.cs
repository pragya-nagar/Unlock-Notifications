using System;
using System.Diagnostics.CodeAnalysis;

namespace OKRNotification.EF
{
    [ExcludeFromCodeCoverage]
    public partial class NotificationsDetails
    {
        public long NotificationsDetailsId { get; set; }
        public long NotificationsBy { get; set; }
        public long NotificationsTo { get; set; }
        public string NotificationsMessage { get; set; }
        public int ApplicationMasterId { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public long NotificationTypeId { get; set; }
        public long MessageTypeId { get; set; }
        public string Url { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; } = null;


        public int NotificationOnTypeId { get; set; }

        public long NotificationOnId { get; set; }


        public virtual ApplicationMaster ApplicationMaster { get; set; }
        public virtual MessageType MessageType { get; set; }
        public virtual NotificationType NotificationType { get; set; }
    }
}
