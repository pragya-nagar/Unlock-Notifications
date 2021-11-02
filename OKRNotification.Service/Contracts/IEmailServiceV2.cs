
using OKRNotification.EF;
using OKRNotification.ViewModel.Request;
using System.Threading.Tasks;

namespace OKRNotification.Service.Contracts
{
    public interface IEmailServiceV2
    {
        Task<MailerTemplate> GetMailerTemplateAsync(string templateCode);
        Task SaveMailAsync(MailRequest mailRequest);
        Task MailLogAsync(MailLogRequest mailLog);
        Task<bool> SentMailAsync(MailRequest mailRequest);
        void SaveLog(string pageName, string functionName, string errorDetail);
    }
}
