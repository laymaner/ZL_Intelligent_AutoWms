namespace Intelligent_AutoWms.Model.RequestDTO.Shelf
{
    /// <summary>
    /// 创建或修改货架实体参数
    /// </summary>
    public class CreateOrUpdateShelfDTO
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 货架名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 货架编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 库区id
        /// </summary>
        public long? Area_Id { get; set; }

        /// <summary>
        /// 巷道数量
        /// </summary>
        public int? Lanway { get; set; }

        /// <summary>
        /// 排数
        /// </summary>
        public int? Shelf_Rows { get; set; }

        /// <summary>
        /// 列数
        /// </summary>
        public int? Shelf_Columns { get; set; }

        /// <summary>
        /// 层数
        /// </summary>
        public int? Shelf_Layers { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
}
