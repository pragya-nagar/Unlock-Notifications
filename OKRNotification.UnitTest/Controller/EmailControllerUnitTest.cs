using Microsoft.AspNetCore.Mvc;
using Moq;
using OKRNotification.EF;
using OKRNotification.Service.Contracts;
using OKRNotification.ViewModel.Request;
using OKRNotification.WebCore.Controller;
using System.Threading.Tasks;
using Xunit;

namespace OKRNotification.UnitTest.Controller
{
    public class EmailControllerUnitTest
    {
        private readonly Mock <IEmailService> _emailService;
       
        private readonly EmailController _mailController;

        public EmailControllerUnitTest()
        {
            _emailService = new Mock<IEmailService>();
        
            _mailController = new EmailController(_emailService.Object);
           
        }

        [Fact]
        public async Task GetTemplate_Successful()
        {
            ///arrange
            string templateCode = "PF";
            MailerTemplate mailerTemplate = new MailerTemplate();
            mailerTemplate.Body = "html";
            ///act
            _emailService.Setup(serv => serv.GetMailerTemplateAsync(It.IsAny<string>())).ReturnsAsync(mailerTemplate);
           

            ///assert
            var result = await _mailController.GetTemplateAsync(templateCode) as StatusCodeResult;
            Assert.NotNull(mailerTemplate);

        }

        [Fact]
        public async Task SentMailAsync_Successful()
        {
            ///arrange
            
            MailRequest mailRequest = new MailRequest();
            mailRequest.Body = "html";
            ///act
            _emailService.Setup(serv => serv.SentMailAsync(It.IsAny<MailRequest>())).ReturnsAsync(true);
            

            ///assert
            var result = await _mailController.SentMailAsync(mailRequest) as StatusCodeResult;
            Assert.NotNull(mailRequest);

        }
    }
}
