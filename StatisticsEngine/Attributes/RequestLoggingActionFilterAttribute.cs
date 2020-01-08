using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace AkkaMjrTwo.StatisticsEngine.Attributes
{
    internal class RequestLoggingActionFilterAttribute : ActionFilterAttribute
    {
    
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var logger = LogManager.GetLogger(context.Controller.GetType().FullName);

            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine("################################################################################################################################");

            sb.AppendLine(string.Format("Identity:     {0}", context.HttpContext.User.Identity.Name));
            sb.AppendLine(string.Format("RequestUri:   {0}", context.HttpContext.Request.Path.ToString()));
            sb.AppendLine(string.Format("UserAgent:    {0}", context.HttpContext.Request.Headers["User-Agent"]));
            sb.AppendLine(string.Format("Host:         {0}", context.HttpContext.Request.Headers["Host"]));
            sb.AppendLine(string.Format("ActionMethod: {0}.{1}", context.Controller.GetType().Name, context.ActionDescriptor.DisplayName));

            if (context.ActionArguments != null && context.ActionArguments.Any())
            {
                sb.AppendLine("Arguments:");
                foreach (var argument in context.ActionArguments)
                {
                    sb.AppendLine(string.Format("Key  : {0}", argument.Key));
                    sb.AppendLine(string.Format("Value: {0}{1}{0}", Environment.NewLine, argument.Value));
                }
            }

            sb.AppendLine("################################################################################################################################");

            logger.Warn(sb.ToString());
        }
    }
}
