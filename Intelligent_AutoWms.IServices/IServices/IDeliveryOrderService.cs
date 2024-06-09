using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.DeliveryOrder;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.IServices.IServices
{
    public interface IDeliveryOrderService
    {
        /// <summary>
        /// 查询出库单信息
        /// </summary>
        /// <param name="deliveryOrderParamsDTO"></param>
        /// <returns></returns>
        public Task<List<WMS_Delivery_Orders>> GetListAsync(DeliveryOrderParamsDTO deliveryOrderParamsDTO);

        /// <summary>
        /// 查询出库单信息分页
        /// </summary>
        /// <param name="deliveryOrderParamsDTO"></param>
        /// <returns></returns>
        public Task<BasePagination<WMS_Delivery_Orders>> GetPaginationAsync([FromQuery] DeliveryOrderParamsDTO deliveryOrderParamsDTO);

        /// <summary>
        /// 根据id查询出库单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<WMS_Delivery_Orders> GetDeliveryOrderByIdAsync(long id);

        /// <summary>
        /// 根据code获取出库单数据
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<WMS_Delivery_Orders> GetDeliveryOrderByCodeAsync(string code);

        /// <summary>
        /// 判断出库单是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> IsExistAsync(long id);

        /// <summary>
        /// 根据ids重新生成出库任务
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<string> RegenerateTaskByIdsAsync(List<long> ids, long currentUserId);

        /// <summary>
        /// 一键重新生成出库任务
        /// </summary>
        /// <returns></returns>
        public Task<string> RegenerateTaskAsync( long currentUserId);

        /// <summary>
        /// 创建出库单---指定出库口出库
        /// </summary>
        /// <param name="createDeliveryOrderDTO"></param>
        /// <returns></returns>
        public Task<long> CreateAsync([FromBody] CreateDeliveryOrderDTO createDeliveryOrderDTO, long currentUserId);

        /// <summary>
        /// 创建出库单---快速出库---物料本货位巷道出库
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<long> QuickyCreateAsync(List<long> ids, long currentUserId);

        /// <summary>
        /// 根据ids删除出库单
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<long> DelAsync(string ids, long currentUserId);

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="deliveryOrderParamsDTO"></param>
        /// <returns></returns>
        public Task<FileStreamResult> ExportAsync([FromQuery] DeliveryOrderParamsDTO deliveryOrderParamsDTO);
    }
}
