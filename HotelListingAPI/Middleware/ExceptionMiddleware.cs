using HotelListingAPI.Exception;
using Newtonsoft.Json;
using System;
using System.Net;

namespace HotelListingAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate _next)
        {
            this._next= _next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(System.Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        
        }
        private Task HandleExceptionAsync(HttpContext context,System.Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            ErrorDetails details = new ErrorDetails()
            {
                ErrorMessage = ex.ToString(),
                ErrorType = "Failure"
            };
            switch (ex)
            {
                case NotFoundException notFoundException:
                    {
                        details.ErrorType = "NotFound";
                        context.Response.StatusCode = 404;
                        break;
                    }
                default:
                    break;
            }
            string response = JsonConvert.SerializeObject(details);
            return context.Response.WriteAsync(response);
        }
    }
    public class ErrorDetails
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
    }
}
