using System;
using System.Collections.Generic;
using System.Text;

namespace OKRNotification.ViewModel.Request
{
   public class UpdateNotificationURL
    {
        public long NotificationsDetailsId { get; set; }
        public string URL { get; set; }
    }
}
