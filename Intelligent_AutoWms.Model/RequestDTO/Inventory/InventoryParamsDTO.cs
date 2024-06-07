using Intelligent_AutoWms.Model.BaseModel;

namespace Intelligent_AutoWms.Model.RequestDTO.Inventory
{
    /// <summary>
    /// 查询库存明细参数实体
    /// </summary>
    public class InventoryParamsDTO:BasicQuery
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string? Material_Code { get; set; }

        /// <summary>
        /// 物料类型
        /// </summary>
        public int? Material_Type { get; set; }

        /// <summary>
        /// 存储位置编码
        /// </summary>
        public string? Location_Code { get; set; }

        /// <summary>
        /// 入口编码
        /// </summary>
        public string? Port_Code { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string? Order_No { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public string? Order_Type { get; set; }

        /// <summary>
        /// Y/N 是否锁定
        /// </summary>
        public string? Is_Lock { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? Start_Time { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? End_Time { get; set; }
    }
}
