using Intelligent_AutoWms.Model.BaseModel;

namespace Intelligent_AutoWms.Model.RequestDTO.Shelf
{
    /// <summary>
    /// 查询货架实体参数
    /// </summary>
    public class ShelfParamsDTO:BasicQuery
    {
        /// <summary>
        /// 货架编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 货架名称
        /// </summary>
        public string? Name { get; set; }

    }
}
