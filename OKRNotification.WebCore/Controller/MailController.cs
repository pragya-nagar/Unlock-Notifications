using Microsoft.AspNetCore.Mvc;
using OKRNotification.Common;
using OKRNotification.EF;
using OKRNotification.Service.Contracts;
using OKRNotification.ViewModel.Request;
using System.Net;
using System.Threading.Tasks;

namespace OKRNotification.WebCore.Controller
{
    [Route("api/v2/OkrNotifications")]
    [ApiController]
    public class MailController : ApiControllerBase
    {
        private readonly IEmailServiceV2 _emailService;
        private readonly ICommonService _commonService;

        public MailController(IEmailServiceV2 emailService, ICommonService commonService)
        {
            _emailService = emailService;
            _commonService = commonService;
        }

        [Route("GetTemplate")]
        [HttpGet]
        public async Task<IActionResult> GetTemplate(string templateCode)
        {
            if (!IsActiveToken)
                return StatusCode((int)HttpStatusCode.Unauthorized);

            var payloadNotification = new PayloadCustom<MailerTemplate>
            {
                Entity = await _emailService.GetMailerTemplateAsync(templateCode)
            };

            if (payloadNotification.Entity != null)
            {
                payloadNotification.MessageType = MessageTypes.Success.ToString();
                payloadNotification.IsSuccess = true;
                payloadNotification.Status = Response.StatusCode;
            }
            else
            {
                payloadNotification.MessageType = MessageTypes.Warning.ToString();
                payloadNotification.IsSuccess = false;
            }

            return Ok(payloadNotification);
        }

        [Route("SentMailAsync")]
        [HttpPost]
        public async Task<IActionResult> SentMailAsync([FromBody] MailRequest mailRequest)
        {
            
            if (!IsActiveToken)
                return StatusCode((int)HttpStatusCode.Unauthorized);

            var payloadNotification = new PayloadCustom<bool>();

            if (mailRequest.Subject == "")
            {
                ModelState.AddModelError("Subject", "Subject cant be blank.");
            }
            else if (mailRequest.MailTo == "")
            {
                ModelState.AddModelError("MailTo", "MailTo cant be blank.");
            }

            if (ModelState.IsValid)
            {
                payloadNotification.IsSuccess = await _emailService.SentMailAsync(mailRequest);

                if (payloadNotification.IsSuccess)
                {
                    payloadNotification.MessageType = MessageTypes.Success.ToString();
                    payloadNotification.IsSuccess = true;
                    payloadNotification.Status = Response.StatusCode;
                }
                else
                {
                    payloadNotification.MessageType = MessageTypes.Warning.ToString();
                    payloadNotification.IsSuccess = false;
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
