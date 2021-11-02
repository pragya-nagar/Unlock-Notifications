using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OKRNotification.Common;
using OKRNotification.EF;
using OKRNotification.Service.Contracts;
using OKRNotification.ViewModel.Request;

namespace OKRNotification.WebCore.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ApiControllerBase
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [Route("GetTemplateAsync")]
        [HttpGet]
        public async Task<IActionResult> GetTemplateAsync(string templateCode)
        {

            var payloadNotification = new PayloadCustom<MailerTemplate>
            {
                Entity = await _emailService.GetMailerTemplateAsync(templateCode)
            };

            if (payloadNotification.Entity != null)
            {
                payloadNotification.MessageType = MessageTypes.Success.ToString();
                payloadNotification.IsSuccess = true;
                payloadNotification.Status = (int)HttpStatusCode.OK;
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
        public async Task<IActionResult> SentMailAsync(MailRequest mailRequest)
        {

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
                        payloadNotification.Status = (int)HttpStatusCode.OK;
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








