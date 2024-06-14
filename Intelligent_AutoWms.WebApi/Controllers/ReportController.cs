using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.RequestDTO.Area;
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
        public async Task<ApiResult<List<Int32>>> GetLocationReport()
        {
            var result = await _reportService.GetLocationReport();
            return SuccessResult(result);
        }

        /// <summary>
        /// 获取库存明细数据分析
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<Int32>>> GetInventoryReport()
        {
            var result = await _reportService.GetInventoryReport();
            return SuccessResult(result);
        }

        /// <summary>
        /// 获取入库单待入库数据分析
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<Int32>>> GetWaitReceiptReport()
        {
            var result = await _reportService.GetWaitReceiptReport();
            return SuccessResult(result);
        }

        /// <summary>
        /// 获取入库单已入库数据分析
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<Int32>>> GetReceiptedReport()
        {
            var result = await _reportService.GetReceiptedReport();
            return SuccessResult(result);
        }


        /// <summary>
        ///  获取出库单待出库数据分析
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<Int32>>> GetWaitDeliveryReport()
        {
            var result = await _reportService.GetWaitDeliveryReport();
            return SuccessResult(result);
        }

        /// <summary>
        ///  获取出库单已出库数据分析
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<Int32>>> GetDeliveredReport()
        {
            var result = await _reportService.GetDeliveredReport();
            return SuccessResult(result);
        }
    }
}
