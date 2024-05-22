using MiniExcelLibs.Attributes;

namespace Intelligent_AutoWms.Model.ImExportTemplate.DeliveryOrder
{
    /// <summary>
    /// 出库单导出模板
    /// </summary>
    public class DeliveryOrderExportTemplate
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
        /// 出口id
        /// </summary>
        [ExcelColumn(Name = "出口ID", Index = 7, Width = 12)]
        public long Port_Id { get; set; }

        /// <summary>
        /// 出口编码
        /// </summary>
        [ExcelColumn(Name = "出口编码", Index = 8, Width = 20)]
        public string Port_Code { get; set; }

        /// <summary>
        /// 出库时间
        /// </summary>
        [ExcelColumn(Name = "出库时间", Index = 9, Width = 40, Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime? Delivery_Time { get; set; }

        /// <summary>
        /// 出库步骤：1--待出库 2--已出库
        /// </summary>
        [ExcelColumn(Name = "出库步骤", Index = 10, Width = 12)]
        public int Delivery_Step { get; set; }

        /// <summary>
        /// 同步上传标志 0--未上传 1---已上传
        /// </summary>
        [ExcelColumn(Name = "同步上传标志", Index = 11, Width = 12)]
        public int Delivery_Upload_Flag { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ExcelColumn(Name = "备注", Index = 12, Width = 40)]
        public string? Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ExcelColumn(Name = "创建时间", Index = 13, Width = 40,  Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime Create_Time { get; set; }
    }
}
