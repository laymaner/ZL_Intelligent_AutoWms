using System.ComponentModel;

namespace Intelligent_AutoWms.Common.Enum
{
    /// <summary>
    /// 出库单状态
    /// </summary>
    public enum DeliveryOrderStatusEnum
    {
        [Description("待出库")]
        WaitingForOutbound = 1,
        [Description("已出库")]
        Outbound = 2,
    }
}
