using MiniExcelLibs.Attributes;

namespace Intelligent_AutoWms.Model.ImExportTemplate.WareHouse
{
    /// <summary>
    /// 下载仓库模板
    /// </summary>
    public class WareHouseDownloadTemplate
    {
        /// <summary>
        /// 仓库名称
        /// </summary>
        [ExcelColumn(Name = "仓库名称", Index = 0, Width = 12)]
        public string Name { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        [ExcelColumn(Name = "仓库编码", Index = 1, Width = 12)]
        public string Code { get; set; }

        /// <summary>
        /// 仓库类型
        /// </summary>
        [ExcelColumn(Name = "仓库类型", Index = 2, Width = 12)]
        public string Type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ExcelColumn(Name = "备注", Index = 3, Width = 40)]
        public string? Remark { get; set; }


    }
}
