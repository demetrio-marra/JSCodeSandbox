using JSCodeSandbox.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JSCodeSandbox.WebAPI.Filters
{
    public class ValidationExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ValidationException validationException)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    error = validationException.Message
                });
                context.ExceptionHandled = true;
            }
        }
    }
}
