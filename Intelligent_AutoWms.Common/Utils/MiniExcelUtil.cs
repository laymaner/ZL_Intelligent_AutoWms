using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;

namespace Intelligent_AutoWms.Common.Utils
{
    /// <summary>
    /// 使用miniExcel导入导出
    /// </summary>
    public static class MiniExcelUtil
    {
        public static IEnumerable<T> Import<T>(string filePath) where T : class, new()
        {
            using (var stream = File.OpenRead(filePath))
            {
                return stream.Query<T>().ToList();
            }
        }

        public async static Task<FileStreamResult> ExportAsync<T>(string FileName, IEnumerable<T> data)
        {
            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(data);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = FileName + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx"
            };
        }
    }
}
