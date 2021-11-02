using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OKRNotification.EF
{
    [ExcludeFromCodeCoverage]
    public partial class MessageType
    {
        public MessageType()
        {
            NotificationsDetails = new HashSet<NotificationsDetails>();
        }

        public long MessageTypeId { get; set; }
        public string Message { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? Isdeleted { get; set; }

        public virtual ICollection<NotificationsDetails> NotificationsDetails { get; set; }
    }
}
