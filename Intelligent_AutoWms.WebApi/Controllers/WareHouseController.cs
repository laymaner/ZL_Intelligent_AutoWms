using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Warehouse;
using Intelligent_AutoWms.Model.RequestDTO.WareHouse;
using Intelligent_AutoWms.Model.ResponseDTO.WareHouse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 仓库
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class WareHouseController : ApiControllerBase
    {
        private readonly IWareHouseService _iwareHouseService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wareHouseService"></param>
        public WareHouseController(IWareHouseService wareHouseService)
        {
            _iwareHouseService = wareHouseService;
        }

        /// <summary>
        /// 查询仓库信息
        /// </summary>
        /// <param name="wareHouseParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_WareHouse>>> GetListAsync([FromQuery] WareHouseParamsDTO wareHouseParamsDTO)
        {
            var result = await _iwareHouseService.GetListAsync(wareHouseParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 查询仓库分页
        /// </summary>
        /// <param name="wareHouseParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<BasePagination<WMS_WareHouse>>> GetPaginationAsync([FromQuery] WareHouseParamsDTO wareHouseParamsDTO)
        {
            var result = await _iwareHouseService.GetPaginationAsync(wareHouseParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据id查询仓库信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_WareHouse>> GetWareHouseByIdAsync(long id)
        {
            var result = await _iwareHouseService.GetWareHouseByIdAsync(id);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids集合获取仓库数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_WareHouse>>> GetWareHouseByIdsAsync(string ids)
        {
            var result = await _iwareHouseService.GetWareHouseByIdsAsync(ids);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据codes集合获取仓库数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_WareHouse>>> GetWareHouseByCodesAsync(string codes)
        {
            var result = await _iwareHouseService.GetWareHouseByCodesAsync(codes);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据code查仓库信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_WareHouse>> GetWareHouseByCodeAsync(string code)
        {
            var result = await _iwareHouseService.GetWareHouseByCodeAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 判断仓库是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<bool>> IsExistAsync(string code)
        {
            var result = await _iwareHouseService.IsExistAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 创建仓库
        /// </summary>
        /// <param name="createWareHouseDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> CreateAsync([FromBody] CreateOrUpdateWareHouseDTO createWareHouseDTO)
        {
            var result = await _iwareHouseService.CreateAsync(createWareHouseDTO, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 更新仓库
        /// </summary>
        /// <param name="updateWareHouseDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> UpdateAsync([FromBody] CreateOrUpdateWareHouseDTO updateWareHouseDTO)
        {
            var result = await _iwareHouseService.UpdateAsync(updateWareHouseDTO, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids删除仓库信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Transation]
        public async Task<ApiResult<long>> DelAsync(string ids)
        {
            var result = await _iwareHouseService.DelAsync(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 下载Excel模板
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> DownloadTemplateAsync()
        {
            return await _iwareHouseService.DownloadTemplateAsync();
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="wareHouseParamsDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> ExportAsync([FromQuery] WareHouseParamsDTO wareHouseParamsDTO)
        {
            return await _iwareHouseService.ExportAsync(wareHouseParamsDTO);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<string>> ImportAsync(string path)
        {
            var result = await _iwareHouseService.ImportAsync(path, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 获取仓库选项集
        /// </summary>
        /// <param name="wareHouseParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WareHouseOptions>>> GetWareHouseOptionsAsync([FromQuery] WareHouseParamsDTO wareHouseParamsDTO)
        {
            var result = await _iwareHouseService.GetWareHouseOptionsAsync(wareHouseParamsDTO);
            return SuccessResult(result);
        }
    }
}
