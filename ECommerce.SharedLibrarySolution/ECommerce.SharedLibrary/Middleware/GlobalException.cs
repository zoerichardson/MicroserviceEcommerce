
using ECommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace ECommerce.SharedLibrary.Middleware
{

    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            //declare default variables
            string message = "sorry, internal server error occured. try again";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                //check if response has too many requests - 429 status
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "warning";
                    message = "too many requests made";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(context, title, message, statusCode);

                }

                //if response is unauthorised - 401 status code
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {

                    title = "alert";
                    message = "you are not authorised to access";
                    await ModifyHeader(context, title, message, statusCode);
                }

                //if response is forbidden - status code 403 
                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "out of access";
                    message = "you are not allowed to access";
                    statusCode = (int)StatusCodes.Status403Forbidden;
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch(Exception ex)
            {
                //log original exceptions / file,debugger, console
                LogException.LogExceptions(ex);

                //check if exception is Timeout  // 408 request timeout 
                if(ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "out of time";
                    message = "Request timeout.. try again";
                    statusCode = StatusCodes.Status408RequestTimeout;
                }

                //if none of the exceptions then do the default
                await ModifyHeader(context, title, message, statusCode);
            }


            }
        private async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            //display scary free message to client 
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
                {
                    Detail = message,
                    Status = statusCode,
                    Title = title,

                }), CancellationToken.None);
            return;

        }
    }
}
