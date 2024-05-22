namespace Intelligent_AutoWms.Model.ResponseDTO.Location
{
    /// <summary>
    /// 货位选项集
    /// </summary>
    public class LocationOptions
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 货位编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 货位名称
        /// </summary>
        public string Name { get; set; }
    }
}
