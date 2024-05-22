using MiniExcelLibs.Attributes;

namespace Intelligent_AutoWms.Model.ImExportTemplate.ReceiptOrder
{
    /// <summary>
    /// 入库单导出模板
    /// </summary>
    public class ReceiptOrderExportTemplate
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [ExcelColumn(Name = "ID", Index = 0, Width = 12)]
        public long Id { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [ExcelColumn(Name = "订单编号", Index = 1, Width = 30)]
        public string Order_No { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        [ExcelColumn(Name = "订单类型", Index = 2, Width = 12)]
        public string Order_Type { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        [ExcelColumn(Name = "物料编码", Index = 3, Width = 30)]
        public string Material_Code { get; set; }

        /// <summary>
        /// 物料类型
        /// </summary>
        [ExcelColumn(Name = "物料类型", Index = 4, Width = 12)]
        public int Material_Type { get; set; }

        /// <summary>
        /// 存储位置id
        /// </summary>
        [ExcelColumn(Name = "存储位置ID", Index = 5, Width = 12)]
        public long Location_Id { get; set; }

        /// <summary>
        /// 存储位置编码
        /// </summary>
        [ExcelColumn(Name = "存储位置编码", Index = 6, Width = 20)]
        public string Location_Code { get; set; }

        /// <summary>
        /// 入口id
        /// </summary>
        [ExcelColumn(Name = "入口ID", Index = 7, Width = 12)]
        public long Port_Id { get; set; }

        /// <summary>
        /// 入口编码
        /// </summary>
        [ExcelColumn(Name = "入口编码", Index = 8, Width = 20)]
        public string Port_Code { get; set; }

        /// <summary>
        /// 入库时间
        /// </summary>
        [ExcelColumn(Name = "入库时间", Index = 9, Width = 40, Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime? Receipt_Time { get; set; }

        /// <summary>
        /// 入库步骤：1--待入库 2--已入库
        /// </summary>
        [ExcelColumn(Name = "出库步骤", Index = 10, Width = 12)]
        public int Receipt_Step { get; set; }

        /// <summary>
        /// 同步上传标志 0--未上传 1---已上传
        /// </summary>
        [ExcelColumn(Name = "同步上传标志", Index = 11, Width = 12)]
        public int Receipt_Upload_Flag { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ExcelColumn(Name = "备注", Index = 12, Width = 40)]
        public string? Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ExcelColumn(Name = "创建时间", Index = 13, Width = 40, Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime Create_Time { get; set; }
    }
}
