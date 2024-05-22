using MiniExcelLibs.Attributes;

namespace Intelligent_AutoWms.Model.ImExportTemplate.Port
{
    /// <summary>
    /// 出入口下载模板
    /// </summary>
    public class PortDownloadTemplate
    {
        /// <summary>
        /// 出入口名称
        /// </summary>
        [ExcelColumn(Name = "名称", Index = 0, Width = 12)]
        public string Name { get; set; }

        /// <summary>
        /// 出入口编码
        /// </summary>
        [ExcelColumn(Name = "编码", Index = 1, Width = 12)]
        public string Code { get; set; }

        /// <summary>
        /// 类型 1：入口 2：出口
        /// </summary>
        [ExcelColumn(Name = "类型", Index = 2, Width = 12)]
        public int Type { get; set; }

        /// <summary>
        /// 仓库id
        /// </summary>
        [ExcelColumn(Name = "仓库编码", Index = 3, Width = 12)]
        public string Warehouse_Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ExcelColumn(Name = "推荐巷道一", Index = 4, Width = 12)]
        public int First_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ExcelColumn(Name = "推荐巷道二", Index = 5, Width = 12)]
        public int Second_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ExcelColumn(Name = "推荐巷道三", Index = 6, Width = 12)]
        public int Third_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ExcelColumn(Name = "推荐巷道四", Index = 7, Width = 12)]
        public int Forth_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ExcelColumn(Name = "推荐巷道五", Index = 8, Width = 12)]
        public int Fifth_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ExcelColumn(Name = "推荐巷道六", Index = 9, Width = 12)]
        public int Sixth_Lanway { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ExcelColumn(Name = "备注", Index = 10, Width = 40)]
        public string? Remark { get; set; }
    }
}
