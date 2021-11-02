using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OKRNotification.EF
{
    [ExcludeFromCodeCoverage]
    public partial class NotificationType
    {
        public NotificationType()
        {
            NotificationsDetails = new HashSet<NotificationsDetails>();
        }

        public long NotificationTypeId { get; set; }
        public string Notification { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? Isdeleted { get; set; }
        public string NotificationCode { get; set; }

        public virtual ICollection<NotificationsDetails> NotificationsDetails { get; set; }
    }
}
