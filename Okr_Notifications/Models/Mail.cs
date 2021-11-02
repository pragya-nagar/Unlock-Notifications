using System;
using System.ComponentModel.DataAnnotations;

namespace Okr_Notifications.Models
{
    public class Mail
    {
        [Key]
        public long MailId { get; set; }
        public string MailTo { get; set; }
        public string MailFrom { get; set; } = "adminsupport@unlockokr.com";
        public string Bcc { get; set; } = "";
        public string CC { get; set; } = "";
        public string Subject { get; set; }
        public string Body { get; set; }
        public long CreatedBy { get; set; } = 14254;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
