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
    public class OkrNotificationControllerUnitTest
    {
        private readonly Mock<IOkrNotificationsService> _okrNotificationsService;
        private readonly Mock<ICommonService> _commonService;
        private readonly OkrNotificationsController _okrNotificationsController;
        public OkrNotificationControllerUnitTest()
        {
            _okrNotificationsService = new Mock<IOkrNotificationsService>();
            _commonService = new Mock<ICommonService>();
            _okrNotificationsController = new OkrNotificationsController(_okrNotificationsService.Object, _commonService.Object);
            SetUserClaimsAndRequest();
        }

        [Fact]
        public async Task Read_InvalidToken()
        {
            ///arrange
            long employeeId = 795;

            ///act
            _okrNotificationsService.Setup(serv => serv.ReadNotificationsForFeedback(It.IsAny<long>())).ReturnsAsync("");
            

            ///assert
            var result = await _okrNotificationsController.Read(employeeId) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task Read_ValidToken()
        {
            ///arrange
            long employeeId = 795;
            UserIdentity userIdentity = new UserIdentity();
            ///act
            _okrNotificationsService.Setup(serv => serv.ReadNotificationsForFeedback(It.IsAny<long>())).ReturnsAsync("");
            _commonService.Setup(e => e.GetUserIdentity(It.IsAny<string>())).ReturnsAsync(userIdentity);
            ///assert
            var result = await _okrNotificationsController.Read(employeeId);
            Assert.NotNull(result);

        }

        [Fact]
        public async Task DeleteNotifications_InvalidToken()
        {
            ///arrange
            long employeeId = 795;
            
            ///act
            _okrNotificationsService.Setup(serv => serv.DeleteNotifications(It.IsAny<long>())).ReturnsAsync("");
            

            ///assert
            var result = await _okrNotificationsController.DeleteNotifications(employeeId) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task DeleteNotifications_ValidToken()
        {
            ///arrange
            long employeeId = 795;
            
            UserIdentity userIdentity = new UserIdentity();
            ///act
            _okrNotificationsService.Setup(serv => serv.DeleteNotifications(It.IsAny<long>())).ReturnsAsync("");
            _commonService.Setup(e => e.GetUserIdentity(It.IsAny<string>())).ReturnsAsync(userIdentity);
            ///assert
            var result = await _okrNotificationsController.DeleteNotifications(employeeId);
            Assert.NotNull(result);

        }


        [Fact]
        public async Task GetNotificationDetails_InvalidToken()
        {
            ///arrange
            
            NotificationsMessageType notificationsMessageType = new NotificationsMessageType();
            NotificationDetailsRequest notificationDetailsRequest = new NotificationDetailsRequest();
            ///act
            _okrNotificationsService.Setup(serv => serv.GetNotificationsDetails(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<List<int>>())).ReturnsAsync(notificationsMessageType);
            

            ///assert
            var result = await _okrNotificationsController.GetNotificationDetails(notificationDetailsRequest) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task GetNotificationDetails_ValidToken()
        {
            ///arrange
            
            UserIdentity userIdentity = new UserIdentity();
            NotificationsMessageType notificationsMessageType = new NotificationsMessageType();
            NotificationDetailsRequest notificationDetailsRequest = new NotificationDetailsRequest();
            ///act
            _okrNotificationsService.Setup(serv => serv.GetNotificationsDetails(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<List<int>>())).ReturnsAsync(notificationsMessageType);
            _commonService.Setup(serv => serv.GetUserIdentity(It.IsAny<string>())).ReturnsAsync(userIdentity);

            ///assert
            var result = await _okrNotificationsController.GetNotificationDetails(notificationDetailsRequest);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ReadAlerts_InvalidToken()
        {
            ///arrange
            long employeeId = 795;
            
            ///act
            _okrNotificationsService.Setup(serv => serv.ReadAlerts(It.IsAny<long>())).Returns("");

            ///assert
            var result = await _okrNotificationsController.ReadAlerts(employeeId) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task ReadAlerts_ValidToken()
        {
            ///arrange
            long employeeId = 795;
            UserIdentity userIdentity = new UserIdentity();
            
            ///act
            _okrNotificationsService.Setup(serv => serv.ReadAlerts(It.IsAny<long>())).Returns("");
            _commonService.Setup(serv => serv.GetUserIdentity(It.IsAny<string>())).ReturnsAsync(userIdentity);

            ///assert
            var result = await _okrNotificationsController.ReadAlerts(employeeId);
            Assert.NotNull(result);
        }


        [Fact]
        public async Task InsertNotificationsDetailsAsync_InvalidToken()
        {
            ///arrange
            long employeeId = 795;
            
            NotificationsRequest notificationsRequest = new NotificationsRequest();
            ///act
            _okrNotificationsService.Setup(serv => serv.SaveNotificationsDetailsAsync(It.IsAny<NotificationsRequest>())).ReturnsAsync(employeeId);
            

            ///assert
            var result = await _okrNotificationsController.InsertNotificationsDetailsAsync(notificationsRequest) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task InsertNotificationsDetailsAsync_ValidToken()
        {
            ///arrange
            long employeeId = 795;
            UserIdentity userIdentity = new UserIdentity();
            
            NotificationsRequest notificationsRequest = new NotificationsRequest();
            ///act
            _okrNotificationsService.Setup(serv => serv.SaveNotificationsDetailsAsync(It.IsAny<NotificationsRequest>())).ReturnsAsync(employeeId);
            _commonService.Setup(serv => serv.GetUserIdentity(It.IsAny<string>())).ReturnsAsync(userIdentity);

            ///assert
            var result = await _okrNotificationsController.InsertNotificationsDetailsAsync(notificationsRequest);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateNotificationsText_InvalidToken()
        {
            ///arrange
            UpdateNotificationTextRequest updateNotificationTextRequest = new UpdateNotificationTextRequest();
            NotificationsDetails notificationsDetails = new NotificationsDetails();

            ///act
            _okrNotificationsService.Setup(serv => serv.UpdateNotificationTextAsync(It.IsAny<UpdateNotificationTextRequest>())).ReturnsAsync(notificationsDetails);

            ///assert
            var result = await _okrNotificationsController.UpdateNotificationTextAsync(updateNotificationTextRequest) as StatusCodeResult;
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Fact]
        public async Task UpdateNotificationsTex_ValidToken()
        {
            ///arrange
            UpdateNotificationTextRequest updateNotificationTextRequest = new UpdateNotificationTextRequest();
            NotificationsDetails notificationsDetails = new NotificationsDetails();

            UserIdentity userIdentity = new UserIdentity();
            ///act
            _okrNotificationsService.Setup(serv => serv.UpdateNotificationTextAsync(It.IsAny<UpdateNotificationTextRequest>())).ReturnsAsync(notificationsDetails);
            _commonService.Setup(e => e.GetUserIdentity(It.IsAny<string>())).ReturnsAsync(userIdentity);
            ///assert
            var result = await _okrNotificationsController.UpdateNotificationTextAsync(updateNotificationTextRequest);
            Assert.NotNull(result);

        }

        #region Private region
        private void SetUserClaimsAndRequest()
        {
            _okrNotificationsController.ControllerContext = new ControllerContext();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "108"),
                new Claim(ClaimTypes.Role, "108"),
                new Claim(ClaimTypes.NameIdentifier, "108"),
                new Claim(ClaimTypes.Email, "abcd@gmail.com")
            };

            var identity = new ClaimsIdentity(claims, "108");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _okrNotificationsController.ControllerContext.HttpContext = new DefaultHttpContext()
            {
                User = claimsPrincipal
            };

            string sampleAuthToken = Guid.NewGuid().ToString();
            _okrNotificationsController.ControllerContext.HttpContext.Request.Headers["Authorization"] = "Bearer " + sampleAuthToken;
        }
        #endregion
    }
}

