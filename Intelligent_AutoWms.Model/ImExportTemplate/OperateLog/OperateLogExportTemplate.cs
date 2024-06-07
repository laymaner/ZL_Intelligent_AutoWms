using MiniExcelLibs.Attributes;

namespace Intelligent_AutoWms.Model.ImExportTemplate.OperateLog
{
    /// <summary>
    /// 操作日志导出模板
    /// </summary>
    public class OperateLogExportTemplate
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [ExcelColumn(Name = "ID", Index = 0, Width = 12)]
        public long Id { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [ExcelColumn(Name = "用户名称", Index = 1, Width = 12)]
        public string? User_Name { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        [ExcelColumn(Name = "用户编码", Index = 2, Width = 12)]
        public string? User_Code { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        [ExcelColumn(Name = "IP地址", Index = 3, Width = 12)]
        public string Ip_Address { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [ExcelColumn(Name = "标题", Index = 4, Width = 20)]
        public string? Title { get; set; }

        /// <summary>
        /// 方法名称
        /// </summary>
        [ExcelColumn(Name = "方法名称", Index = 5, Width = 20)]
        public string Method_Name { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [ExcelColumn(Name = "操作类型", Index = 6, Width = 20)]
        public string Operate_Type { get; set; }

        /// <summary>
        /// 操作路径
        /// </summary>
        [ExcelColumn(Name = "操作路径", Index = 7, Width = 40)]
        public string Operate_Url { get; set; }

        /// <summary>
        /// 操作状态
        /// </summary>
        [ExcelColumn(Name = "操作状态", Index = 8, Width = 50)]
        public int Operate_Status { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        [ExcelColumn(Name = "错误消息", Index = 9, Width = 50)]
        public string? Error_Msg { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ExcelColumn(Name = "备注", Index = 10, Width = 50)]
        public string? Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ExcelColumn(Name = "创建时间", Index = 11, Width = 40, Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime Create_Time { get; set; }
    }
}
