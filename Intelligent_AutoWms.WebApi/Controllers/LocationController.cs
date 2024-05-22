using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Location;
using Intelligent_AutoWms.Model.ResponseDTO.Location;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 货位
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class LocationController : ApiControllerBase
    {
        private readonly ILocationService _ilocationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationService"></param>
        public LocationController(ILocationService locationService)
        {
            _ilocationService = locationService;
        }

        /// <summary>
        /// 查询货位信息
        /// </summary>
        /// <param name="locationParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Location>>> GetListAsync([FromQuery] LocationParamsDTO locationParamsDTO)
        {
            var result = await _ilocationService.GetListAsync(locationParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 查询货位分页
        /// </summary>
        /// <param name="locationParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<BasePagination<WMS_Location>>> GetPaginationAsync([FromQuery] LocationParamsDTO locationParamsDTO)
        {
            var result = await _ilocationService.GetPaginationAsync(locationParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据id查询货位信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Location>> GetLocationByIdAsync(long id)
        {
            var result = await _ilocationService.GetLocationByIdAsync(id);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids集合获取货位数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Location>>> GetLocationByIdsAsync(string ids)
        {
            var result = await _ilocationService.GetLocationByIdsAsync(ids);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据codes集合获取货位数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Location>>> GetLocationByCodesAsync(string codes)
        {
            var result = await _ilocationService.GetLocationByCodesAsync(codes);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据code查货位信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Location>> GetLocationByCodeAsync(string code)
        {
            var result = await _ilocationService.GetLocationByCodeAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 判断货位是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<bool>> IsExistAsync(string code)
        {
            var result = await _ilocationService.IsExistAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 判断货位是空闲
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<bool>> IsIdleAsync(string code)
        {
            var result = await _ilocationService.IsIdleAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 判断货位是否锁定
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<bool>> IsLockAsync(string code)
        {
            var result = await _ilocationService.IsLockAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 判断货位是否占用
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<bool>> IsOccupyAsync(string code)
        {
            var result = await _ilocationService.IsOccupyAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 创建货位
        /// </summary>
        /// <param name="createLocationDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> CreateAsync([FromBody] CreateOrUpdateLocationDTO createLocationDTO)
        {
            var result = await _ilocationService.CreateAsync(createLocationDTO, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 修改货位
        /// </summary>
        /// <param name="updateLocationDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> UpdateAsync([FromBody] CreateOrUpdateLocationDTO updateLocationDTO)
        {
            var result = await _ilocationService.UpdateAsync(updateLocationDTO, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据id删除货位信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Transation]
        public async Task<ApiResult<long>> DelAsync(string ids)
        {
            var result = await _ilocationService.DelAsync(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 下载Excel模板
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> DownloadTemplateAsync()
        {
            return await _ilocationService.DownloadTemplateAsync();
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="locationParamsDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> ExportAsync([FromQuery] LocationParamsDTO locationParamsDTO)
        {
            return await _ilocationService.ExportAsync(locationParamsDTO);
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
            var result = await _ilocationService.ImportAsync(path, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }
        /// <summary>
        /// 获取货位选项集
        /// </summary>
        /// <param name="locationParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<LocationOptions>>> GetLocationOptionsAsync([FromQuery] LocationParamsDTO locationParamsDTO)
        {
            var result = await _ilocationService.GetLocationOptionsAsync(locationParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 入库推荐货位 根据入库口
        /// </summary>
        /// <param name="portCode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Location>> RecommendedStorageLocationAsync(string portCode)
        {
            var result = await _ilocationService.RecommendedStorageLocationAsync(portCode);
            return SuccessResult(result);
        }
    }
}
