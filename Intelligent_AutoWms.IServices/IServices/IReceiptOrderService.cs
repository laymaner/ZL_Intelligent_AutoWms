using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.ReceiptOrder;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.IServices.IServices
{
    public interface IReceiptOrderService
    {
        /// <summary>
        /// 查询入库单信息
        /// </summary>
        /// <param name="receiptOrderParamsDTO"></param>
        /// <returns></returns>
        public Task<List<WMS_Receipt_Orders>> GetListAsync(ReceiptOrderParamsDTO receiptOrderParamsDTO);

        /// <summary>
        /// 查询入库单信息分页
        /// </summary>
        /// <param name="receiptOrderParamsDTO"></param>
        /// <returns></returns>
        public Task<BasePagination<WMS_Receipt_Orders>> GetPaginationAsync([FromQuery] ReceiptOrderParamsDTO receiptOrderParamsDTO);

        /// <summary>
        /// 根据id查询入库单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<WMS_Receipt_Orders> GetReceiptOrderByIdAsync(long id);

        /// <summary>
        /// 根据code查询入库单信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<WMS_Receipt_Orders> GetReceiptOrderByCodeAsync(string code);

        /// <summary>
        /// 根据id判断入库单是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<bool> IsExistAsync(long id);

        /// <summary>
        /// 根据ids重新生成入库任务
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<string> RegenerateTaskAsync(List<long> ids, long currentUserId);

        /// <summary>
        /// 创建入库单
        /// </summary>
        /// <param name="createReceiptOrderDTO"></param>
        /// <returns></returns>
        public Task<long> CreateAsync([FromBody] CreateReceiptOrderDTO createReceiptOrderDTO, long currentUserId);

        /// <summary>
        /// 根据ids删除入库单
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<long> DelAsync(string ids, long currentUserId);

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="receiptOrderParamsDTO"></param>
        /// <returns></returns>
        public Task<FileStreamResult> ExportAsync([FromQuery] ReceiptOrderParamsDTO receiptOrderParamsDTO);
    }
}
