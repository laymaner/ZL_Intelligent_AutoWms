using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Warehouse;
using Intelligent_AutoWms.Model.RequestDTO.WareHouse;
using Intelligent_AutoWms.Model.ResponseDTO.WareHouse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.IServices.IServices
{
    public interface IWareHouseService
    {
        /// <summary>
        /// 查询仓库信息
        /// </summary>
        /// <param name="wareHouseParamsDTO"></param>
        /// <returns></returns>
        public Task<List<WMS_WareHouse>> GetListAsync([FromQuery] WareHouseParamsDTO wareHouseParamsDTO);

        /// <summary>
        /// 查询仓库分页
        /// </summary>
        /// <param name="wareHouseParamsDTO"></param>
        /// <returns></returns>
        public Task<BasePagination<WMS_WareHouse>> GetPaginationAsync([FromQuery] WareHouseParamsDTO wareHouseParamsDTO);

        /// <summary>
        /// 根据id查询仓库信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<WMS_WareHouse> GetWareHouseByIdAsync(long id);

        /// <summary>
        /// 根据code查询仓库信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<WMS_WareHouse> GetWareHouseByCodeAsync(string code);

        /// <summary>
        /// 根据ids集合获取仓库数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<List<WMS_WareHouse>> GetWareHouseByIdsAsync(string ids);

        /// <summary>
        /// 根据codes集合获取仓库数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public Task<List<WMS_WareHouse>> GetWareHouseByCodesAsync(string codes);

        /// <summary>
        /// 判断仓库是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<bool> IsExistAsync(string code);

        /// <summary>
        /// 创建仓库
        /// </summary>
        /// <param name="createWareHouseDTO"></param>
        /// <returns></returns>
        public Task<long> CreateAsync([FromBody] CreateOrUpdateWareHouseDTO createWareHouseDTO, long currentUserId);

        /// <summary>
        /// 修改仓库
        /// </summary>
        /// <param name="updateWareHouseDTO"></param>
        /// <returns></returns>
        public Task<long> UpdateAsync([FromBody] CreateOrUpdateWareHouseDTO updateWareHouseDTO, long currentUserId);

        /// <summary>
        /// 根据ids删除仓库信息
        /// </summary>
        /// <param name="id"></param>
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
        /// <param name="wareHouseParamsDTO"></param>
        /// <returns></returns>
        public Task<FileStreamResult> ExportAsync([FromQuery] WareHouseParamsDTO wareHouseParamsDTO);

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
        /// 获取仓库选项集
        /// </summary>
        /// <param name="wareHouseParamsDTO"></param>
        /// <returns></returns>
        public Task<List<WareHouseOptions>> GetWareHouseOptionsAsync([FromQuery] WareHouseParamsDTO wareHouseParamsDTO);
    }
}
