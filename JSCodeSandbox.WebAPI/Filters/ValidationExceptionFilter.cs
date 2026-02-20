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
                var errorType = validationException is InvalidCodeToRunException
                    ? "CodeSyntaxError"
                    : "InvalidRequest";

                context.Result = new BadRequestObjectResult(new
                {
                    errorType = errorType,
                    error = validationException.Message
                });
                context.ExceptionHandled = true;
            }
        }
    }
}
