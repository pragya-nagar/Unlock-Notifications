using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using OKRNotification.EF;
using OKRNotification.Service.Contracts;
using OKRNotification.ViewModel.Request;

namespace OKRNotification.Service
{
    [ExcludeFromCodeCoverage]
    public class EmailService : BaseService,IEmailService
    {
        private readonly IRepositoryAsync<Emails> emailRepo;
        private readonly IRepositoryAsync<Mail> mailRepo;
        private readonly IRepositoryAsync<ErrorLog> errorLogRepo;
        private readonly IRepositoryAsync<MailerTemplate> mailerTemplateRepo;
        private readonly IRepositoryAsync<MailSetupConfig> mailSetupRepo;
        private readonly IRepositoryAsync<MailSentLog> mailSentRepo;
        

        public EmailService(IServicesAggregator servicesAggregateService) : base(servicesAggregateService)
        {
            emailRepo = UnitOfWorkAsync.RepositoryAsync<Emails>();
            mailRepo = UnitOfWorkAsync.RepositoryAsync<Mail>();
            errorLogRepo = UnitOfWorkAsync.RepositoryAsync<ErrorLog>();
            mailerTemplateRepo = UnitOfWorkAsync.RepositoryAsync<MailerTemplate>();
            mailSetupRepo = UnitOfWorkAsync.RepositoryAsync<MailSetupConfig>();
            mailSentRepo = UnitOfWorkAsync.RepositoryAsync<MailSentLog>();
           
        }


     
       

        //public void SaveMail(MailRequest mailRequest)
        //{
        //    Mail mail = new Mail
        //    {
        //        Subject = mailRequest.Subject,
        //        MailTo = mailRequest.MailTo,
        //        Body = mailRequest.Body,
        //        Bcc = mailRequest.Bcc,
        //        Cc = mailRequest.CC
        //    };
        //    _emailDataProvider.SaveMail(mail);
        //}

        public async Task<Mail> InsertMail(Mail mail)
        {
            mailRepo.Add(mail);
            await UnitOfWorkAsync.SaveChangesAsync();
            return mail;
        }


        public async Task SaveMailAsync(MailRequest mailRequest)
        {
            Mail mail = new Mail
            {
                Subject = mailRequest.Subject,
                ///  mail.MailFrom = mailRequest.MailFrom;
                MailTo = mailRequest.MailTo,
                Body = mailRequest.Body,
                Bcc = mailRequest.Bcc,
                Cc = mailRequest.CC
            };
            await InsertMail(mail);
        }

        //public void MailLog(MailLogRequest mailLog)
        //{
        //    MailSentLog mailSentLog = new MailSentLog();
        //    mailSentLog.MailSubject = mailLog.MailSubject;
        //    /// mailSentLog.MailFrom = mailLog.MailFrom;
        //    mailSentLog.MailTo = mailLog.MailTo;
        //    mailSentLog.Body = mailLog.Body;
        //    mailSentLog.Bcc = mailLog.Bcc;
        //    mailSentLog.CC = mailLog.CC;
        //    mailSentLog.MailSentOn = mailLog.MailSentOn;
        //    mailSentLog.IsMailSent = mailLog.IsMailSent;
        //    _emailDataProvider.MailLog(mailSentLog);

        //}

        public async Task MailLogAsync(MailLogRequest mailLog)
        {
            MailSentLog mailSentLog = new MailSentLog
            {
                MailSubject = mailLog.MailSubject,
                ///  mailSentLog.MailFrom = mailLog.MailFrom;
                MailTo = mailLog.MailTo,
                Body = mailLog.Body,
                Bcc = mailLog.Bcc,
                Cc = mailLog.CC,
                MailSentOn = mailLog.MailSentOn,
                IsMailSent = mailLog.IsMailSent
            };
            await InsertMailSentLog(mailSentLog);
        }

        public async Task<MailSentLog> InsertMailSentLog(MailSentLog mailSentLog)
        {
            mailSentRepo.Add(mailSentLog);
            await UnitOfWorkAsync.SaveChangesAsync();
            return mailSentLog;
        }

        public async Task<List<Emails>> GetEmailAddress()
        {
             return await emailRepo.GetQueryable().ToListAsync();
            
        }

        public async Task<MailSetupConfig> IsMailExist(string emailId)
        {
            return await mailSetupRepo.GetQueryable().FirstOrDefaultAsync(x => x.AwsemailId == emailId && (bool)x.IsActive);

        }

        public async Task<bool> SentMailAsync(MailRequest mailRequest)
        {
            bool IsMailSent = false;
            MailLogRequest log = new MailLogRequest();
     
                try
                {
                    MimeMessage message = new MimeMessage();
                    var mailCC = Configuration.GetValue<string>("Mail:CC");
                    var emailTag = Configuration.GetValue<string>("Mail:EmailTag");
                    var emailUser = Configuration.GetValue<string>("Mail:UserTag");
                    string azureEmailId = Configuration.GetValue<string>("SMTPDetails:AzureEmailID");
                    string account = Configuration.GetValue<string>("SMTPDetails:AccountName");
                    string password = Configuration.GetValue<string>("SMTPDetails:Password");

                    int port = Configuration.GetValue<int>("SMTPDetails:Port");

                    string host = Configuration.GetValue<string>("SMTPDetails:Host");
                    string environment = Configuration.GetValue<string>("Mail:Environment");

                    if (string.IsNullOrWhiteSpace(mailRequest.MailFrom) && mailRequest.MailFrom == "")
                    {
                        MailboxAddress from = new MailboxAddress(emailTag, azureEmailId);
                        message.From.Add(from);
                    }
                    else
                    {
                        var isMailExist = IsMailExist(mailRequest.MailFrom);
                        if (isMailExist != null)
                        {
                            MailboxAddress mailboxAddress = new MailboxAddress(emailUser, mailRequest.MailFrom);
                            message.From.Add(mailboxAddress);
                        }
                    }

                    MailboxAddress From = new MailboxAddress(emailTag, azureEmailId);
                    message.From.Add(From);


                    if (environment != "LIVE")
                    {
                        mailRequest.Subject = mailRequest.Subject + " - " + environment + " This mail is for " + mailRequest.MailTo;

                        var emails = await GetEmailAddress();
                        foreach (var address in from item in emails
                                                let address = new MailboxAddress(item.FullName, item.EmailAddress)
                                                select address)
                        {
                            message.To.Add(address);
                        }
                        MailboxAddress CC = new MailboxAddress(mailCC);
                        message.Cc.Add(CC);
                    }

                    else if (environment == "LIVE")
                    {
                        string[] strTolist = mailRequest.MailTo.Split(';');

                        foreach (var item in strTolist)
                        {
                            MailboxAddress mailto = new MailboxAddress(item);
                            message.To.Add(mailto);
                        }


                        if (mailRequest.Bcc != "")
                        {
                            string[] strbcclist = mailRequest.CC.Split(';');
                            foreach (var item in strbcclist)
                            {
                                MailboxAddress bcc = new MailboxAddress(item);
                                message.Bcc.Add(bcc);
                            }
                        }

                        if (mailRequest.CC != "")
                        {
                            string[] strCcList = mailRequest.CC.Split(';');
                            foreach (var item in strCcList)
                            {
                                MailboxAddress CC = new MailboxAddress(item);
                                message.Cc.Add(CC);
                            }
                        }
                    }


                    message.Subject = mailRequest.Subject;
                    BodyBuilder bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = mailRequest.Body;
                    message.Body = bodyBuilder.ToMessageBody();


                    await SaveMailAsync(mailRequest);

                    if (message.Subject != "")
                    {
                        SmtpClient client = new SmtpClient();
                        client.Connect(host, port, false);
                        client.Authenticate(account, password);
                        client.Send(message);
                        client.Disconnect(true);
                        client.Dispose();
                        log.MailTo = mailRequest.MailTo;

                        log.Body = mailRequest.Body;
                        log.MailSubject = mailRequest.Subject;
                        log.CC = mailRequest.CC;
                        log.Bcc = mailRequest.Bcc;
                        log.IsMailSent = true;
                        log.MailSentOn = DateTime.UtcNow;
                        IsMailSent = true;
                        await MailLogAsync(log);
                    }

                }
                catch (Exception e)
                {
                    log.MailTo = mailRequest.MailTo;

                    log.Body = mailRequest.Body;
                    log.MailSubject = mailRequest.Subject;
                    log.CC = mailRequest.CC;
                    log.Bcc = mailRequest.Bcc;
                    log.IsMailSent = false;
                    log.MailSentOn = null;
                    await MailLogAsync(log);
                    IsMailSent = false;

                }


            return IsMailSent;
        }

        public async Task<MailerTemplate> GetMailerTemplateAsync(string templateCode)
        {
            return await mailerTemplateRepo.GetQueryable().FirstOrDefaultAsync(x => x.TemplateCode.Equals(templateCode));
        }
        public void SaveLog(string pageName, string functionName, string errorDetail)
        {
            var errorLog = new ErrorLog
            {
                PageName = pageName,
                FunctionName = functionName,
                ErrorDetail = errorDetail,
                ApplicationName = " Okr_Notifications"
            };
            errorLogRepo.Add(errorLog);
            UnitOfWorkAsync.SaveChanges();
        }
    }
}
