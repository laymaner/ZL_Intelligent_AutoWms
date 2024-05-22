using Intelligent_AutoWms.Model.BaseModel;

namespace Intelligent_AutoWms.Model.RequestDTO.Area
{
    /// <summary>
    /// 查询库区参数实体
    /// </summary>
    public class AreaParamsDTO:BasicQuery
    {
        /// <summary>
        /// 库区编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 库区名称
        /// </summary>
        public string? Name { get; set; }

    }
}
