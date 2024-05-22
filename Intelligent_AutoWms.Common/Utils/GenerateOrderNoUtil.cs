namespace Intelligent_AutoWms.Common.Utils
{
    public class GenerateOrderNoUtil
    {
        private static object obj = new object();

        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="mark">前缀</param>
        /// <param name="timeType">时间精确类型  1 日,2 时,3 分，4 秒,5毫秒(默认) </param>
        /// <returns></returns>
        public static string Gener(string mark, int timeType = 5)
        {
            lock (obj)
            {
                var number = mark;

                number += GetTimeStr(timeType);

                return number;
            }
        }

        /// <summary>
        /// 获取时间字符串
        /// </summary>
        /// <param name="timeType">时间精确类型  1 日,2 时,3 分，4 秒(默认)</param>
        /// <returns></returns>
        private static string GetTimeStr(int timeType)
        {
            var time = DateTime.Now;
            if (timeType == 1)
            {
                return time.ToString("yyyyMMdd");
            }
            else if (timeType == 2)
            {
                return time.ToString("yyyyMMddHH");
            }
            else if (timeType == 3)
            {
                return time.ToString("yyyyMMddHHmm");
            }
            else if (timeType == 4)
            { 
                return time.ToString("yyyyMMddHHmmss");
            }
            else
            {
                return time.ToString("yyyyMMddHHmmssffff");
            }
        }
    }
}
