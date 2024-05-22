namespace Intelligent_AutoWms.Model.RequestDTO.Location
{
    /// <summary>
    /// 创建或更新货位实体参数
    /// </summary>
    public class CreateOrUpdateLocationDTO
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 货位名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 货位编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 货架id
        /// </summary>
        public long? Shelf_Id { get; set; }

        /// <summary>
        /// 巷道
        /// </summary>
        public int? Lanway { get; set; }

        /// <summary>
        /// 排
        /// </summary>
        public int? Location_Row { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        public int? Location_Column { get; set; }

        /// <summary>
        /// 层
        /// </summary>
        public int? Location_Layer { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

    }
}
