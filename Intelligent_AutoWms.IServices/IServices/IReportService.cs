namespace Intelligent_AutoWms.IServices.IServices
{
    /// <summary>
    /// 报表接口
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// 获取货位使用情况
        /// </summary>
        /// <returns></returns>
        public Task<List<Int32>> GetLocationReport();

        /// <summary>
        /// 获取库存明细数据分析
        /// </summary>
        /// <returns></returns>
        public Task<List<Int32>> GetInventoryReport();

        /// <summary>
        /// 获取入库单待入库数据分析
        /// </summary>
        /// <returns></returns>
        public Task<List<Int32>> GetWaitReceiptReport();

        /// <summary>
        /// 获取入库单已入库数据分析
        /// </summary>
        /// <returns></returns>
        public Task<List<Int32>> GetReceiptedReport();

        /// <summary>
        ///  获取出库单待出库数据分析
        /// </summary>
        /// <returns></returns>
        public Task<List<Int32>> GetWaitDeliveryReport();

        /// <summary>
        ///  获取出库单已出库数据分析
        /// </summary>
        /// <returns></returns>
        public Task<List<Int32>> GetDeliveredReport();
    }
}
