using Intelligent_AutoWms.Model.ResponseDTO.Report;

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
        public Task<List<Int32>> GetLocationReportAsync();

        /// <summary>
        /// 获取库存明细数据分析
        /// </summary>
        /// <returns></returns>
        public Task<List<Int32>> GetInventoryReportAsync();

        /// <summary>
        /// 获取入库单待入库数据分析
        /// </summary>
        /// <returns></returns>
        public Task<List<Int32>> GetWaitReceiptReportAsync();

        /// <summary>
        /// 获取入库单已入库数据分析
        /// </summary>
        /// <returns></returns>
        public Task<List<Int32>> GetReceiptedReportAsync();

        /// <summary>
        ///  获取出库单待出库数据分析
        /// </summary>
        /// <returns></returns>
        public Task<List<Int32>> GetWaitDeliveryReportAsync();

        /// <summary>
        ///  获取出库单已出库数据分析
        /// </summary>
        /// <returns></returns>
        public Task<List<Int32>> GetDeliveredReportAsync();

        /// <summary>
        ///  获取出库单已出库数据分析-----综合
        /// </summary>
        /// <returns></returns>
        public  Task<ReceiptDeliveryInfo> GetReceiptDeliveryInfoAsync();
    }
}
