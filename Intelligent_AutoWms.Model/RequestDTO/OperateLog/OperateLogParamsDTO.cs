using Intelligent_AutoWms.Model.BaseModel;

namespace Intelligent_AutoWms.Model.RequestDTO.OperateLog
{
    /// <summary>
    /// 查询操作日志参数实体
    /// </summary>
    public class OperateLogParamsDTO:BasicQuery
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string? User_Code { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string? User_Name { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        public string? Ip_Address { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string? Method_Name { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string? Operate_Type { get; set; }

        /// <summary>
        /// 操作状态
        /// </summary>
        public int? Operate_Status { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? Start_Time { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? End_Time { get; set; }
    }
}
