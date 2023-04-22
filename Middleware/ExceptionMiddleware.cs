using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StudentsDashboard.Errors;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TinyUrl.Models.Exceptions;

namespace TinyUrl.Middleware
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            this._logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            
            try
            {
              
                await next(context);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                await handleExceptionAsync(context, ex);


                /*    _logger.LogError(ex.Message.ToString());
                    Console.WriteLine(ex.Message + " from middleware"); 
                    if (ex.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        if (!context.Response.HasStarted)
                        {
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(ex.Message));
                        }
                    }*/


            }
        }

        private async Task handleExceptionAsync(HttpContext context, Exception ex)
        {
            string errorId = Guid.NewGuid().ToString();
            ErrorResults errorResults = new ErrorResults()
            {
                ErrorId = errorId,
                Source = ex.TargetSite?.DeclaringType?.FullName,
                Exception = ex.Message.Trim(),
                SupportMessage = $"Provide the Error Id: {errorId} to the support team for further analysis."
            };

            errorResults.Messages.Add(ex.Message);

            if (ex is not CustomeException && ex.InnerException != null)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    errorResults.Messages.Add(ex.Message);
                }
            }

            switch (ex)
            {
                case CustomeException exception:
                    errorResults.StatusCode = (int)exception.StatusCode;
                    if (exception.ErrorMessages is not null) errorResults.Messages = exception.ErrorMessages;
                    break;

                case KeyNotFoundException:
                    errorResults.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case ValidationException validationException:
                    errorResults.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResults.Messages.Add(validationException.Message);
                    break;
                default:
                    errorResults.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            HttpResponse response = context.Response;
            if (!response.HasStarted)
            {
                response.ContentType = "application/json";
                response.StatusCode = errorResults.StatusCode;
                await response.WriteAsync(JsonConvert.SerializeObject(errorResults));
            }
            else
            {
                _logger.LogWarning("cant write error response, response has already started");
            }
        }
    }
}
