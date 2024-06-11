using System.ComponentModel.DataAnnotations.Schema;

namespace Intelligent_AutoWms.Model.Entities
{
    /// <summary>
    /// 入库单
    /// </summary>
    [Table("WMS_Receipt_Orders")]
    public class WMS_Receipt_Orders
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [Column("order_no")]
        public string Order_No { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        [Column("order_type")]
        public string Order_Type { get; set; }

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
        /// 入口id
        /// </summary>
        [Column("port_id")]
        public long Port_Id { get; set; }

        /// <summary>
        /// 入口编码
        /// </summary>
        [Column("port_code")]
        public string Port_Code { get; set; }

        /// <summary>
        /// 入库时间
        /// </summary>
        [Column("receipt_time")]
        public DateTime? Receipt_Time { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        [Column("upload_time")]
        public DateTime? Upload_Time { get; set; }

        /// <summary>
        /// 入库步骤：1--待入库 2--已入库
        /// </summary>
        [Column("receipt_step")]
        public int Receipt_Step { get; set; }

        /// <summary>
        /// 同步上传标志 0--未上传 1---已上传
        /// </summary>
        [Column("receipt_upload_flag")]
        public int Receipt_Upload_Flag { get; set; }

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
