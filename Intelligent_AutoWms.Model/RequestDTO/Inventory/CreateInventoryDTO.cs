namespace Intelligent_AutoWms.Model.RequestDTO.Inventory
{
    /// <summary>
    /// 创建库存明细参数实体
    /// </summary>
    public class CreateInventoryDTO
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string Material_Code { get; set; }

        /// <summary>
        /// 物料类型
        /// </summary>
        public int Material_Type { get; set; }

        /// <summary>
        /// 存储位置编码
        /// </summary>
        public string Location_Code { get; set; }

        /// <summary>
        /// 入口编码
        /// </summary>
        public string Port_Code { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string Order_No { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public string Order_Type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
}
