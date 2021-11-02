using System.Collections.Generic;

namespace OKRNotification.ViewModel.Request
{
    public class NotificationDetailsRequest
    {
        public long EmployeeId { get; set; }
        public List<int> appId { get; set; }
    }
}
