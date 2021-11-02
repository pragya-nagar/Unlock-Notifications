﻿using System;

namespace OKRNotification.ViewModel.Request
{
    public class MailLogRequest
    {
        public string MailTo { get; set; }
        public string MailFrom { get; set; }
        public string Bcc { get; set; }
        public string CC { get; set; }
        public string MailSubject { get; set; }
        public string Body { get; set; }
        public DateTime? MailSentOn { get; set; }
        public bool IsMailSent { get; set; }
    }
}
