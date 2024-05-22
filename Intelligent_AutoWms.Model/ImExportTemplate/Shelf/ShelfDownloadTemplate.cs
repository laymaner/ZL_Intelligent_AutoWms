using MiniExcelLibs.Attributes;

namespace Intelligent_AutoWms.Model.ImExportTemplate.Shelf
{
    /// <summary>
    /// 下载货架模板
    /// </summary>
    public class ShelfDownloadTemplate
    {
        /// <summary>
        /// 货架名称
        /// </summary>
        [ExcelColumn(Name = "货架名称", Index = 0, Width = 12)]
        public string Name { get; set; }

        /// <summary>
        /// 货架编码
        /// </summary>
        [ExcelColumn(Name = "货架编码", Index = 1, Width = 12)]
        public string Code { get; set; }

        /// <summary>
        /// 库区编码
        /// </summary>
        [ExcelColumn(Name = "库区编码", Index = 2, Width = 12)]
        public string Area_Code { get; set; }

        /// <summary>
        /// 巷道
        /// </summary>
        [ExcelColumn(Name = "巷道", Index = 3, Width = 12)]
        public int Lanway { get; set; }

        /// <summary>
        /// 排数
        /// </summary>
        [ExcelColumn(Name = "排数", Index = 4, Width = 12)]
        public int Shelf_Rows { get; set; }

        /// <summary>
        /// 列数
        /// </summary>
        [ExcelColumn(Name = "列数", Index = 5, Width = 12)]
        public int Shelf_Columns { get; set; }

        /// <summary>
        /// 层数
        /// </summary>
        [ExcelColumn(Name = "层数", Index = 6, Width = 12)]
        public int Shelf_Layers { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ExcelColumn(Name = "备注", Index = 7, Width = 40)]
        public string? Remark { get; set; }
    }
}
