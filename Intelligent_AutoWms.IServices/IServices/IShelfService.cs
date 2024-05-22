using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Shelf;
using Intelligent_AutoWms.Model.ResponseDTO.Shelf;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.IServices.IServices
{
    public interface IShelfService
    {
        /// <summary>
        /// 查询货架信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task<List<WMS_Shelf>> GetListAsync([FromQuery] ShelfParamsDTO shelfParamsDTO);

        /// <summary>
        /// 查询货架分页
        /// </summary>
        /// <param name="factoryParamsDTO"></param>
        /// <returns></returns>
        public Task<BasePagination<WMS_Shelf>> GetPaginationAsync([FromQuery] ShelfParamsDTO shelfParamsDTO);

        /// <summary>
        /// 根据id查询货架信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<WMS_Shelf> GetShelfByIdAsync(long id);

        /// <summary>
        /// 根据code查询货架信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<WMS_Shelf> GetShelfByCodeAsync(string code);

        /// <summary>
        /// 根据ids集合获取货架数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<List<WMS_Shelf>> GetShelfByIdsAsync(string ids);

        /// <summary>
        /// 根据codes集合获取货架数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public Task<List<WMS_Shelf>> GetShelfByCodesAsync(string codes);

        /// <summary>
        /// 判断货架是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<bool> IsExistAsync(string code);

        /// <summary>
        /// 创建货架
        /// </summary>
        /// <param name="createShelfDTO"></param>
        /// <returns></returns>
        public Task<long> CreateAsync([FromBody] CreateOrUpdateShelfDTO createShelfDTO, long currentUserId);

        /// <summary>
        /// 更新货架
        /// </summary>
        /// <param name="updateShelfDTO"></param>
        /// <returns></returns>
        public Task<long> UpdateAsync([FromBody] CreateOrUpdateShelfDTO updateShelfDTO, long currentUserId);

        /// <summary>
        /// 根据ids删除货架信息
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
        /// <param name=""></param>
        /// <returns></returns>
        public Task<FileStreamResult> ExportAsync([FromQuery] ShelfParamsDTO shelfParamsDTO);

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="path"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public Task<string> ImportAsync(string path, long currentUserId);

        /// <summary>
        /// 获取货架选项集
        /// </summary>
        /// <param name="shelfParamsDTO"></param>
        /// <returns></returns>
        public Task<List<ShelfOptions>> GetShelfOptionsAsync([FromQuery] ShelfParamsDTO shelfParamsDTO);
    }
}
