namespace Intelligent_AutoWms.Model.RequestDTO.Port
{
    /// <summary>
    /// 创建或更新出入口实体参数
    /// </summary>
    public class CreateOrUpdatePortDTO
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 出入口名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 出入口编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 类型 1：入口 2：出口
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 仓库id
        /// </summary>
        public long? Warehouse_Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int First_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Second_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Third_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Forth_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Fifth_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Sixth_Lanway { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }


    }
}
