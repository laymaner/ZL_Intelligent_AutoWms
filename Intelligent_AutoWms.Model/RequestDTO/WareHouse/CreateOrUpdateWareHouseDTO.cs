namespace Intelligent_AutoWms.Model.RequestDTO.WareHouse
{
    /// <summary>
    /// 创建或修改仓库实体参数
    /// </summary>
    public class CreateOrUpdateWareHouseDTO
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 仓库类型
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
}
