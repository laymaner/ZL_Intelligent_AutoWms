using MiniExcelLibs.Attributes;

namespace Intelligent_AutoWms.Model.ImExportTemplate.Inventory
{
    /// <summary>
    /// 库存明细导出模板
    /// </summary>
    public class InventoryExportTemplate
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [ExcelColumn(Name = "ID", Index = 0, Width = 12)]
        public long Id { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        [ExcelColumn(Name = "物料编码", Index = 1, Width = 20)]
        public string Material_Code { get; set; }

        /// <summary>
        /// 物料类型
        /// </summary>
        [ExcelColumn(Name = "物料类型", Index = 2, Width = 20)]
        public int Material_Type { get; set; }


        /// <summary>
        /// 存储位置ID
        /// </summary>
        [ExcelColumn(Name = " 存储位置ID", Index = 3, Width = 12)]
        public long Location_Id { get; set; }

        /// <summary>
        /// 存储位置编码
        /// </summary>
        [ExcelColumn(Name = "存储位置编码", Index = 4, Width = 20)]
        public string Location_Code { get; set; }


        /// <summary>
        /// 入口ID
        /// </summary>
        [ExcelColumn(Name = "入口ID", Index = 5, Width = 12)]
        public string Port_Id { get; set; }

        /// <summary>
        /// 入口编码
        /// </summary>
        [ExcelColumn(Name = "入口编码", Index = 6, Width = 20)]
        public string Port_Code { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [ExcelColumn(Name = "订单编号", Index = 7, Width = 30)]
        public string Order_No { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        [ExcelColumn(Name = "订单类型", Index = 8, Width = 12)]
        public string Order_Type { get; set; }

        /// <summary>
        /// Y/N 是否锁定
        /// </summary>
        [ExcelColumn(Name = "是否锁定", Index = 9, Width = 12)]
        public string Is_Lock { get; set; }

        /// <summary>
        /// 状态 1：正常 2：注销 3.禁用
        /// </summary>
        [ExcelColumn(Name = "状态", Index = 10, Width = 12)]
        public int Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ExcelColumn(Name = "备注", Index = 11, Width = 40)]
        public string? Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ExcelColumn(Name = "创建时间", Index = 12, Width = 40, Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime Create_Time { get; set; }

    }
}
