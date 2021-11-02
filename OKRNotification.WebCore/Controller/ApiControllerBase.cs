using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OKRNotification.EF;
using Serilog;
using System.Net;

namespace OKRNotification.WebCore.Controller
{
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        protected ILogger Logger { get; set; }
        public ApiControllerBase()
        {
            Logger = Log.Logger;
        }

        protected string LoggedInUserEmail => HttpContext.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(x => x.Type == "email")?.Value;

        protected string UserToken => HttpContext.User.Identities.FirstOrDefault()?.Claims.FirstOrDefault(x => x.Type == "token")?.Value;

        protected bool IsActiveToken => (!string.IsNullOrEmpty(LoggedInUserEmail) && !string.IsNullOrEmpty(UserToken));


        public PayloadCustom<T> GetPayloadStatus<T>(PayloadCustom<T> payload)
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {

                    payload.MessageList.Add(state.Key, error.ErrorMessage);
                }
            }
            payload.IsSuccess = false;
            payload.Status = (int)HttpStatusCode.BadRequest;
            return payload;
        }
    }
}
