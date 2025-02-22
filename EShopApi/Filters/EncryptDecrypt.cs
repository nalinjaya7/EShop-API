using System.Security.Cryptography;

namespace EShopApi.Filters
{
    public class EncryptDecrypt
    {
        readonly string EncrKey = "jdsg4323";
        readonly string EncryptionKey= "jdsg4323";
        public string Encryptor(string strText)
        { 
            byte[] IV = {22,56,90,124,148,175,210,243};
             byte[] byKey = System.Text.Encoding.UTF8.GetBytes(EncrKey);
            DESCryptoServiceProvider des = new();
            byte[] inputByteArray = System.Text.Encoding.UTF8.GetBytes(strText);
            MemoryStream ms = new();
            CryptoStream cs = new(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        public string DecryptFromBase64String(string stringToDecrypt)
        { 
            byte[] IV = { 22, 56, 90, 124, 148, 175, 210, 243 };
            byte[] byKey = System.Text.Encoding.UTF8.GetBytes(EncryptionKey);
            DESCryptoServiceProvider des = new();
            byte[] inputByteArray = Convert.FromBase64String(stringToDecrypt);
            MemoryStream ms = new();
            CryptoStream cs = new(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }
    }
}