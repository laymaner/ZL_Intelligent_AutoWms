using MiniExcelLibs.Attributes;

namespace Intelligent_AutoWms.Model.ImExportTemplate.Area
{
    /// <summary>
    /// 库区信息导出模板
    /// </summary>
    public class AreaExportTemplate
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [ExcelColumn(Name = "ID", Index = 0, Width = 12)]
        public long Id { get; set; }

        /// <summary>
        /// 库区名称
        /// </summary>
        [ExcelColumn(Name = "库区名称", Index = 1, Width = 12)]
        public string Name { get; set; }

        /// <summary>
        /// 库区编码
        /// </summary>
        [ExcelColumn(Name = "库区编码", Index = 2, Width = 12)]
        public string Code { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        [ExcelColumn(Name = "仓库编码", Index = 3, Width = 12)]
        public string WareHouse_Code { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ExcelColumn(Name = "备注", Index = 4, Width = 40)]
        public string? Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ExcelColumn(Name = "创建时间", Index = 5, Width = 40, Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime Create_Time { get; set; }
    }
}
