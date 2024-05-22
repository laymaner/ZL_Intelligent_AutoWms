using System.Security.Cryptography;
using System.Text;

namespace Intelligent_AutoWms.Common.Utils
{
    public class MD5EncryptionUtil
    {

        /// <summary>
        /// AES加密解密 密钥及初始化向量  在实际的生产环境中，最好使用更安全的方式来管理密钥和向量，例如使用Azure Key Vault、AWS KMS或其他安全服务。
        /// </summary>
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("SidTek_CipherSTC"); // 16字节密钥
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("SidTek_VectorSTC");  // 16字节初始化向量

        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMd5Hash16(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < 4; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMd5Hash32(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

        /// <summary>
        /// 64位MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMd5Hash64(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(data);
            }
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            using var streamWriter = new StreamWriter(cryptoStream);
            streamWriter.Write(plainText);
            streamWriter.Flush();
            cryptoStream.FlushFinalBlock();

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        /// <summary>
        ///  AES解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            byte[] bytes = Convert.FromBase64String(cipherText);

            using var memoryStream = new MemoryStream(bytes);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);

            return streamReader.ReadToEnd();
        }
    }
}
