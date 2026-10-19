using System.Security.Cryptography;
using System.Text;
using Interface_IEncryption = IdentityService.Test.Common.Interface.IEncryption;

namespace IdentityService.Test.Common.Cryptography
{
    public class Encryption : Interface_IEncryption
    {
        // // encryption methods
        // [Obsolete("Obsolete")]
        // public string? Encrypt(object model, byte[] keyAes)
        // {
        //     try
        //     {
        //         var nonce = RandomNumberGenerator.GetBytes(12);
        //         var tag = new byte[16];
        //         using var aesGcm = new AesGcm(keyAes);
        //         var jsonDataModel = JsonSerializer.Serialize(model, JsonHelper.JsonCamelCaseOption());
        //         var plaintextBytes = Encoding.UTF8.GetBytes(jsonDataModel);
        //         var ciphertext = new byte[plaintextBytes.Length];
        //         aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);
        //         var result = new AesGcmCipherText(nonce, tag, ciphertext);
        //         return result.ToString();
        //     }
        //     catch (Exception)
        //     {
        //         return null;
        //     }
        // }
        //
        // public CryptoSession<T>? Decrypt<T>(string encryptedData, string encryptedKey)
        // {
        //     try
        //     {
        //         var cryptoSession = new CryptoSession<T>();
        //         var decryptKey = DecryptRsa(encryptedKey);
        //         var keyAesOr = Convert.FromBase64String(decryptKey);
        //         var dataBy = Convert.FromBase64String(encryptedData);
        //         var textJson = Aes.Decrypt(dataBy, keyAesOr);
        //         var jsonModel = Encoding.UTF8.GetString(textJson);
        //         cryptoSession.Key = decryptKey;
        //         cryptoSession.Value = JsonSerializer.Deserialize<T>(jsonModel, JsonHelper.JsonCamelCaseOption())!;
        //         return cryptoSession;
        //     }
        //     catch (Exception)
        //     {
        //         return null;
        //     }
        // }

        public static string DecryptRsa(string encryptedKey)
        {
            var path = $"{AppContext.BaseDirectory}/pemFile/PrivateKey.pem";
            var loadedRsa = File.ReadAllText(path);
            var privateRsaKey = NormalizeRsaKeyFromPem(loadedRsa)!;
            var result = Encoding.UTF8.GetString(privateRsaKey.Decrypt(Convert.FromBase64String(encryptedKey), false));
            return result;
        }

        public static byte[] EncryptRsa(byte[] key)
        {
            var path = $"{AppContext.BaseDirectory}/pemFile/PrivateKey.pem";
            var loadedRsa = File.ReadAllText(path);
            var privateRsaKey = NormalizeRsaKeyFromPem(loadedRsa)!;
            var result = privateRsaKey.Encrypt(key, false);
            return result;
        }

        private static RSACryptoServiceProvider? NormalizeRsaKeyFromPem(string? pem)
        {
            RSACryptoServiceProvider? rsa = null;
            const string keyHeader = "-----BEGIN RSA PRIVATE KEY-----";
            const string keyFooter = "-----END RSA PRIVATE KEY-----";

            if (!IsPrivateKeyAvailable(pem)) 
                return rsa;
            
            var keyFormatted = pem;
            if (string.IsNullOrEmpty(keyFormatted))
                return null;
            
            var cutIndex = keyFormatted.IndexOf(keyHeader, StringComparison.Ordinal);
            keyFormatted = keyFormatted.Substring(cutIndex, keyFormatted.Length - cutIndex);
            cutIndex = keyFormatted.IndexOf(keyFooter, StringComparison.Ordinal);
            keyFormatted = keyFormatted.Substring(0, cutIndex + keyFooter.Length);
            keyFormatted = keyFormatted.Replace(keyHeader, "");
            keyFormatted = keyFormatted.Replace(keyFooter, "");
            keyFormatted = keyFormatted.Replace("\r", "");
            keyFormatted = keyFormatted.Replace("\n", "");
            keyFormatted = keyFormatted.Trim();

            var privateKeyInDer = Convert.FromBase64String(keyFormatted);

            rsa = DecodeRsaPrivateKey(privateKeyInDer);

            return rsa;
        }

        private static bool IsPrivateKeyAvailable(string? privateKeyInPem)
        {
            const string keyHeader = "-----BEGIN RSA PRIVATE KEY-----";
            const string keyFooter = "-----END RSA PRIVATE KEY-----";

            return privateKeyInPem is not null && privateKeyInPem.Contains(keyHeader)
                && privateKeyInPem.Contains(keyFooter);
        }

        private static RSACryptoServiceProvider? DecodeRsaPrivateKey(byte[] privateKey)
        {

            // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
            var mem = new MemoryStream(privateKey);
            var binaryReader = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            try
            {
                var twobytes = binaryReader.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binaryReader.ReadByte();        //advance 1 byte
                else if (twobytes == 0x8230)
                    binaryReader.ReadInt16();       //advance 2 bytes
                else
                    return null;

                twobytes = binaryReader.ReadUInt16();
                if (twobytes != 0x0102) //version number
                    return null;
                var bt = binaryReader.ReadByte();
                if (bt != 0x00)
                    return null;


                //------  all private key components are Integer sequences ----
                var elems = GetIntegerSize(binaryReader);
                var modulus = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                var e = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                var d = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                var p = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                var q = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                var dp = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                var dq = binaryReader.ReadBytes(elems);

                elems = GetIntegerSize(binaryReader);
                var iq = binaryReader.ReadBytes(elems);

                // ------- create RSACryptoServiceProvider instance and initialize with private key -----
                var rsa = new RSACryptoServiceProvider();
                var rsaParams = new RSAParameters
                {
                    Modulus = modulus,
                    Exponent = e,
                    D = d,
                    P = p,
                    Q = q,
                    DP = dp,
                    DQ = dq,
                    InverseQ = iq
                };
                rsa.ImportParameters(rsaParams);
                return rsa;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                binaryReader.Close();
            }
        }

        private static int GetIntegerSize(BinaryReader binary)
        {
            int count;
            var bt = binary.ReadByte();
            if (bt != 0x02)
                return 0;

            bt = binary.ReadByte();
            switch (bt)
            {
                case 0x81:
                    count = binary.ReadByte();
                    break;
                case 0x82:
                {
                    var highByte = binary.ReadByte();
                    var lowByte = binary.ReadByte();
                    byte[] modInt = { lowByte, highByte, 0x00, 0x00 };
                    count = BitConverter.ToInt32(modInt, 0);
                    break;
                }
                default:
                    count = bt;
                    break;
            }

            while (binary.ReadByte() == 0x00)
                count -= 1;

            binary.BaseStream.Seek(-1, SeekOrigin.Current);

            return count;
        }

    }
}