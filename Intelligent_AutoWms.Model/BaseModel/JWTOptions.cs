namespace Intelligent_AutoWms.Model.BaseModel
{
    public class JWTOptions
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string SigningKey { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public long ExpireSeconds { get; set; }
    }
}
