using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OKRNotification.EF
{
    [ExcludeFromCodeCoverage]
    public partial class ApplicationMaster
    {
        public ApplicationMaster()
        {
            NotificationsDetails = new HashSet<NotificationsDetails>();
        }

        public int ApplicationMasterId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public string AppName { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<NotificationsDetails> NotificationsDetails { get; set; }
    }
}
