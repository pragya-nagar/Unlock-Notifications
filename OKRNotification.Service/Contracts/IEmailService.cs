using System.Threading.Tasks;
using OKRNotification.EF;
using OKRNotification.ViewModel.Request;

namespace OKRNotification.Service.Contracts
{
    public interface IEmailService
    {
       
        Task SaveMailAsync(MailRequest mailRequest);
        Task MailLogAsync(MailLogRequest mailLog);
        Task<bool> SentMailAsync(MailRequest mailRequest);
        Task<MailerTemplate> GetMailerTemplateAsync(string templateCode);
        void SaveLog(string pageName, string functionName, string errorDetail);
    }
}
