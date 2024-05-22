using MiniExcelLibs.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intelligent_AutoWms.Model.ImExportTemplate.Shelf
{
    /// <summary>
    /// 货架导出模板
    /// </summary>
    public class ShelfExportTemplate
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [ExcelColumn(Name = "ID", Index = 0, Width = 12)]
        public long Id { get; set; }

        /// <summary>
        /// 货架名称
        /// </summary>
        [ExcelColumn(Name = "货架名称", Index = 1, Width = 12)]
        public string Name { get; set; }

        /// <summary>
        /// 货架编码
        /// </summary>
        [ExcelColumn(Name = "货架编码", Index = 2, Width = 12)]
        public string Code { get; set; }

        /// <summary>
        /// 库区编码
        /// </summary>
        [ExcelColumn(Name = "库区编码", Index = 3, Width = 12)]
        public string Area_Code { get; set; }

        /// <summary>
        /// 巷道数量
        /// </summary>
        [ExcelColumn(Name = "巷道数量", Index = 4, Width = 12)]
        public int Lanway { get; set; }

        /// <summary>
        /// 排数
        /// </summary>
        [ExcelColumn(Name = "排数", Index = 5, Width = 12)]
        public int Shelf_Rows { get; set; }

        /// <summary>
        /// 列数
        /// </summary>
        [ExcelColumn(Name = "列数", Index = 6, Width = 12)]
        public int Shelf_Columns { get; set; }

        /// <summary>
        /// 层数
        /// </summary>
        [ExcelColumn(Name = "层数", Index = 7, Width = 12)]
        public int Shelf_Layers { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ExcelColumn(Name = "备注", Index = 8, Width = 40)]
        public string? Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ExcelColumn(Name = "创建时间", Index = 9, Width = 40, Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime Create_Time { get; set; }
    }
}
