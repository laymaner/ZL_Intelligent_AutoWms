using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.ResponseDTO.Report;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intelligent_AutoWms.Services.Services
{
    public class ReportService:IReportService
    {
        private readonly Intelligent_AutoWms_DbContext _db;
        private readonly ILogger<ReportService> _log;

        public ReportService(Intelligent_AutoWms_DbContext db, ILogger<ReportService> logger)
        {
            _db = db;
            _log = logger;
        }

        /// <summary>
        /// 获取货位使用情况
        /// </summary>
        /// <returns></returns>
        public async Task<List<Int32>> GetLocationReportAsync()
        {
            try 
            {
                List<Int32> list = new List<Int32>();
                var idleCount = await _db.Locations.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Step == (int)LocationStatusEnum.Idle).CountAsync();
                var warehousingLockCount = await _db.Locations.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Step == (int)LocationStatusEnum.Warehousing_Lock).CountAsync();
                var outboundLockCount = await _db.Locations.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Step == (int)LocationStatusEnum.Outbound_Lock).CountAsync();
                var OccupyCount = await _db.Locations.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Step == (int)LocationStatusEnum.Occupying).CountAsync();
                list.Add(idleCount);
                list.Add(warehousingLockCount);
                list.Add(outboundLockCount);
                list.Add(OccupyCount);
                return list;
            }
            catch (Exception ex) 
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取库存明细数据分析
        /// </summary>
        /// <returns></returns>
        public async Task<List<Int32>> GetInventoryReportAsync()
        {
            try
            {
                List<Int32> list = new List<Int32>();
                DateTime todayStart = DateTime.Today.AddDays(1);
                var first_seven = await _db.Inventories.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Create_Time < todayStart.AddDays(-6)).CountAsync();
                var first_six = await _db.Inventories.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Create_Time < todayStart.AddDays(-5)).CountAsync();
                var first_five = await _db.Inventories.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Create_Time < todayStart.AddDays(-4)).CountAsync();
                var first_four = await _db.Inventories.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Create_Time < todayStart.AddDays(-3)).CountAsync();
                var first_three = await _db.Inventories.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Create_Time < todayStart.AddDays(-2)).CountAsync();
                var first_two = await _db.Inventories.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Create_Time < todayStart.AddDays(-1)).CountAsync();
                var first_now = await _db.Inventories.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Create_Time < todayStart).CountAsync();
                list.Add(first_seven);
                list.Add(first_six);
                list.Add(first_five);
                list.Add(first_four);
                list.Add(first_three);
                list.Add(first_two);
                list.Add(first_now);
                return list;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取入库单待入库数据分析
        /// </summary>
        /// <returns></returns>
        public async Task<List<Int32>> GetWaitReceiptReportAsync()
        {
            try
            {
                List<Int32> list = new List<Int32>();
                DateTime todayStart = DateTime.Today; // 当日开始时间，默认为00:00:00
                DateTime todayEnd = todayStart.AddDays(1); // 当日结束时间
                var first_seven = await _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.WaitingForStorage && m.Create_Time >= todayStart.AddDays(-6) && m.Create_Time < todayEnd.AddDays(-6)).CountAsync();
                var first_six = await _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.WaitingForStorage && m.Create_Time >= todayStart.AddDays(-5) && m.Create_Time < todayEnd.AddDays(-5)).CountAsync();
                var first_five = await _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.WaitingForStorage && m.Create_Time >= todayStart.AddDays(-4) && m.Create_Time < todayEnd.AddDays(-4)).CountAsync();
                var first_four = await _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.WaitingForStorage && m.Create_Time >= todayStart.AddDays(-3) && m.Create_Time < todayEnd.AddDays(-3)).CountAsync();
                var first_three = await _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.WaitingForStorage && m.Create_Time >= todayStart.AddDays(-2) && m.Create_Time < todayEnd.AddDays(-2)).CountAsync();
                var first_two = await _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.WaitingForStorage && m.Create_Time >= todayStart.AddDays(-1) && m.Create_Time < todayEnd.AddDays(-1)).CountAsync();
                var first_now = await _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.WaitingForStorage && m.Create_Time >= todayStart && m.Create_Time < todayEnd).CountAsync();
                list.Add(first_seven);
                list.Add(first_six);
                list.Add(first_five);
                list.Add(first_four);
                list.Add(first_three);
                list.Add(first_two);
                list.Add(first_now);
                return list;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取入库单已入库数据分析
        /// </summary>
        /// <returns></returns>
        public async Task<List<Int32>> GetReceiptedReportAsync()
        {
            try
            {
                List<Int32> list = new List<Int32>();
                DateTime todayStart = DateTime.Today; // 当日开始时间，默认为00:00:00
                DateTime todayEnd = todayStart.AddDays(1); // 当日结束时间
                var first_seven = await _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.Received && m.Receipt_Time >= todayStart.AddDays(-6) && m.Receipt_Time < todayEnd.AddDays(-6)).CountAsync();
                var first_six = await _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.Received && m.Receipt_Time >= todayStart.AddDays(-5) && m.Receipt_Time < todayEnd.AddDays(-5)).CountAsync();
                var first_five = await _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.Received && m.Receipt_Time >= todayStart.AddDays(-4) && m.Receipt_Time < todayEnd.AddDays(-4)).CountAsync();
                var first_four = await _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.Received && m.Receipt_Time >= todayStart.AddDays(-3) && m.Receipt_Time < todayEnd.AddDays(-3)).CountAsync();
                var first_three = await _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.Received && m.Receipt_Time >= todayStart.AddDays(-2) && m.Receipt_Time < todayEnd.AddDays(-2)).CountAsync();
                var first_two = await _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.Received && m.Receipt_Time >= todayStart.AddDays(-1) && m.Receipt_Time < todayEnd.AddDays(-1)).CountAsync();
                var first_now = await _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.Received && m.Receipt_Time >= todayStart && m.Receipt_Time < todayEnd).CountAsync();
                list.Add(first_seven);
                list.Add(first_six);
                list.Add(first_five);
                list.Add(first_four);
                list.Add(first_three);
                list.Add(first_two);
                list.Add(first_now);
                return list;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        ///  获取出库单待出库数据分析
        /// </summary>
        /// <returns></returns>
        public async Task<List<Int32>> GetWaitDeliveryReportAsync()
        {
            try
            {
                List<Int32> list = new List<Int32>();
                DateTime todayStart = DateTime.Today; // 当日开始时间，默认为00:00:00
                DateTime todayEnd = todayStart.AddDays(1); // 当日结束时间
                var first_seven = await _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.WaitingForOutbound && m.Create_Time >= todayStart.AddDays(-6) && m.Create_Time < todayEnd.AddDays(-6)).CountAsync();
                var first_six = await _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.WaitingForOutbound && m.Create_Time >= todayStart.AddDays(-5) && m.Create_Time < todayEnd.AddDays(-5)).CountAsync();
                var first_five = await _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.WaitingForOutbound && m.Create_Time >= todayStart.AddDays(-4) && m.Create_Time < todayEnd.AddDays(-4)).CountAsync();
                var first_four = await _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.WaitingForOutbound && m.Create_Time >= todayStart.AddDays(-3) && m.Create_Time < todayEnd.AddDays(-3)).CountAsync();
                var first_three = await _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.WaitingForOutbound && m.Create_Time >= todayStart.AddDays(-2) && m.Create_Time < todayEnd.AddDays(-2)).CountAsync();
                var first_two = await _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.WaitingForOutbound && m.Create_Time >= todayStart.AddDays(-1) && m.Create_Time < todayEnd.AddDays(-1)).CountAsync();
                var first_now = await _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.WaitingForOutbound && m.Create_Time >= todayStart && m.Create_Time < todayEnd).CountAsync();
                list.Add(first_seven);
                list.Add(first_six);
                list.Add(first_five);
                list.Add(first_four);
                list.Add(first_three);
                list.Add(first_two);
                list.Add(first_now);
                return list;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        ///  获取出库单已出库数据分析
        /// </summary>
        /// <returns></returns>
        public async Task<List<Int32>> GetDeliveredReportAsync()
        {
            try
            {
                List<Int32> list = new List<Int32>();
                DateTime todayStart = DateTime.Today; // 当日开始时间，默认为00:00:00
                DateTime todayEnd = todayStart.AddDays(1); // 当日结束时间
                var first_seven = await _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.Outbound && m.Delivery_Time >= todayStart.AddDays(-6) && m.Delivery_Time < todayEnd.AddDays(-6)).CountAsync();
                var first_six = await _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.Outbound && m.Delivery_Time >= todayStart.AddDays(-5) && m.Delivery_Time < todayEnd.AddDays(-5)).CountAsync();
                var first_five = await _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.Outbound && m.Delivery_Time >= todayStart.AddDays(-4) && m.Delivery_Time < todayEnd.AddDays(-4)).CountAsync();
                var first_four = await _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.Outbound && m.Delivery_Time >= todayStart.AddDays(-3) && m.Delivery_Time < todayEnd.AddDays(-3)).CountAsync();
                var first_three = await _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.Outbound && m.Delivery_Time >= todayStart.AddDays(-2) && m.Delivery_Time < todayEnd.AddDays(-2)).CountAsync();
                var first_two = await _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.Outbound && m.Delivery_Time >= todayStart.AddDays(-1) && m.Delivery_Time < todayEnd.AddDays(-1)).CountAsync();
                var first_now = await _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.Outbound && m.Delivery_Time >= todayStart && m.Delivery_Time < todayEnd).CountAsync();
                list.Add(first_seven);
                list.Add(first_six);
                list.Add(first_five);
                list.Add(first_four);
                list.Add(first_three);
                list.Add(first_two);
                list.Add(first_now);
                return list;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        ///  获取出库单已出库数据分析-----综合
        /// </summary>
        /// <returns></returns>
        public async Task<ReceiptDeliveryInfo> GetReceiptDeliveryInfoAsync()
        {
            try
            {
                ReceiptDeliveryInfo receiptDeliveryInfo = new ReceiptDeliveryInfo();
                receiptDeliveryInfo.WaitReceipt = await GetWaitReceiptReportAsync();
                receiptDeliveryInfo.Receipt = await GetReceiptedReportAsync();
                receiptDeliveryInfo.WaitDelivery = await GetWaitDeliveryReportAsync();
                receiptDeliveryInfo.Delivery = await GetDeliveredReportAsync();
                return receiptDeliveryInfo;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
