using Microsoft.AspNetCore.Mvc;
using OKRNotification.Common;
using OKRNotification.EF;
using OKRNotification.Service.Contracts;
using OKRNotification.ViewModel.Request;
using OKRNotification.ViewModel.Response;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OKRNotification.WebCore.Controller
{
    [Route("api/v2/OkrNotifications")]
    [ApiController]
    public class OkrNotificationsController : ApiControllerBase
    {

        private readonly IOkrNotificationsService _okrNotificationsService;
        private readonly ICommonService _commonService;

        public OkrNotificationsController(IOkrNotificationsService okrNotificationsService, ICommonService commonService)
        {
            _okrNotificationsService = okrNotificationsService;
            _commonService = commonService;
        }


        [Route("Notifications/{id}")]
        [HttpPut]

        public async Task<IActionResult> Read(long id)
        {
            if (!IsActiveToken)
                return StatusCode((int)HttpStatusCode.Unauthorized);

            var payloadNotification = new PayloadCustom<string>
            {
                Entity = await _okrNotificationsService.ReadNotificationsForFeedback(id)
            };
            if (payloadNotification.Entity == "")
            {
                payloadNotification.MessageType = MessageTypes.Success.ToString();
                payloadNotification.MessageList.Add("id", "Notifications is read");
                payloadNotification.IsSuccess = true;
                payloadNotification.Status = Response.StatusCode;
            }
            else
            {
                payloadNotification.IsSuccess = false;
                payloadNotification.MessageList.Add("notificationTo", "Notifications is not read");
                payloadNotification.Status = (int)HttpStatusCode.BadRequest;
            }
            return Ok(payloadNotification);
        }

        [Route("Notifications/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteNotifications(long id)
        {
            if (!IsActiveToken)
                return StatusCode((int)HttpStatusCode.Unauthorized);

            var payloadNotification = new PayloadCustom<string>
            {
                Entity = await _okrNotificationsService.DeleteNotifications(id)
            };

            if (payloadNotification.Entity == "")
            {
                payloadNotification.MessageType = MessageTypes.Success.ToString();
                payloadNotification.MessageList.Add("id", "Notifications is deleted");
                payloadNotification.IsSuccess = true;
                payloadNotification.Status = Response.StatusCode;
            }
            else
            {

                payloadNotification.IsSuccess = false;
                payloadNotification.MessageList.Add("notificationTo", "Notifications is not deleted");
                payloadNotification.Status = (int)HttpStatusCode.BadRequest;
            }
            return Ok(payloadNotification);

        }

        [Route("Notifications")]
        [HttpGet]
        public async Task<IActionResult> GetNotificationDetails([FromQuery] NotificationDetailsRequest notificationDetailsRequest)
        {

            if (!IsActiveToken)
                return StatusCode((int)HttpStatusCode.Unauthorized);

            var payloadNotification = new PayloadCustom<NotificationsMessageType>();

            if (notificationDetailsRequest.EmployeeId == 0)
            {

                ModelState.AddModelError("employeeId", "Employee Id cant be 0.");
            }


            if (ModelState.IsValid)
            {
                payloadNotification.Entity =
              await _okrNotificationsService.GetNotificationsDetails(notificationDetailsRequest.EmployeeId, UserToken, notificationDetailsRequest.appId);

                if (payloadNotification.Entity != null)
                {
                    payloadNotification.MessageType = MessageTypes.Success.ToString();
                    payloadNotification.IsSuccess = true;
                    payloadNotification.Status = Response.StatusCode;
                }
                else
                {
                    payloadNotification.MessageType = MessageTypes.Success.ToString();
                    payloadNotification.IsSuccess = true;
                    payloadNotification.MessageList.Add("employeeId", "There is no notifications for particular employee");
                    payloadNotification.Status = Response.StatusCode;
                }
            }
            else
            {
                payloadNotification = GetPayloadStatus(payloadNotification);
            }

            return Ok(payloadNotification);


        }


        [Route("ReadAlerts")]
        [HttpGet]
        public async Task<IActionResult> ReadAlerts(long employeeId)
        {

            if (!IsActiveToken)
                return StatusCode((int)HttpStatusCode.Unauthorized);

            var payloadNotification = new PayloadCustom<string>();

            if (employeeId == 0)
            {

                ModelState.AddModelError("employeeId", "employeeId cant be 0.");
            }

            if (ModelState.IsValid)
            {
                payloadNotification.Entity = _okrNotificationsService.ReadAlerts(employeeId);
                if (payloadNotification.Entity == "")
                {
                    payloadNotification.MessageType = MessageTypes.Success.ToString();
                    payloadNotification.MessageList.Add("employeeId", "Read Alerts");
                    payloadNotification.IsSuccess = true;
                    payloadNotification.Status = Response.StatusCode;
                }
                else
                {

                    payloadNotification.IsSuccess = false;
                    payloadNotification.MessageList.Add("notificationTo", "There is no unread notifications");
                    payloadNotification.Status = (int)HttpStatusCode.BadRequest;
                }
            }
            else
            {
                payloadNotification = GetPayloadStatus(payloadNotification);
            }


            return Ok(payloadNotification);

        }

        [Route("InsertNotificationsDetailsAsync")]
        [HttpPost]
        public async Task<IActionResult> InsertNotificationsDetailsAsync(NotificationsRequest notificationsRequest)
        {
            if (!IsActiveToken)
                return StatusCode((int)HttpStatusCode.Unauthorized);

            var payloadNotification = new PayloadCustom<NotificationsDetails>();

            var insertedId = await _okrNotificationsService.SaveNotificationsDetailsAsync(notificationsRequest);

            if (insertedId > 0)
            {
                payloadNotification.MessageType = MessageTypes.Success.ToString();
                payloadNotification.MessageList.Add("Save", "Inserted Successfully");
                payloadNotification.IsSuccess = true;
                payloadNotification.Status = Response.StatusCode;

            }
            else
            {

                payloadNotification.MessageList.Add("Save", "Inserted Unsuccessfully");
                payloadNotification.Status = (int)HttpStatusCode.BadRequest;
            }

            return Ok(payloadNotification);

        }

        [Route("UpdateNotificationText")]
        [HttpPut]
        public async Task<IActionResult> UpdateNotificationTextAsync(UpdateNotificationTextRequest updateNotificationTextRequest)
        {
            if (!IsActiveToken)
                return StatusCode((int)HttpStatusCode.Unauthorized);

            var payloadNotification = new PayloadCustom<NotificationsDetails>();
            if (updateNotificationTextRequest.NotificationsDetailsId == 0)
            {

                ModelState.AddModelError("NotificationsDetailsId", "NotificationsDetailsId cant be 0.");
            }
            if (updateNotificationTextRequest.NotificationTypeId == 0)
            {

                ModelState.AddModelError("NotificationTypeId", "NotificationTypeId cant be 0.");
            }
            if (ModelState.IsValid)
            {
                var updatedData = await _okrNotificationsService.UpdateNotificationTextAsync(updateNotificationTextRequest);


                if (updatedData != null)
                {
                    payloadNotification.MessageType = MessageTypes.Success.ToString();
                    payloadNotification.MessageList.Add("Text", "Text Updated Successfully");
                    payloadNotification.IsSuccess = true;
                    payloadNotification.Status = Response.StatusCode;
                }
                else
                {
                    payloadNotification.IsSuccess = false;
                    payloadNotification.MessageList.Add("Text", "Something went wrong");
                    payloadNotification.Status = (int)HttpStatusCode.BadRequest;
                }
            }
            return Ok(payloadNotification);
        }


        [Route("UpdateNotificationURL")]
        [HttpPut]

        public async Task<IActionResult> UpdateNotificationURLAsync(UpdateNotificationURL updateNotificationURL)
        {
            if (!IsActiveToken)
                return StatusCode((int)HttpStatusCode.Unauthorized);

            var payloadNotification = new PayloadCustom<NotificationsDetails>();

            var updatedData = await _okrNotificationsService.UpdateNotificationUrlAsync(updateNotificationURL);

            if (updatedData != null)
            {
                payloadNotification.MessageType = MessageTypes.Success.ToString();
                payloadNotification.MessageList.Add("URL", "URL Updated Successfully");
                payloadNotification.IsSuccess = true;
                payloadNotification.Status = Response.StatusCode;
            }
            else
            {
                payloadNotification.IsSuccess = false;
                payloadNotification.MessageList.Add("URL", "Something went wrong");
                payloadNotification.Status = (int)HttpStatusCode.BadRequest;
            }

            return Ok(payloadNotification);
        }



        [Route("GetNotifications")]
        [HttpGet]

        public async Task<IActionResult> GetNotifications(long id)
        {
            if (!IsActiveToken)
                return StatusCode((int)HttpStatusCode.Unauthorized);

            var payloadNotification = new PayloadCustom<NotificationsDetails>();

            var updatedData = await _okrNotificationsService.GetNotificationsById(id);

            if (updatedData != null)
            {
                payloadNotification.MessageType = MessageTypes.Success.ToString();
                payloadNotification.MessageList.Add("Id", "Successfully");
                payloadNotification.IsSuccess = true;
                payloadNotification.Status = Response.StatusCode;
                payloadNotification.Entity = updatedData;
            }
            else
            {
                payloadNotification.IsSuccess = false;
                payloadNotification.MessageList.Add("Id", "Something went wrong");
                payloadNotification.Status = (int)HttpStatusCode.BadRequest;
            }

            return Ok(payloadNotification);
        }


        [Route("GetAllNotifications")]
        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {



            if (!IsActiveToken)
                return StatusCode((int)HttpStatusCode.Unauthorized);



            var payloadNotification = new PayloadCustom<NotificationsDetailsResponse>();



            if (ModelState.IsValid)
            {
                payloadNotification.EntityList = await _okrNotificationsService.GetAllNotifications();
                if (payloadNotification.EntityList.Count > 0)
                {
                    payloadNotification.MessageType = MessageTypes.Success.ToString();
                    payloadNotification.IsSuccess = true;
                    payloadNotification.Status = Response.StatusCode;
                }
                else
                {



                    payloadNotification.IsSuccess = false;
                    payloadNotification.MessageList.Add("notification", "There are no notifications");
                    payloadNotification.Status = (int)HttpStatusCode.BadRequest;
                }
            }
            else
            {
                payloadNotification = GetPayloadStatus(payloadNotification);
            }




            return Ok(payloadNotification);



        }


    }
}
