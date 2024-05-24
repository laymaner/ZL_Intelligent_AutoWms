using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.ReceiptOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 入库单
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ReceiptOrderController : ApiControllerBase
    {
        private readonly IReceiptOrderService _receiptOrderService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiptOrderService"></param>
        public ReceiptOrderController(IReceiptOrderService receiptOrderService)
        {
            _receiptOrderService = receiptOrderService;
        }

        /// <summary>
        /// 查询入库单信息
        /// </summary>
        /// <param name="receiptOrderParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Receipt_Orders>>> GetListAsync([FromQuery] ReceiptOrderParamsDTO receiptOrderParamsDTO)
        {
            var result = await _receiptOrderService.GetListAsync(receiptOrderParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 查询入库单信息分页
        /// </summary>
        /// <param name="receiptOrderParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<BasePagination<WMS_Receipt_Orders>>> GetPaginationAsync([FromQuery] ReceiptOrderParamsDTO receiptOrderParamsDTO)
        {
            var result = await _receiptOrderService.GetPaginationAsync(receiptOrderParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据id查询入库单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Receipt_Orders>> GetReceiptOrderByIdAsync(long id)
        {
            var result = await _receiptOrderService.GetReceiptOrderByIdAsync(id);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据code获取入库单数据
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Receipt_Orders>> GetReceiptOrderByCodeAsync(string code)
        {
            var result = await _receiptOrderService.GetReceiptOrderByCodeAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 判断入库单是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<bool>> IsExistAsync(long id)
        {
            var result = await _receiptOrderService.IsExistAsync(id);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids重新生成入库任务
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<string>> RegenerateTaskAsync(List<long> ids)
        {
            var result = await _receiptOrderService.RegenerateTaskAsync(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 创建入库单
        /// </summary>
        /// <param name="createReceiptOrderDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> CreateAsync([FromBody] CreateReceiptOrderDTO createReceiptOrderDTO)
        {
            var result = await _receiptOrderService.CreateAsync(createReceiptOrderDTO, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids删除入库单
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Transation]
        public async Task<ApiResult<long>> DelAsync(string ids)
        {
            var result = await _receiptOrderService.DelAsync(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="receiptOrderParamsDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> ExportAsync([FromQuery] ReceiptOrderParamsDTO receiptOrderParamsDTO)
        {
            return await _receiptOrderService.ExportAsync(receiptOrderParamsDTO);
        }
    }
}
