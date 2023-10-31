﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using AuthApi.Exceptions;

namespace AuthApi.Filters
{
    public class ProblemDetailsExceptionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is ProblemDetailsException exception)
            {
                context.Result = new ObjectResult(exception.Value)
                {
                    StatusCode = exception.Value.Status
                };

                context.ExceptionHandled = true;
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
