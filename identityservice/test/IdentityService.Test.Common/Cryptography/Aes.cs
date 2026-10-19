using System.Security.Cryptography;

namespace IdentityService.Test.Common.Cryptography
{
    public class Aes
    {
        public static RijndaelManaged GetRijndaelManaged(byte[] secretKey)
        {
            var keyBytes = new byte[secretKey.Length];
            Array.Copy(secretKey, keyBytes, Math.Min(keyBytes.Length, secretKey.Length));
            byte[] ivArr = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 256,
                BlockSize = 128,
                Key = keyBytes,
                IV = ivArr
            };
        }

        private static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        private static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        /// <summary>
        /// Encrypts plaintext using AES 128bit key and a Chain Block Cipher and returns a base64 encoded string
        /// </summary>
        /// <param name="plainText">Plain text to encrypt</param>
        /// <param name="key">Secret key</param>
        /// <returns>Base64 encoded string</returns>
        public static byte[] Encrypt(byte[] plainText, byte[] key)
        {
            //var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Encrypt(plainText, GetRijndaelManaged(key));
        }

        /// <summary>
        /// Decrypts a base64 encoded string using the given key (AES 128bit key and a Chain Block Cipher)
        /// </summary>
        /// <param name="encryptedText">Base64 Encoded String</param>
        /// <param name="key">Secret Key</param>
        /// <returns>Decrypted String</returns>
        public static byte[] Decrypt(byte[] encryptedText, byte[] key)
        {
            return Decrypt(encryptedText, GetRijndaelManaged(key));
        }
    }
}
