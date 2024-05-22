using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Port;
using Intelligent_AutoWms.Model.ResponseDTO.Port;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 出入口
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PortController : ApiControllerBase
    {
        private readonly IPortService _portService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portService"></param>
        public PortController(IPortService portService)
        {
            _portService = portService;
        }

        /// <summary>
        /// 查询出入口信息
        /// </summary>
        /// <param name="portParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Port>>> GetListAsync([FromQuery] PortParamsDTO portParamsDTO)
        {
            var result = await _portService.GetListAsync(portParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 查询出入口分页
        /// </summary>
        /// <param name="portParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<BasePagination<WMS_Port>>> GetPaginationAsync([FromQuery] PortParamsDTO portParamsDTO)
        {
            var result = await _portService.GetPaginationAsync(portParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据id查询出入口信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Port>> GetPortByIdAsync(long id)
        {
            var result = await _portService.GetPortByIdAsync(id);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据code查询出入口信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Port>> GetPortByCodeAsync(string code)
        {
            var result = await _portService.GetPortByCodeAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids集合获取出入口数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Port>>> GetPortByIdsAsync(string ids)
        {
            var result = await _portService.GetPortByIdsAsync(ids);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据codes集合获取出入口数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Port>>> GetPortByCodesAsync(string codes)
        {
            var result = await _portService.GetPortByCodesAsync(codes);
            return SuccessResult(result);
        }

        /// <summary>
        /// 判断出入口是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<bool>> IsExistAsync(string code)
        {
            var result = await _portService.IsExistAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 创建出入口
        /// </summary>
        /// <param name="createPortDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> CreateAsync([FromBody] CreateOrUpdatePortDTO createPortDTO)
        {
            var result = await _portService.CreateAsync(createPortDTO, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 修改出入口
        /// </summary>
        /// <param name="updatePortDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> UpdateAsync([FromBody] CreateOrUpdatePortDTO updatePortDTO)
        {
            var result = await _portService.UpdateAsync(updatePortDTO, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids删除出入口信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Transation]
        public async Task<ApiResult<long>> DelAsync(string ids)
        {
            var result = await _portService.DelAsync(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 下载Excel模板
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> DownloadTemplateAsync()
        {
            return await _portService.DownloadTemplateAsync();
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="portParamsDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> ExportAsync([FromQuery] PortParamsDTO portParamsDTO)
        {
            return await _portService.ExportAsync(portParamsDTO);
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
            var result = await _portService.ImportAsync(path, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 获取出入口选项集
        /// </summary>
        /// <param name="portParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<PortOptions>>> GetPortOptionsAsync([FromQuery] PortParamsDTO portParamsDTO)
        {
            var result = await _portService.GetPortOptionsAsync(portParamsDTO);
            return SuccessResult(result);
        }
    }
}
