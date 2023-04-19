using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

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
                Console.WriteLine(ex.Message + " from middleware"); 
                if (ex.GetType() == typeof(SecurityTokenExpiredException))
                {
                    if (!context.Response.HasStarted)
                    {
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(ex.Message));
                    }
                }

                    
            }
        }
    }
}
