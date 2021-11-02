using System;
using System.Diagnostics.CodeAnalysis;

namespace OKRNotification.EF
{
    [ExcludeFromCodeCoverage]
    public partial class MailerTemplate
    {
        public long MailerTemplateId { get; set; }
        public string TemplateName { get; set; }
        public string TemplateCode { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
        public bool? IsActive { get; set; }
    }
}
