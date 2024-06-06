using Intelligent_AutoWms.Model.BaseModel;

namespace Intelligent_AutoWms.Model.RequestDTO.Warehouse
{
    /// <summary>
    /// 查询仓库参数实体
    /// </summary>
    public class WareHouseParamsDTO : BasicQuery
    {
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 仓库类型
        /// </summary>
        public string? WarehouseType { get; set; }
    }
}
