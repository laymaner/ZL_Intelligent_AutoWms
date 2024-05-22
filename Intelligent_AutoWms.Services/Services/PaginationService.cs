using Intelligent_AutoWms.Model.BaseModel;
using Microsoft.EntityFrameworkCore;

namespace Intelligent_AutoWms.Services.Services
{
    public static class PaginationService
    {

        /// <summary>
        /// 获取分页后的数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="source">数据源IQueryable</param>
        /// <param name="pagination">分页参数</param>
        /// <returns></returns>
        public static async Task<BasePagination<T>> PaginateAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize) where T : class
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new BasePagination<T>(items, count, pageIndex, pageSize);
        }
    }
}
