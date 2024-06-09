using System.ComponentModel.DataAnnotations.Schema;

namespace Intelligent_AutoWms.Model.Entities
{
    /// <summary>
    /// 任务表
    /// </summary>
    [Table("WMS_Task")]
    public class WMS_Task
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 任务编码
        /// </summary>
        [Column("task_no")]
        public string Task_No { get; set; }

        /// <summary>
        /// 任务编码备用字段
        /// </summary>
        [Column("task_no_backup")]
        public string Task_No_Backup { get; set; }

        /// <summary>
        /// 任务优先级 0-9 0:最优先
        /// </summary>
        [Column("task_priority")]
        public int Task_Priority { get; set; }

        /// <summary>
        /// 任务模式 1:正常入库 2:正常出库 4:穿越入库 5.穿越出库
        /// </summary>
        [Column("task_mode")]
        public int Task_Mode { get; set; }

        /// <summary>
        /// 巷道
        /// </summary>
        [Column("task_lanway")]
        public int Task_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("source_position_x")]
        public int Source_Position_X { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("source_position_y")]
        public int Source_Position_Y { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("source_position_z")]
        public int Source_Position_Z { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("destination_position_x")]
        public int Destination_Position_X { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("destination_position_y")]
        public int Destination_Position_Y { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("destination_position_z")]
        public int Destination_Position_Z { get; set; }

        /// <summary>
        /// 存储位置id
        /// </summary>
        [Column("location_id")]
        public long Location_Id { get; set; }

        /// <summary>
        /// 存储位置编码
        /// </summary>
        [Column("location_code")]
        public string Location_Code { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        [Column("material_code")]
        public string Material_Code { get; set; }

        /// <summary>
        /// 物料类型
        /// </summary>
        [Column("material_type")]
        public int Material_Type { get; set; }

        /// <summary>
        /// 出入口id
        /// </summary>
        [Column("port_id")]
        public long Port_Id { get; set; }

        /// <summary>
        /// 出入口编码
        /// </summary>
        [Column("port_code")]
        public string Port_Code { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [Column("order_no")]
        public string Order_No { get; set; }

        /// <summary>
        /// 执行状态 0：未执行 1：已执行
        /// </summary>
        [Column("task_execute_flag")]
        public int Task_Execute_Flag { get; set; }

        /// <summary>
        /// 任务结束时间
        /// </summary>
        [Column("task_end_time")]
        public DateTime? Task_End_Time { get; set; }

        /// <summary>
        /// 状态 1：正常 2：注销 3.禁用
        /// </summary>
        [Column("status")]
        public int Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column("remark")]
        public string? Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("create_time")]
        public DateTime Create_Time { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Column("creator")]
        public long Creator { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column("update_time")]
        public DateTime? Update_Time { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [Column("updator")]
        public long? Updator { get; set; }
    }
}
