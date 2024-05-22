namespace Intelligent_AutoWms.Model.RequestDTO.DeliveryOrder
{
    /// <summary>
    /// 创建出库单实体参数
    /// </summary>
    public class CreateDeliveryOrderDTO
    {
        public List<long> ids { get; set; } = new List<long>();

        /// <summary>
        /// 入口编码
        /// </summary>
        public string Port_Code { get; set; }
    }
}
