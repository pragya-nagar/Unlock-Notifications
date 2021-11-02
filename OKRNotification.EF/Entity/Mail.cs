using System;
using System.Diagnostics.CodeAnalysis;

namespace OKRNotification.EF
{
    [ExcludeFromCodeCoverage]
    public partial class Mail
    {
        public long MailId { get; set; }
        public string MailTo { get; set; }
        public string MailFrom { get; set; } = "adminsupport@unlockokr.com";
        public string Cc { get; set; } = "";
        public string Bcc { get; set; } = "";
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
        public long? CreatedBy { get; set; } = 14254;
        public bool? IsActive { get; set; } = true;
    }
}
