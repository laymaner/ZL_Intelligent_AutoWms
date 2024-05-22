using MiniExcelLibs.Attributes;

namespace Intelligent_AutoWms.Model.ImExportTemplate.Task
{
    public class TaskExportTemplate
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [ExcelColumn(Name = "ID", Index = 0, Width = 12)]
        public long Id { get; set; }

        /// <summary>
        /// 任务编码
        /// </summary>
        [ExcelColumn(Name = "任务编码", Index = 1, Width = 20)]
        public string Task_No { get; set; }

        /// <summary>
        /// 任务编码备用字段
        /// </summary>
        [ExcelColumn(Name = "任务编码备用字段", Index = 2, Width = 20)]
        public string Task_No_Backup { get; set; }

        /// <summary>
        /// 任务优先级 0-9 0:最优先
        /// </summary>
        [ExcelColumn(Name = "任务优先级", Index = 3, Width = 12)]
        public int Task_Priority { get; set; }

        /// <summary>
        /// 任务模式 1:入库 2:出库 3:移库
        /// </summary>
        [ExcelColumn(Name = "任务模式", Index = 4, Width = 12)]
        public int Task_Mode { get; set; }

        /// <summary>
        /// 巷道
        /// </summary>
        [ExcelColumn(Name = "巷道", Index = 5, Width = 12)]
        public int Task_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ExcelColumn(Name = "Source_Position_X", Index = 6, Width = 12)]
        public int Source_Position_X { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ExcelColumn(Name = "Source_Position_Y", Index = 7, Width = 12)]
        public int Source_Position_Y { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ExcelColumn(Name = "Source_Position_Z", Index = 8, Width = 12)]
        public int Source_Position_Z { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ExcelColumn(Name = "Destination_Position_X", Index = 9, Width = 12)]
        public int Destination_Position_X { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ExcelColumn(Name = "Destination_Position_Y", Index = 10, Width = 12)]
        public int Destination_Position_Y { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ExcelColumn(Name = "Destination_Position_Z", Index = 11, Width = 12)]
        public int Destination_Position_Z { get; set; }

        /// <summary>
        /// 存储位置id
        /// </summary>
        [ExcelColumn(Name = "存储位置ID", Index = 12, Width = 12)]
        public long Location_Id { get; set; }

        /// <summary>
        /// 存储位置编码
        /// </summary>
        [ExcelColumn(Name = "存储位置编码", Index = 13, Width = 30)]
        public string Location_Code { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        [ExcelColumn(Name = "物料编码", Index = 14, Width = 30)]
        public string Material_Code { get; set; }

        /// <summary>
        /// 物料类型
        /// </summary>
        [ExcelColumn(Name = "物料类型", Index = 15, Width = 12)]
        public int Material_Type { get; set; }

        /// <summary>
        /// 出入口id
        /// </summary>
        [ExcelColumn(Name = "出入口ID", Index = 16, Width = 12)]
        public long Port_Id { get; set; }

        /// <summary>
        /// 出入口编码
        /// </summary>
        [ExcelColumn(Name = "出入口编码", Index = 17, Width = 20)]
        public string Port_Code { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [ExcelColumn(Name = "订单编号", Index = 18, Width = 30)]
        public string Order_No { get; set; }

        /// <summary>
        /// 执行状态 0：未执行 1：已执行
        /// </summary>
        [ExcelColumn(Name = "执行状态", Index = 19, Width = 12)]
        public int Task_Execute_Flag { get; set; }

        /// <summary>
        /// 发送状态 0：未发送 1：已发送
        /// </summary>
        [ExcelColumn(Name = "发送状态", Index = 20, Width = 12)]
        public int Task_Send_Flag { get; set; }

        /// <summary>
        /// 任务结束时间
        /// </summary>
        [ExcelColumn(Name = "任务结束时间", Index = 21, Width = 40, Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime? Task_End_Time { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ExcelColumn(Name = "备注", Index = 22, Width = 50)]
        public string? Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ExcelColumn(Name = "创建时间", Index = 23, Width = 40, Format = "yyyy-MM-dd HH:mm:ss")]
        public DateTime Create_Time { get; set; }

    }
}
