using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace AkkaMjrTwo.GameEngine.Api.Attributes
{
    public class UnhandledExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        public override void OnException(ExceptionContext context)
        {
            var logger = LogManager.GetCurrentClassLogger();
            
            logger.Error(context.Exception);

        }
    }
}
