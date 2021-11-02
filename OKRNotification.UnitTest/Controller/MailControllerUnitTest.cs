using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OKRNotification.EF;
using OKRNotification.Service.Contracts;
using OKRNotification.ViewModel.Request;
using OKRNotification.ViewModel.Response;
using OKRNotification.WebCore.Controller;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace OKRNotification.UnitTest.Controller
{
    public class MailControllerUnitTest
    {
        private readonly Mock<IEmailServiceV2> _mailService;
        private readonly Mock<ICommonService> _commonService;
        private readonly MailController _mailController;

        public MailControllerUnitTest()
        {
            _mailService = new Mock<IEmailServiceV2>();
            _commonService = new Mock<ICommonService>();
            _mailController = new MailController(_mailService.Object, _commonService.Object);
            SetUserClaimsAndRequest();
        }

        [Fact]
        public async Task GetTemplate_InvalidToken()
        {
            ///arrange
            string templateCode = "PF";
            
            MailerTemplate mailerTemplate = new MailerTemplate();
            ///act
            _mailService.Setup(serv => serv.GetMailerTemplateAsync(It.IsAny<string>())).ReturnsAsync(mailerTemplate);
            

            ///assert
            var result = await _mailController.GetTemplate(templateCode) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);

        }


        [Fact]
        public async Task GetTemplate_ValidToken()
        {
            ///arrange
            string templateCode = "PF";
            
            UserIdentity userIdentity = new UserIdentity();
            MailerTemplate mailerTemplate = new MailerTemplate();
            ///act
            _mailService.Setup(serv => serv.GetMailerTemplateAsync(It.IsAny<string>())).ReturnsAsync(mailerTemplate);
            _commonService.Setup(e => e.GetUserIdentity(It.IsAny<string>())).ReturnsAsync(userIdentity);
            ///assert
            var result = await _mailController.GetTemplate(templateCode);
            Assert.NotNull(result);

        }

        [Fact]
        public async Task SentMailAsync_InvalidToken()
        {
            ///arrange
            
            MailRequest mailRequest = new MailRequest();
            ///act
            _mailService.Setup(serv => serv.SentMailAsync(It.IsAny<MailRequest>())).ReturnsAsync(true);
            

            ///assert
            var result = await _mailController.SentMailAsync(mailRequest) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);

        }

        [Fact]
        public async Task SentMailAsync_ValidToken()
        {
            ///arrange
            
            MailRequest mailRequest = new MailRequest();
            UserIdentity userIdentity = new UserIdentity();
            ///act
            _mailService.Setup(serv => serv.SentMailAsync(It.IsAny<MailRequest>())).ReturnsAsync(true);
            _commonService.Setup(serv => serv.GetUserIdentity(It.IsAny<string>())).ReturnsAsync(userIdentity);

            ///assert
            var result = await _mailController.SentMailAsync(mailRequest);
            Assert.NotNull(result);

        }

        #region Private region
        private void SetUserClaimsAndRequest()
        {
            _mailController.ControllerContext = new ControllerContext();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "108"),
                new Claim(ClaimTypes.Role, "108"),
                new Claim(ClaimTypes.NameIdentifier, "108"),
                new Claim(ClaimTypes.Email, "abcd@gmail.com")
            };

            var identity = new ClaimsIdentity(claims, "108");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _mailController.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = claimsPrincipal
            };

            string sampleAuthToken = Guid.NewGuid().ToString();
            _mailController.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer " + sampleAuthToken;
        }
        #endregion
    }
}
