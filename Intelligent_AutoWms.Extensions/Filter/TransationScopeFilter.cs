using Intelligent_AutoWms.Extensions.Attri;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Transactions;

namespace Intelligent_AutoWms.Extensions.Filter
{
    public class TransationScopeFilter : IAsyncActionFilter
    {
        /// <summary>
        /// 事务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool hasTransactionalAttribute = false;
            if (context.ActionDescriptor is ControllerActionDescriptor)
            {
                var actionDesc = (ControllerActionDescriptor)context.ActionDescriptor;
                hasTransactionalAttribute = actionDesc.MethodInfo.IsDefined(typeof(TransationAttribute));
            }
            if (!hasTransactionalAttribute)
            {
                await next();
                return;
            }
            using var txScope =
                    new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var result = await next();
            if (result.Exception == null)
            {
                txScope.Complete();
            }
        }
    }
}
