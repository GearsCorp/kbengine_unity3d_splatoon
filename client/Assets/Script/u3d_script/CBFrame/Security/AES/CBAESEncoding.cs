using System.Collections;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System;

namespace CBFrame.Security
{
    public class AESEncoding
    {
        private const string randomChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQ" +
            "RSTUVWXYZ0123456789~!@#$%^&*()_+|-=,./[]{}:;':";
        private static string GenerateKey()
        {
            string password = string.Empty;
            int randomNum;
            System.Random random = new System.Random();
            for (int i = 0; i < 16; i++)
            {
                randomNum = random.Next(randomChars.Length);
                password += randomChars[randomNum];
            }
            return password;
        }

        public static byte[] Encrypt(byte[] content, string key, string iv)
        {
            RijndaelManaged rDel = new RijndaelManaged();

            rDel.BlockSize = 128;
            rDel.KeySize = 256;
            rDel.FeedbackSize = 128;
            rDel.Padding = PaddingMode.PKCS7;
            rDel.Mode = CipherMode.CBC;

            rDel.Key = UTF8Encoding.UTF8.GetBytes(key);
            rDel.IV = UTF8Encoding.UTF8.GetBytes(iv);

            ICryptoTransform transform = rDel.CreateEncryptor();
            return transform.TransformFinalBlock(content, 0, content.Length);
        }

        public static byte[] Decrypt(byte[] content, string key, string iv)
        {
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.BlockSize = 128;
            rDel.KeySize = 256;
            rDel.FeedbackSize = 128;
            rDel.Padding = PaddingMode.PKCS7;
            rDel.Mode = CipherMode.CBC;

            rDel.Key = UTF8Encoding.UTF8.GetBytes(key);
            rDel.IV = UTF8Encoding.UTF8.GetBytes(iv);

            ICryptoTransform transform = rDel.CreateDecryptor();
            return transform.TransformFinalBlock(content, 0, content.Length);
        }
    }
}