using System.ComponentModel.DataAnnotations.Schema;

namespace Intelligent_AutoWms.Model.Entities
{
    /// <summary>
    /// 操作日志
    /// </summary>
    [Table("WMS_Operate_Log")]
    public class WMS_Operate_Log
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        [Column("user_code")]
        public string? User_Code { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [Column("user_name")]
        public string? User_Name { get; set; }

        /// <summary>
        /// ip地址
        /// </summary>
        [Column("ip_address")]
        public string Ip_Address { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Column("title")]
        public string? Title { get; set; }

        /// <summary>
        /// 方法名称
        /// </summary>
        [Column("method_name")]
        public string Method_Name { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        [Column("operate_type")]
        public string Operate_Type { get; set; }

        /// <summary>
        /// 操作路径
        /// </summary>
        [Column("operate_url")]
        public string Operate_Url { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        [Column("operate_params")]
        public string? Operate_Params { get; set; }

        /// <summary>
        /// 操作状态
        /// </summary>
        [Column("operate_status")]
        public int Operate_Status { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        [Column("error_msg")]
        public string? Error_Msg { get; set; }

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
        public DateTime? Create_Time { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Column("creator")]
        public long? Creator { get; set; }

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
