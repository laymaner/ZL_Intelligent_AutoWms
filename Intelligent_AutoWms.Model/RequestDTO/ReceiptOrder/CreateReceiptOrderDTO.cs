namespace Intelligent_AutoWms.Model.RequestDTO.ReceiptOrder
{
    /// <summary>
    /// 创建入库单实体参数
    /// </summary>
    public class CreateReceiptOrderDTO
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
        /// 入口编码
        /// </summary>
        public string Port_Code { get; set; }
    }
}
