using Microsoft.AspNetCore.Mvc.Filters;
using ReadRequestBodyASPNETCoreWebAPI.Controllers;
using System.Net.NetworkInformation;

namespace ReadRequestBodyASPNETCoreWebAPI
{
    //Bunu kullanılır hale getırmek için 
    //    Program.cs icerisine 
    //    var app = builder.Build();
    //     app.UseMiddleware<RequestBodyMiddleware>();
    //    eklenmelidir
    public class RequestBodyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly int MaxContentLength = 1024;
        public RequestBodyMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }
        //public async Task Invoke(HttpContext context)
        //{
        //    await _next(context);
        //}

        public async Task Invoke(HttpContext context)
        {
            var requestPath = context.Request.Path.Value;
            if (requestPath.IndexOf("read-from-middleware") > -1)
            {
                context.Request.EnableBuffering();
                var requestBody = await context.Request.Body.ReadAsStringAsync(true);
                //Avoid Reading Large Request Bodies
                if (requestBody.Length > MaxContentLength)
                {
                    context.Response.StatusCode = 413;
                    await context.Response.WriteAsync("Request Body Too Large");
                    return;
                }
                _logger.LogInformation("Request Body:{@requestBody}", requestBody);
                context.Request.Headers.Add("RequestBodyMiddleware", requestBody);
                context.Items.Add("RequestBody", requestBody);
                context.Request.Body.Position = 0;
            }
            await _next(context);
        }
    }
    //Using Action Filters to Read the Request Body
    //Using Action Filters to Read the Request Body
    //Using Action Filters to Read the Request Body
    // BUnu kullanmak için Program.cs'e
    //builder.Services.AddControllers(options =>
    //{
    //options.Filters.Add<ReadRequestBodyActionFilter>();
    //});

    public class ReadRequestBodyActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var requestPath = context.HttpContext.Request.Path.Value;
            if (requestPath.IndexOf("read-from-action-filter") > -1)
            {
                var requestBody = await context.HttpContext.Request.Body.ReadAsStringAsync();
                context.HttpContext.Request.Headers.Add("ReadRequestBodyActionFilter", requestBody);
            }
            await next();
        }
    }
    /********Using a Custom Attribute to Read the Request Body******/
    [AttributeUsage(AttributeTargets.Method)]
    public class ReadRequestBodyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var requestBody = await context.HttpContext.Request.Body.ReadAsStringAsync();
            context.HttpContext.Request.Headers.Add("ReadRequestBodyAttribute", requestBody);
            await next();
        }
    }
    /********Using a Custom Attribute to Read the Request Body******/
}