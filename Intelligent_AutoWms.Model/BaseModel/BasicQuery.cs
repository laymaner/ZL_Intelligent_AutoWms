namespace Intelligent_AutoWms.Model.BaseModel
{
    /// <summary>
    /// 基本查询参数
    /// </summary>
    public class BasicQuery
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 当前页行数
        /// </summary>
        public int PageSize { get; set; } = 10;

    }
}
