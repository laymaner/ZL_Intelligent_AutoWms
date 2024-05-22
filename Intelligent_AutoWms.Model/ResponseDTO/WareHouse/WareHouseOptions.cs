using System.ComponentModel.DataAnnotations.Schema;

namespace Intelligent_AutoWms.Model.ResponseDTO.WareHouse
{
    /// <summary>
    /// 仓库选项集
    /// </summary>
    public class WareHouseOptions
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 仓库类型
        /// </summary>
        public string Type { get; set; }
    }
}
