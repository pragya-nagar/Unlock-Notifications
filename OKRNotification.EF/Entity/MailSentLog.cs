using System;
using System.Diagnostics.CodeAnalysis;

namespace OKRNotification.EF
{
    [ExcludeFromCodeCoverage]
    public partial class MailSentLog
    {
        public long MailSentLogId { get; set; }
        public string MailTo { get; set; }
        public string MailFrom { get; set; } = "adminsupport@unlockokr.com";
        public string Cc { get; set; } = "";
        public string Bcc { get; set; } = "";
        public string MailSubject { get; set; }
        public string Body { get; set; }
        public long? CreatedBy { get; set; } = 14254;
        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? MailSentOn { get; set; }
        public bool? IsMailSent { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
