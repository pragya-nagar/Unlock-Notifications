using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Okr_Notifications.Models;

namespace Okr_Notifications.DataProvider
{

    public class EmailDataProvider : IEmailDataProvider
    {
        private readonly Okr_NotificationsDbContext _database;

        public EmailDataProvider(Okr_NotificationsDbContext database)
        {
            _database = database;
        }

        public void SaveLog(string pageName, string functionName, string applicationName, string errorDetail)
        {
            ErrorLog errorLog = new ErrorLog();
            errorLog.PageName = pageName;
            errorLog.FunctionName = functionName;
            errorLog.ApplicationName = applicationName;
            errorLog.ErrorDetail = errorDetail;
            _database.ErrorLog.Add(errorLog);
            _database.SaveChanges();
        }

        public async Task<MailerTemplate> GetMailerTemplateAsync(string templateCode)
        {
            MailerTemplate mailerTemplate = new MailerTemplate();
            mailerTemplate = await _database.MailerTemplate.FirstOrDefaultAsync(x => x.TemplateCode.Equals(templateCode));
            return mailerTemplate;
        }

        public void SaveMail(Mail mailResponse)
        {
            _database.Mail.Add(mailResponse);
            _database.SaveChanges();
        }

        public async Task SaveMailAsync(Mail mailResponse)
        {
            await _database.Mail.AddAsync(mailResponse);
            await _database.SaveChangesAsync();
        }

        public void MailLog(MailSentLog log)
        {
            _database.MailSentLog.Add(log);
            _database.SaveChanges();
        }

        public async Task MailLogAsync(MailSentLog log)
        {
            await _database.MailSentLog.AddAsync(log);
            await _database.SaveChangesAsync();
        }

        public async Task<List<Emails>> GetEmailAddress()
        {
            var emails = await _database.Emails.ToListAsync();
            return emails;
        }

        public async Task<MailSetupConfig> IsMailExist(string emailId)
        {
            var email = await _database.MailSetupConfig.FirstOrDefaultAsync(x => x.AWSEmailId == emailId);
            return email;
        }
    }
}
