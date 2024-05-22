using System.ComponentModel;

namespace Intelligent_AutoWms.Common.Enum
{
    public enum TaskModeEnum
    {

        [Description("正常入库")]
        NormalReceipt = 1,
        [Description("正常出库")]
        NormalDelivery = 2,
        [Description("穿越入库")]
        AcrossReceipt = 4,
        [Description("穿越出库")]
        AcrossDelivery = 5,
    }
}
