using Intelligent_AutoWms.Model.BaseModel;

namespace Intelligent_AutoWms.Model.RequestDTO.ReceiptOrder
{
    /// <summary>
    /// 查询入库单实体参数
    /// </summary>
    public class ReceiptOrderParamsDTO : BasicQuery
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
        /// 入口编码
        /// </summary>
        public string? Port_Code { get; set; }

        /// <summary>
        /// 入库时间开始
        /// </summary>
        public DateTime? Receipt_Time_Strat { get; set; }

        /// <summary>
        /// 入库时间结束
        /// </summary>
        public DateTime? Receipt_Time_End { get; set; }

        /// <summary>
        /// 创建时间开始
        /// </summary>
        public DateTime? Create_Time_Strat { get; set; }

        /// <summary>
        /// 创建时间结束
        /// </summary>
        public DateTime? Create_Time_End { get; set; }

        /// <summary>
        /// 入库步骤：1--待入库 2--已入库
        /// </summary>
        public int? Receipt_Step { get; set; }
    }
}
