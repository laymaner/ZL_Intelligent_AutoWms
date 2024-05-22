using MiniExcelLibs.Attributes;

namespace Intelligent_AutoWms.Model.ImExportTemplate.Location
{
    /// <summary>
    /// 下载货位模板
    /// </summary>
    public class LocationDownloadTemplate
    {
        /// <summary>
        /// 货位名称
        /// </summary>
        [ExcelColumn(Name = "货位名称", Index = 0, Width = 12)]
        public string Name { get; set; }

        /// <summary>
        /// 货位编码
        /// </summary>
        [ExcelColumn(Name = "货位编码", Index = 1, Width = 12)]
        public string Code { get; set; }

        /// <summary>
        /// 货架编码
        /// </summary>
        [ExcelColumn(Name = "货架编码", Index = 2, Width = 12)]
        public string Shelf_Code { get; set; }

        /// <summary>
        /// 巷道
        /// </summary>
        [ExcelColumn(Name = "巷道", Index = 3, Width = 12)]
        public int Lanway { get; set; }

        /// <summary>
        /// 排
        /// </summary>
        [ExcelColumn(Name = "排", Index = 4, Width = 12)]
        public int Location_Row { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        [ExcelColumn(Name = "列", Index = 5, Width = 12)]
        public int Location_Column { get; set; }

        /// <summary>
        /// 层
        /// </summary>
        [ExcelColumn(Name = "层", Index = 6, Width = 12)]
        public int Location_Layer { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ExcelColumn(Name = "备注", Index = 7, Width = 40)]
        public string? Remark { get; set; }
    }
}
