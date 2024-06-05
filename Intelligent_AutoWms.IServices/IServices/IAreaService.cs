using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Area;
using Intelligent_AutoWms.Model.ResponseDTO.Area;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.IServices.IServices
{
    public interface IAreaService
    {
        /// <summary>
        /// 查询库区信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<List<WMS_Area>> GetListAsync([FromQuery] AreaParamsDTO areaParamsDTO);

        /// <summary>
        /// 查询库区分页
        /// </summary>
        /// <param name="areaParamsDTO"></param>
        /// <returns></returns>
        public Task<BasePagination<WMS_Area>> GetPaginationAsync([FromQuery] AreaParamsDTO areaParamsDTO);

        /// <summary>
        /// 根据id查询库区信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<WMS_Area> GetAreaByIdAsync(long id);

        /// <summary>
        /// 根据code查询库区信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<WMS_Area> GetAreaByCodeAsync(string code);

        /// <summary>
        /// 根据ids集合获取库区数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<List<WMS_Area>> GetAreaByIdsAsync(string ids);

        /// <summary>
        /// 根据codes集合获取库区数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public Task<List<WMS_Area>> GetAreaByCodesAsync(string codes);

        /// <summary>
        /// 判断库区是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<bool> IsExistAsync(string code);

        /// <summary>
        /// 创建库区
        /// </summary>
        /// <param name="createAreaDTO"></param>
        /// <returns></returns>
        public Task<long> CreateAsync([FromBody] CreateOrUpdateAreaDTO createAreaDTO, long currentUserId);

        /// <summary>
        /// 更新库区
        /// </summary>
        /// <param name="updateAreaDTO"></param>
        /// <returns></returns>
        public Task<long> UpdateAsync([FromBody] CreateOrUpdateAreaDTO updateAreaDTO, long currentUserId);

        /// <summary>
        /// 根据ids删除库区信息
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
        /// <param name="areaParamsDTO"></param>
        /// <returns></returns>
        public Task<FileStreamResult> ExportAsync([FromQuery] AreaParamsDTO areaParamsDTO);

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
        /// 获取库区选项集
        /// </summary>
        /// <param name="areaParamsDTO"></param>
        /// <returns></returns>
        public Task<List<AreaOptions>> GetAreaOptionsAsync([FromQuery] AreaParamsDTO areaParamsDTO);
    }
}
