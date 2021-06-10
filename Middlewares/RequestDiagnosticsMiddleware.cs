using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemindONServer.Middlewares
{
    public class RequestDiagnosticsMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestDiagnosticsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IActionDescriptorCollectionProvider provider = null)
        {
            if (provider != null)
            {
                var routes = provider.ActionDescriptors.Items;
            }

            await _next(context);
        }
    }
}
