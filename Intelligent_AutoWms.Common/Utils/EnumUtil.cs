using System.ComponentModel;
using System.Reflection;

namespace Intelligent_AutoWms.Common.Utils
{
    public class EnumUtil
    {
        /// <summary>
        /// 获取当前枚举描述
        /// EnumHelper.GetEnumDescription(Enum.enumValue)
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription<T>(T enumValue)
        {
            Type type = enumValue.GetType();
            MemberInfo[] memInfo = type.GetMember(enumValue.ToString());
            if (null != memInfo && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (null != attrs && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return enumValue.ToString();
        }

        /// 根据 Description 的值获取枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T GetEnumByDescription<T>(string description)
        {
            FieldInfo[] fields = typeof(T).GetFields();
            foreach (FieldInfo field in fields)
            {
                object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (objs.Length > 0 && (objs[0] as DescriptionAttribute).Description == description)
                {
                    return (T)field.GetValue(null);
                }
            }
            return default(T);
        }
    }
}
