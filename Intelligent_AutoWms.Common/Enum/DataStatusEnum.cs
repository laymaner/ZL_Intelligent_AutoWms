using System.ComponentModel;

namespace Intelligent_AutoWms.Common.Enum
{
    public enum DataStatusEnum
    {
        [Description("正常")]
        Normal = 1,
        [Description("删除")]
        Delete = 2,
        [Description("禁用")]
        Disabled = 3,
        [Description("结束")]
        Finished = 4,
    }
}
