using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Area;
using Intelligent_AutoWms.Model.ResponseDTO.Area;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 库区
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AreaController : ApiControllerBase
    {
        private readonly IAreaService _iareaService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="areaService"></param>
        public AreaController(IAreaService areaService)
        {
            _iareaService = areaService;
        }

        /// <summary>
        /// 查询库区信息
        /// </summary>
        /// <param name="areaParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Area>>> GetListAsync([FromQuery] AreaParamsDTO areaParamsDTO)
        {
            var result = await _iareaService.GetListAsync(areaParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 查询库区分页
        /// </summary>
        /// <param name="areaParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<BasePagination<WMS_Area>>> GetPaginationAsync([FromQuery] AreaParamsDTO areaParamsDTO)
        {
            var result = await _iareaService.GetPaginationAsync(areaParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据id查询库区信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Area>> GetAreaByIdAsync(long id)
        {
            var result = await _iareaService.GetAreaByIdAsync(id);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids集合获取库区数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Area>>> GetAreaByIdsAsync(string ids)
        {
            var result = await _iareaService.GetAreaByIdsAsync(ids);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据codes集合获取库区数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Area>>> GetAreaByCodesAsync(string codes)
        {
            var result = await _iareaService.GetAreaByCodesAsync(codes);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据code查库区信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Area>> GetAreaByCodeAsync(string code)
        {
            var result = await _iareaService.GetAreaByCodeAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 判断库区是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<bool>> IsExistAsync(string code)
        {
            var result = await _iareaService.IsExistAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 创建库区
        /// </summary>
        /// <param name="createAreaDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> CreateAsync([FromBody]  CreateOrUpdateAreaDTO createAreaDTO)
        {
            var result = await _iareaService.CreateAsync(createAreaDTO, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 修改库区
        /// </summary>
        /// <param name="updateAreaDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> UpdateAsync([FromBody] CreateOrUpdateAreaDTO updateAreaDTO)
        {
            var result = await _iareaService.UpdateAsync(updateAreaDTO, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据id删除库区信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Transation]
        public async Task<ApiResult<long>> DelAsync(string ids)
        {
            var result = await _iareaService.DelAsync(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 下载Excel模板
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> DownloadTemplateAsync()
        {
            return await _iareaService.DownloadTemplateAsync();
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="areaParamsDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> ExportAsync([FromQuery] AreaParamsDTO areaParamsDTO)
        {
            return await _iareaService.ExportAsync(areaParamsDTO);
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
            var result = await _iareaService.ImportAsync(path, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 获取库区选项集
        /// </summary>
        /// <param name="areaParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<AreaOptions>>> GetAreaOptionsAsync([FromQuery] AreaParamsDTO areaParamsDTO)
        {
            var result = await _iareaService.GetAreaOptionsAsync(areaParamsDTO);
            return SuccessResult(result);
        }
    }
}
