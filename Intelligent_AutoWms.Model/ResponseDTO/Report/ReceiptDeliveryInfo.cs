namespace Intelligent_AutoWms.Model.ResponseDTO.Report
{
    /// <summary>
    /// 出入库报表数据
    /// </summary>
    public class ReceiptDeliveryInfo
    {
        /// <summary>
        /// 待入库
        /// </summary>
        public List<Int32> WaitReceipt { get; set; } = new List<Int32>();

        /// <summary>
        /// 已入库
        /// </summary>
        public List<Int32> Receipt { get; set; } = new List<Int32>();

        /// <summary>
        /// 待出库
        /// </summary>
        public List<Int32> WaitDelivery { get; set; } = new List<Int32>();

        /// <summary>
        /// 已出库
        /// </summary>
        public List<Int32> Delivery { get; set; } = new List<Int32>();
    }
}
