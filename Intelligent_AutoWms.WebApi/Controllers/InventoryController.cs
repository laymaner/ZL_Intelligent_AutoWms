using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 库存明细
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ApiControllerBase
    {
        private readonly IInventoryService _inventoryService;

        /// <summary>
        /// 
        /// </summary>
        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// 查询库明细信息
        /// </summary>
        /// <param name="inventoryParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Inventory>>> GetListAsync([FromQuery] InventoryParamsDTO inventoryParamsDTO)
        {
            var result = await _inventoryService.GetListAsync(inventoryParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 查询库存明细信息分页
        /// </summary>
        /// <param name="inventoryParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<BasePagination<WMS_Inventory>>> GetPaginationAsync([FromQuery] InventoryParamsDTO inventoryParamsDTO)
        {
            var result = await _inventoryService.GetPaginationAsync(inventoryParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据id查询库存明细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Inventory>> GetInventoryByIdAsync(long id)
        {
            var result = await _inventoryService.GetInventoryByIdAsync(id);
            return SuccessResult(result);
        }


        /// <summary>
        /// 判断库存明细是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<bool>> IsExistAsync(long id)
        {
            var result = await _inventoryService.IsExistAsync(id);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids锁定库存明细
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<string>> LockInventoryAsync(List<long> ids)
        {
            var result = await _inventoryService.LockInventoryAsync(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids解锁库存明细
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<string>> UnLockInventoryAsync(List<long> ids)
        {
            var result = await _inventoryService.UnLockInventoryAsync(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 创建库存明细
        /// </summary>
        /// <param name="createInventoryDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> CreateAsync([FromBody] CreateInventoryDTO createInventoryDTO)
        {
            var result = await _inventoryService.CreateAsync(createInventoryDTO, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids删除库存明细
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Transation]
        public async Task<ApiResult<long>> DelAsync(string ids)
        {
            var result = await _inventoryService.DelAsync(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="inventoryParamsDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> ExportAsync([FromQuery] InventoryParamsDTO inventoryParamsDTO)
        {
            return await _inventoryService.ExportAsync(inventoryParamsDTO);
        }
    }
}
