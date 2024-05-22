using MiniExcelLibs.Attributes;

namespace Intelligent_AutoWms.Model.ImExportTemplate.Role
{
    /// <summary>
    /// 角色下载模板
    /// </summary>
    public class RoleDownloadTemplate
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [ExcelColumn(Name = "名称", Index = 0, Width = 12)]
        public string Name { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        [ExcelColumn(Name = "编码", Index = 1, Width = 12)]
        public string Code { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        [ExcelColumn(Name = "描述", Index = 2, Width = 40)]
        public string? Description { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ExcelColumn(Name = "备注", Index = 3, Width = 40)]
        public string? Remark { get; set; }
    }
}
