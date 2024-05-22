using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace Intelligent_AutoWms.Extensions.Filter
{
    public class RateLimitFilter : IAsyncActionFilter
    {
        private readonly IMemoryCache memCache;
        public RateLimitFilter(IMemoryCache memCache)
        {
            this.memCache = memCache;
        }
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //// 获取IP
            string removeIP = context.HttpContext.Connection.RemoteIpAddress.ToString();

            //设置cache的Key
            string cacheKey = $"LastVisitTick_{removeIP}";

            //在缓存中获取key记录 允许为空
            long? lastTick = memCache.Get<long?>(cacheKey);

            // 如果没有lasttick 或者当前时间-lasttick的值大于1秒钟
            if (lastTick == null || Environment.TickCount64 - lastTick > 1000)
            {
                // 重新设置缓存过期时间，
                // Environment.TickCount64是一个64位整数，表示从系统启动开始经过的微秒数。它通常用于计时和性能测量。
                //TimeSpan.FromSeconds(10)表示一个时间跨度，从当前时间开始，经过10秒钟。
                //设置内存缓存中数据的有效期为10秒。
                memCache.Set(cacheKey, Environment.TickCount64, TimeSpan.FromSeconds(10));//避免长期不访问的用户，占用缓存的内存

                // 放行 
                return next();
            }
            else
            {
                // 否则就拦截
                context.Result = new ContentResult { StatusCode = 429 };

                //表示返回一个已经完成的任务。
                //这个方法通常用于异步编程中，当一个任务已经完成时，可以使用这个方法来避免抛出异常。
                return Task.CompletedTask;
            }
        }
    }
}
