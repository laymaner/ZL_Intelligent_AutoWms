using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.DeliveryOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 出库单
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class DeliveryOrderController : ApiControllerBase
    {
        private readonly IDeliveryOrderService _deliveryOrderService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deliveryOrderService"></param>
        public DeliveryOrderController(IDeliveryOrderService deliveryOrderService)
        {
            _deliveryOrderService = deliveryOrderService;
        }

        /// <summary>
        /// 查询出库单信息
        /// </summary>
        /// <param name="deliveryOrderParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Delivery_Orders>>> GetListAsync([FromQuery] DeliveryOrderParamsDTO deliveryOrderParamsDTO)
        {
            var result = await _deliveryOrderService.GetListAsync(deliveryOrderParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 查询出库单信息分页
        /// </summary>
        /// <param name="deliveryOrderParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<BasePagination<WMS_Delivery_Orders>>> GetPaginationAsync([FromQuery] DeliveryOrderParamsDTO deliveryOrderParamsDTO)
        {
            var result = await _deliveryOrderService.GetPaginationAsync(deliveryOrderParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据id查询出库单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Delivery_Orders>> GetDeliveryOrderByIdAsync(long id)
        {
            var result = await _deliveryOrderService.GetDeliveryOrderByIdAsync(id);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据code获取出库单数据
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Delivery_Orders>> GetDeliveryOrderByCodeAsync(string code)
        {
            var result = await _deliveryOrderService.GetDeliveryOrderByCodeAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 判断出库单是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<bool>> IsExistAsync(long id)
        {
            var result = await _deliveryOrderService.IsExistAsync(id);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids重新生成出库任务
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<string>> RegenerateTaskByIdsAsync(List<long> ids)
        {
            var result = await _deliveryOrderService.RegenerateTaskByIdsAsync(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 一键重新生成出库任务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<string>> RegenerateTaskAsync()
        {
            var result = await _deliveryOrderService.RegenerateTaskAsync(long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 创建出库单---指定出库口
        /// </summary>
        /// <param name="createDeliveryOrderDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> CreateAsync([FromBody] CreateDeliveryOrderDTO createDeliveryOrderDTO)
        {
            var result = await _deliveryOrderService.CreateAsync(createDeliveryOrderDTO, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }


        /// <summary>
        /// 创建出库单---非指定出库口
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> QuickyCreateAsync(List<long> ids)
        {
            var result = await _deliveryOrderService.QuickyCreateAsync(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids删除出库单
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Transation]
        public async Task<ApiResult<long>> DelAsync(string ids)
        {
            var result = await _deliveryOrderService.DelAsync(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="deliveryOrderParamsDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> ExportAsync([FromQuery] DeliveryOrderParamsDTO deliveryOrderParamsDTO)
        {
            return await _deliveryOrderService.ExportAsync(deliveryOrderParamsDTO);
        }
    }
}
