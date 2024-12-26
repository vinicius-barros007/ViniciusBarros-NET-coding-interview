using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SecureFlight.Api.Utils
{
    public class ErrorFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = new ObjectResult(context.Result)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }
}
