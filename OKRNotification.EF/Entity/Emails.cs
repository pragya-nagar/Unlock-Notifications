

using System.Diagnostics.CodeAnalysis;

namespace OKRNotification.EF
{
    [ExcludeFromCodeCoverage]
    public partial class Emails
    {
        public long Id { get; set; }
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
    }
}
