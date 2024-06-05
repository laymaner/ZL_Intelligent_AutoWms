using Intelligent_AutoWms.Model.BaseModel;

namespace Intelligent_AutoWms.Model.RequestDTO.Location
{
    /// <summary>
    /// 查询货位实体参数
    /// </summary>
    public class LocationParamsDTO:BasicQuery
    {
        /// <summary>
        /// 货位编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 货位名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 货架id
        /// </summary>
        public long? ShelfId { get; set; }

        /// <summary>
        /// 1:货位空闲 2:入库锁定 3:出库锁定 4:占用中
        /// </summary>
        public int? Step { get; set; }
    }
}
