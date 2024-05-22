using Intelligent_AutoWms.Model.BaseModel;

namespace Intelligent_AutoWms.Model.RequestDTO.Port
{
    /// <summary>
    /// 查询出入库实体参数
    /// </summary>
    public class PortParamsDTO:BasicQuery
    {
        /// <summary>
        /// 出入口编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 出入口名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        ///  类型 1：入口 2：出口
        /// </summary>
        public int? Type { get; set; }

    }
}
