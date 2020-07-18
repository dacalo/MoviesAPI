using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace MoviesAPI.Helpers
{
    public class FilterErrors : ExceptionFilterAttribute
    {
        private readonly ILogger<FilterErrors> _logger;

        public FilterErrors(ILogger<FilterErrors> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
