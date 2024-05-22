using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Inventory;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.IServices.IServices
{
    public interface IInventoryService
    {
        /// <summary>
        /// 查询库明细信息
        /// </summary>
        /// <param name="inventoryParamsDTO"></param>
        /// <returns></returns>
        public Task<List<WMS_Inventory>> GetListAsync([FromQuery] InventoryParamsDTO inventoryParamsDTO);

        /// <summary>
        /// 查询库存明细信息分页
        /// </summary>
        /// <param name="inventoryParamsDTO"></param>
        /// <returns></returns>
        public Task<BasePagination<WMS_Inventory>> GetPaginationAsync([FromQuery] InventoryParamsDTO inventoryParamsDTO);

        /// <summary>
        /// 根据id查询库存明细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<WMS_Inventory> GetInventoryByIdAsync(long id);

        /// <summary>
        /// 判断库存明细是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> IsExistAsync(long id);

        /// <summary>
        /// 根据ids锁定库存明细
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<string> LockInventoryAsync(List<long> ids, long currentUserId);

        /// <summary>
        /// 根据ids解锁库存明细
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<string> UnLockInventoryAsync(List<long> ids, long currentUserId);

        /// <summary>
        /// 创建库存明细
        /// </summary>
        /// <param name="createInventoryDTO"></param>
        /// <returns></returns>
        public Task<long> CreateAsync([FromBody] CreateInventoryDTO createInventoryDTO, long currentUserId);

        /// <summary>
        /// 根据ids删除库存明细
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<long> DelAsync(string ids, long currentUserId);

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="inventoryParamsDTO"></param>
        /// <returns></returns>
        public Task<FileStreamResult> ExportAsync([FromQuery] InventoryParamsDTO inventoryParamsDTO);

    }
}
