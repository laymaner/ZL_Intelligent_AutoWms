using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Port;
using Intelligent_AutoWms.Model.ResponseDTO.Port;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.IServices.IServices
{
    public interface IPortService
    {
        /// <summary>
        /// 查询出入口信息
        /// </summary>
        /// <param name="portParamsDTO"></param>
        /// <returns></returns>
        public Task<List<WMS_Port>> GetListAsync([FromQuery] PortParamsDTO portParamsDTO);

        /// <summary>
        /// 查询出入口分页
        /// </summary>
        /// <param name="portParamsDTO"></param>
        /// <returns></returns>
        public Task<BasePagination<WMS_Port>> GetPaginationAsync([FromQuery] PortParamsDTO portParamsDTO);

        /// <summary>
        /// 根据id查询出入口信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<WMS_Port> GetPortByIdAsync(long id);

        /// <summary>
        /// 根据code查询出入口信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<WMS_Port> GetPortByCodeAsync(string code);

        /// <summary>
        /// 根据ids集合获取出入口数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<List<WMS_Port>> GetPortByIdsAsync(string ids);

        /// <summary>
        /// 根据codes集合获取出入口数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public Task<List<WMS_Port>> GetPortByCodesAsync(string codes);

        /// <summary>
        /// 判断出入口是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<bool> IsExistAsync(string code);

        /// <summary>
        /// 创建出入口
        /// </summary>
        /// <param name="createPortDTO"></param>
        /// <returns></returns>
        public Task<long> CreateAsync([FromBody] CreateOrUpdatePortDTO createPortDTO, long currentUserId);

        /// <summary>
        /// 修改出入口
        /// </summary>
        /// <param name="updatePortDTO"></param>
        /// <returns></returns>
        public Task<long> UpdateAsync([FromBody] CreateOrUpdatePortDTO updatePortDTO, long currentUserId);

        /// <summary>
        /// 根据ids删除出入口信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<long> DelAsync(string ids, long currentUserId);

        /// <summary>
        /// 下载Excel模板
        /// </summary>
        /// <returns></returns>
        public Task<FileStreamResult> DownloadTemplateAsync();

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="portParamsDTO"></param>
        /// <returns></returns>
        public Task<FileStreamResult> ExportAsync([FromQuery] PortParamsDTO portParamsDTO);

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="path"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public Task<string> ImportAsync(string path, long currentUserId);

        /// <summary>
        /// 导入----excel导入
        /// </summary>
        /// <param name="fileForm"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public Task<string> ImportExcelAsync(IFormFile fileForm, long currentUserId);

        /// <summary>
        /// 获取出入口选项集
        /// </summary>
        /// <param name="portParamsDTO"></param>
        /// <returns></returns>
        public Task<List<PortOptions>> GetPortOptionsAsync([FromQuery] PortParamsDTO portParamsDTO);
    }
}
