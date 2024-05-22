namespace Intelligent_AutoWms.Model.BaseModel
{
    /// <summary>
    /// 分页基类
    /// </summary>
    public class BasePagination<T>
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 当前页行数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 数据集合
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="count"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        public BasePagination(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Items = items;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
    }
}
