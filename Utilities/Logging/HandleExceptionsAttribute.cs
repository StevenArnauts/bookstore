using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Utilities.Logging
{
    public class HandleExceptionsAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception == null)
            {
                return;
            }
			Logger.Error(this, context.Exception);
            Error error = ConvertToError(context.Exception);
            context.HttpContext.Response.StatusCode = error.Code;
            context.Result = new ObjectResult(error);
        }

        private static Error ConvertToError(Exception exception)
        {
            switch (exception)
            {
                case InvalidOperationException _:
                    return CreateError(HttpStatusCode.BadRequest, exception.Message);
                case NotSupportedException _:
                    return CreateError(HttpStatusCode.NotImplemented, exception.Message);
            }
            return CreateError(HttpStatusCode.InternalServerError, exception.Message);
        }

        /// <summary>
        ///     Creates a standard Bringme Error response object.
        /// </summary>
        /// <param name="code">The HTTP status code.</param>
        /// <param name="message">The message.</param>
        /// <returns>Error object.</returns>
        private static Error CreateError(HttpStatusCode code, string message)
        {
            return new Error((int) code, message);
        }
    }
   
}