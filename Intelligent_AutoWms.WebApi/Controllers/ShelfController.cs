using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Shelf;
using Intelligent_AutoWms.Model.ResponseDTO.Shelf;
using Intelligent_AutoWms.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 货架
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ShelfController : ApiControllerBase
    {
        private readonly IShelfService _ishelfService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shelfService"></param>
        public ShelfController(IShelfService shelfService)
        {
            _ishelfService = shelfService;
        }

        /// <summary>
        /// 查询货架信息
        /// </summary>
        /// <param name="shelfParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Shelf>>> GetListAsync([FromQuery] ShelfParamsDTO shelfParamsDTO)
        {
            var result = await _ishelfService.GetListAsync(shelfParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 查询货架分页
        /// </summary>
        /// <param name="shelfParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<BasePagination<WMS_Shelf>>> GetPaginationAsync([FromQuery] ShelfParamsDTO shelfParamsDTO)
        {
            var result = await _ishelfService.GetPaginationAsync(shelfParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据id查询货架信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Shelf>> GetShelfByIdAsync(long id)
        {
            var result = await _ishelfService.GetShelfByIdAsync(id);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids集合获取货架数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Shelf>>> GetShelfByIdsAsync(string ids)
        {
            var result = await _ishelfService.GetShelfByIdsAsync(ids);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据codes集合获取货架数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Shelf>>> GetShelfByCodesAsync(string codes)
        {
            var result = await _ishelfService.GetShelfByCodesAsync(codes);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据code查货架信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Shelf>> GetShelfByCodeAsync(string code)
        {
            var result = await _ishelfService.GetShelfByCodeAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 判断货架是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<bool>> IsExistAsync(string code)
        {
            var result = await _ishelfService.IsExistAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 创建货架
        /// </summary>
        /// <param name="createShelfDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> CreateAsync([FromBody] CreateOrUpdateShelfDTO createShelfDTO)
        {
            var result = await _ishelfService.CreateAsync(createShelfDTO, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 修改货架
        /// </summary>
        /// <param name="updateShelfDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> UpdateAsync([FromBody] CreateOrUpdateShelfDTO updateShelfDTO)
        {
            var result = await _ishelfService.UpdateAsync(updateShelfDTO, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据id删除货架信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Transation]
        public async Task<ApiResult<long>> DelAsync(string ids)
        {
            var result = await _ishelfService.DelAsync(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 下载Excel模板
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> DownloadTemplateAsync()
        {
            return await _ishelfService.DownloadTemplateAsync();
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="shelfParamsDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> ExportAsync([FromQuery] ShelfParamsDTO shelfParamsDTO)
        {
            return await _ishelfService.ExportAsync(shelfParamsDTO);
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
            var result = await _ishelfService.ImportAsync(path, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 导入----excel导入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<string>> ImportExcelAsync()
        {
            var fileForm = Request.Form.Files.FirstOrDefault();
            var result = await _ishelfService.ImportExcelAsync(fileForm, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 获取货架选项集
        /// </summary>
        /// <param name="shelfParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<ShelfOptions>>> GetShelfOptionsAsync([FromQuery] ShelfParamsDTO shelfParamsDTO)
        {
            var result = await _ishelfService.GetShelfOptionsAsync(shelfParamsDTO);
            return SuccessResult(result);
        }
    }
}
