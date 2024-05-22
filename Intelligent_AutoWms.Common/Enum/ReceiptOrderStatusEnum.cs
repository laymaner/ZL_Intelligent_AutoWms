using System.ComponentModel;

namespace Intelligent_AutoWms.Common.Enum
{
    /// <summary>
    /// 入库单状态
    /// </summary>
    public enum ReceiptOrderStatusEnum
    {
        [Description("待入库")]
        WaitingForStorage = 1,
        [Description("已入库")]
        Received = 2,

    }
}
