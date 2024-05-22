using MiniExcelLibs.Attributes;

namespace Intelligent_AutoWms.Model.ImExportTemplate.User
{
    /// <summary>
    /// 用户下载模板
    /// </summary>
    public class UserDownloadTemplate
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [ExcelColumn(Name = "名称", Index = 0, Width = 12)]
        public string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [ExcelColumn(Name = "编码", Index = 1, Width = 12)]
        public string Code { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        [ExcelColumn(Name = "年龄", Index = 2, Width = 12)]
        public int? Age { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [ExcelColumn(Name = "性别", Index = 3, Width = 12)]
        public string Gender { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [ExcelColumn(Name = "密码", Index = 4, Width = 30)]
        public string Password { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [ExcelColumn(Name = "出生日期", Index = 5, Width = 30, Format = "yyyy-MM-dd HH:mm:ss")]
        public string? Birth { get; set; }

        /// <summary>
        /// 电子邮件
        /// </summary>
        [ExcelColumn(Name = "电子邮件", Index = 6, Width = 30)]
        public string? Email { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [ExcelColumn(Name = "电话", Index = 7, Width = 30)]
        public string? Phone { get; set; }

        /// <summary>
        /// 居住地址
        /// </summary>
        [ExcelColumn(Name = "居住地址", Index = 8, Width = 30)]
        public string? Address { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ExcelColumn(Name = "备注", Index = 9, Width = 30)]
        public string? Remark { get; set; }

    }
}
