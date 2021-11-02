using System;
using System.Diagnostics.CodeAnalysis;

namespace OKRNotification.EF
{
    [ExcludeFromCodeCoverage]
    public partial class ErrorLog
    {
        public long ErrorLogId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string PageName { get; set; }
        public string FunctionName { get; set; }
        public string ApplicationName { get; set; }
        public string ErrorDetail { get; set; }
    }
}
