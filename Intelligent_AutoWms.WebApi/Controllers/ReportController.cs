using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.RequestDTO.Area;
using Intelligent_AutoWms.Model.ResponseDTO.Report;
using Intelligent_AutoWms.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 报表
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ReportController : ApiControllerBase
    {
        private readonly IReportService _reportService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportService"></param>
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// 获取货位使用情况
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<Int32>>> GetLocationReportAsync()
        {
            var result = await _reportService.GetLocationReportAsync();
            return SuccessResult(result);
        }

        /// <summary>
        /// 获取库存明细数据分析
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<Int32>>> GetInventoryReportAsync()
        {
            var result = await _reportService.GetInventoryReportAsync();
            return SuccessResult(result);
        }

        /// <summary>
        /// 获取入库单待入库数据分析
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<Int32>>> GetWaitReceiptReportAsync()
        {
            var result = await _reportService.GetWaitReceiptReportAsync();
            return SuccessResult(result);
        }

        /// <summary>
        /// 获取入库单已入库数据分析
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<Int32>>> GetReceiptedReportAsync()
        {
            var result = await _reportService.GetReceiptedReportAsync();
            return SuccessResult(result);
        }


        /// <summary>
        ///  获取出库单待出库数据分析
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<Int32>>> GetWaitDeliveryReportAsync()
        {
            var result = await _reportService.GetWaitDeliveryReportAsync();
            return SuccessResult(result);
        }

        /// <summary>
        ///  获取出库单已出库数据分析
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<Int32>>> GetDeliveredReportAsync()
        {
            var result = await _reportService.GetDeliveredReportAsync();
            return SuccessResult(result);
        }

        /// <summary>
        ///  获取出库单已出库数据分析-----综合
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<ReceiptDeliveryInfo>> GetReceiptDeliveryInfoAsync()
        {
            var result = await _reportService.GetReceiptDeliveryInfoAsync();
            return SuccessResult(result);
        }
    }
}
