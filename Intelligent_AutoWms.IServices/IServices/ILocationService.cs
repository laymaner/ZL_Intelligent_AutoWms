using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Location;
using Intelligent_AutoWms.Model.ResponseDTO.Location;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.IServices.IServices
{
    public interface ILocationService
    {
        /// <summary>
        /// 查询货位信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<List<WMS_Location>> GetListAsync([FromQuery] LocationParamsDTO locationParamsDTO);

        /// <summary>
        /// 查询货位分页
        /// </summary>
        /// <param name="locationParamsDTO"></param>
        /// <returns></returns>
        public Task<BasePagination<WMS_Location>> GetPaginationAsync([FromQuery] LocationParamsDTO locationParamsDTO);

        /// <summary>
        /// 根据id查询货位信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<WMS_Location> GetLocationByIdAsync(long id);

        /// <summary>
        /// 根据code查询货位信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<WMS_Location> GetLocationByCodeAsync(string code);

        /// <summary>
        /// 根据ids集合获取货位数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<List<WMS_Location>> GetLocationByIdsAsync(string ids);

        /// <summary>
        /// 根据codes集合获取货位数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public Task<List<WMS_Location>> GetLocationByCodesAsync(string codes);

        /// <summary>
        /// 判断货位是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<bool> IsExistAsync(string code);

        /// <summary>
        /// 创建货位
        /// </summary>
        /// <param name="createLocationDTO"></param>
        /// <returns></returns>
        public Task<long> CreateAsync([FromBody] CreateOrUpdateLocationDTO createLocationDTO, long currentUserId);

        /// <summary>
        /// 更新货位
        /// </summary>
        /// <param name="updateLocationDTO"></param>
        /// <returns></returns>
        public Task<long> UpdateAsync([FromBody] CreateOrUpdateLocationDTO updateLocationDTO, long currentUserId);

        /// <summary>
        /// 根据ids删除货位信息
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
        /// <param name="locationParamsDTO"></param>
        /// <returns></returns>
        public Task<FileStreamResult> ExportAsync([FromQuery] LocationParamsDTO locationParamsDTO);

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
        /// 获取货位选项集
        /// </summary>
        /// <param name="locationParamsDTO"></param>
        /// <returns></returns>
        public Task<List<LocationOptions>> GetLocationOptionsAsync([FromQuery] LocationParamsDTO locationParamsDTO);

        /// <summary>
        /// 入库推荐货位 根据入库口
        /// </summary>
        /// <param name="portCode"></param>
        /// <returns></returns>
        public Task<WMS_Location> RecommendedStorageLocationAsync(string portCode);

        /// <summary>
        /// 判断货位是否空闲
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<bool> IsIdleAsync(string code);

        /// <summary>
        /// 判断货位是否占用
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<bool> IsOccupyAsync(string code);

        /// <summary>
        /// 判断货位是否锁定
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<bool> IsLockAsync(string code);
    }
}
