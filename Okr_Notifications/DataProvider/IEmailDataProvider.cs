using System.Collections.Generic;
using System.Threading.Tasks;
using Okr_Notifications.Models;

namespace Okr_Notifications.DataProvider
{
    public interface IEmailDataProvider
    {
        void SaveLog(string pageName, string functionName, string applicationName, string errorDetail);
        void SaveMail(Mail mailResponse);
        void MailLog(MailSentLog log);
        Task MailLogAsync(MailSentLog log);
        Task SaveMailAsync(Mail mailResponse);
        Task<MailerTemplate> GetMailerTemplateAsync(string templateCode);
        Task<List<Emails>> GetEmailAddress();
        Task<MailSetupConfig> IsMailExist(string emailId);
    }
}
