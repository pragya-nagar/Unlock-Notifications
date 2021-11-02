using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OKRNotification.Service.Contracts;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace OKRNotification.WebCore.Filter
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExceptionFiltersAttribute : ExceptionFilterAttribute
    {
        private readonly IEmailService emailService;
        private readonly IEmailServiceV2 emailServiceV2;
        private readonly IOkrNotificationsService  okrNotificationsService;

        public ExceptionFiltersAttribute(IEmailService emailServices , IEmailServiceV2 emailServicesV2, IOkrNotificationsService okrNotificationsServices) : base()
        {
            emailService = emailServices;
            emailServiceV2 = emailServicesV2;
            okrNotificationsService = okrNotificationsServices;

        }

        public override void OnException(ExceptionContext context)
        {
            var controller = string.Empty;
            var action = string.Empty;

            var statusCode = HttpStatusCode.InternalServerError;

            if (context.Exception is DataNotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
            }

            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)statusCode;

            if (context.RouteData != null)
            {
                action = context.RouteData.Values["action"].ToString();
                controller = context.RouteData.Values["controller"].ToString();
            }

            context.Result = new JsonResult(new
            {
                error = new[] { context.Exception.Message },
                stackTrace = context.Exception.StackTrace
            });

            emailService.SaveLog(controller, action, context.Exception.ToString() + "InnerException" + context.Exception.InnerException);
            emailServiceV2.SaveLog(controller, action, context.Exception.ToString() + "InnerException" + context.Exception.InnerException);
            okrNotificationsService.SaveLog(controller, action, context.Exception.ToString() + "InnerException" + context.Exception.InnerException);
        }

    }
 }
