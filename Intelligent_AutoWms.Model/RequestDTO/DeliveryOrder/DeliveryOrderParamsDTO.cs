using Intelligent_AutoWms.Model.BaseModel;

namespace Intelligent_AutoWms.Model.RequestDTO.DeliveryOrder
{
    /// <summary>
    /// 查询出库单参数实体信息
    /// </summary>
    public class DeliveryOrderParamsDTO:BasicQuery
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string? Order_No { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public string? Order_Type { get; set; }

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
        /// 出口编码
        /// </summary>
        public string? Port_Code { get; set; }

        /// <summary>
        /// 出库时间开始
        /// </summary>
        public DateTime? Delivery_Time_Strat { get; set; }

        /// <summary>
        /// 出库时间结束
        /// </summary>
        public DateTime? Delivery_Time_End { get; set; }

        /// <summary>
        /// 创建时间开始
        /// </summary>
        public DateTime? Create_Time_Strat { get; set; }

        /// <summary>
        /// 创建时间结束
        /// </summary>
        public DateTime? Create_Time_End { get; set; }

        /// <summary>
        /// 出库步骤：1--待出库 2--已出库
        /// </summary>
        public int? Delivery_Step { get; set; }
    }
}
