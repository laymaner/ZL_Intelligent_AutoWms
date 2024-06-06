namespace Intelligent_AutoWms.Model.RequestDTO.Area
{
    /// <summary>
    /// 创建或修改库区实体参数
    /// </summary>
    public class CreateOrUpdateAreaDTO
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 库区名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 库区编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 仓库id
        /// </summary>
        public long? Warehouse_Id { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
}
