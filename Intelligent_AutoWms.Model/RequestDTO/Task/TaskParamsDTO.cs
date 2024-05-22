using Intelligent_AutoWms.Model.BaseModel;

namespace Intelligent_AutoWms.Model.RequestDTO.Task
{
    /// <summary>
    /// 查询任务实体参数
    /// </summary>
    public class TaskParamsDTO : BasicQuery
    {
        /// <summary>
        /// 任务编码
        /// </summary>
        public string? Task_No { get; set; }

        /// <summary>
        /// 任务模式
        /// </summary>
        public int Task_Mode { get; set; }


        /// <summary>
        /// 物料编码
        /// </summary>
        public string? Material_Code { get; set; }

        /// <summary>
        /// 物料类型
        /// </summary>
        public int? Material_Type { get; set; }

        /// <summary>
        /// 存储位置编码
        /// </summary>
        public string? Location_Code { get; set; }

        /// <summary>
        /// 出口编码
        /// </summary>
        public string? Port_Code { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string? Order_No { get; set; }

        /// <summary>
        /// 状态 1：正常 2：删除 3.禁用
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 任务结束时间开始
        /// </summary>
        public DateTime? Task_Time_Strat { get; set; }

        /// <summary>
        /// 任务结束时间结束
        /// </summary>
        public DateTime? Task_Time_End { get; set; }

        /// <summary>
        /// 创建时间开始
        /// </summary>
        public DateTime? Create_Time_Strat { get; set; }

        /// <summary>
        /// 创建时间结束
        /// </summary>
        public DateTime? Create_Time_End { get; set; }


    }
}
