using System.Security.Cryptography;
using System.Text;

namespace NetCoreIoT.Common
{
    public static class AESHelper
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plainText">要加密的数据</param>
        /// <param name="key">key，128、192或256位密钥 使用16位密码</param>
        /// <param name="iv">iv，128、192或256位密钥 使用16位密码</param>
        /// <returns></returns>
        public static string Encrypt(string plainText, string key, string iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText, string key, string iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        //测试范例
        //static void Main()
        //{
        //    // 要加密的数据
        //    string originalText = "Hello, AES Encryption!";

        //    // 128、192或256位密钥
        //    string key = "0123456789abcdef";
        //    string iv = "0123456789abcdef";

        //    // 加密
        //    string encryptedText = Encrypt(originalText, key, iv);
        //    Console.WriteLine($"Encrypted Text: {encryptedText}");

        //    // 解密
        //    string decryptedText = Decrypt(encryptedText, key, iv);
        //    Console.WriteLine($"Decrypted Text: {decryptedText}");
        //}
    }
}