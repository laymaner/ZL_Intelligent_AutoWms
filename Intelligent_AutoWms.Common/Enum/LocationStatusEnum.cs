using System.ComponentModel;

namespace Intelligent_AutoWms.Common.Enum
{
    public enum LocationStatusEnum
    {
        [Description("空闲中")]
        Idle = 1,
        [Description("入库锁定")]
        Warehousing_Lock = 2,
        [Description("出库锁定")]
        Outbound_Lock = 3,
        [Description("占用中")]
        Occupying = 4,
    }
}
