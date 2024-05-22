using Intelligent_AutoWms.Extensions.MiddleWares;
using Microsoft.AspNetCore.Builder;

namespace Intelligent_AutoWms.Extensions.Extensions
{
    public static class RequestMiddleWareExtension
    {
        public static IApplicationBuilder UserRequestMiddleWare(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestMiddleWare>();  
        }
    }
}
