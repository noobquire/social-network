using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SocialNetworkApi.Models;

namespace SocialNetworkApi.Middleware
{
    public class UnhandledExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public UnhandledExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = 500;
                var error = new ApiError(GetMessage(ex), HttpStatusCode.InternalServerError);
                var result = JsonConvert.SerializeObject(error);
                await response.WriteAsync(result);
            }
        }

        private static string GetMessage(Exception ex, string message = "")
        {
            message += $"{ex.GetType()}: {ex.Message}";
            return ex.InnerException == null ? message : GetMessage(ex.InnerException, message + Environment.NewLine);
        }
    }
}