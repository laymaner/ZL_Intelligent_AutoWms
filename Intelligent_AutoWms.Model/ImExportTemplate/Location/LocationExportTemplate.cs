using MiniExcelLibs.Attributes;

namespace Intelligent_AutoWms.Model.ImExportTemplate.Location
{
    /// <summary>
    /// 货位导出模板
    /// </summary>
    public class LocationExportTemplate
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [ExcelColumn(Name = "ID", Index = 0, Width = 12)]
        public long Id { get; set; }

        /// <summary>
        /// 货位名称
        /// </summary>
        [ExcelColumn(Name = "货位名称", Index = 1, Width = 12)]
        public string Name { get; set; }

        /// <summary>
        /// 货位编码
        /// </summary>
        [ExcelColumn(Name = "货位编码", Index = 2, Width = 12)]
        public string Code { get; set; }

        /// <summary>
        /// 货架编码
        /// </summary>
        [ExcelColumn(Name = "货架编码", Index = 3, Width = 12)]
        public string Shelf_Code { get; set; }

        /// <summary>
        /// 巷道
        /// </summary>
        [ExcelColumn(Name = "巷道", Index = 4, Width = 12)]
        public int Lanway { get; set; }

        /// <summary>
        /// 排
        /// </summary>
        [ExcelColumn(Name = "排", Index = 5, Width = 12)]
        public int Location_Row { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        [ExcelColumn(Name = "列", Index = 6, Width = 12)]
        public int Location_Column { get; set; }

        /// <summary>
        /// 层
        /// </summary>
        [ExcelColumn(Name = "层", Index = 7, Width = 12)]
        public int Location_Layer { get; set; }

        /// <summary>
        /// 货位状态
        /// </summary>
        [ExcelColumn(Name = "货位状态", Index = 8, Width = 12)]
        public string LocationStatus { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ExcelColumn(Name = "备注", Index = 9, Width = 40)]
        public string? Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ExcelColumn(Name = "创建时间", Index = 10, Width = 40, Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime Create_Time { get; set; }
    }
}
