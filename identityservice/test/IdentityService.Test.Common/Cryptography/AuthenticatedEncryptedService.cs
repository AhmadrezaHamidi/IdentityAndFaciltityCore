using System.Security.Cryptography;
using System.Text;

namespace IdentityService.Test.Common.Cryptography;

public class AuthenticatedEncryptionService
{
    private readonly byte[] _encryptionKey;

    public AuthenticatedEncryptionService(string base64Key)
    {
        _encryptionKey = Convert.FromBase64String(base64Key);
    }

    public static string GenerateKey()
    {
        var key = RandomNumberGenerator.GetBytes(32); // 256-bit key
        return Convert.ToBase64String(key);
    }

    public string Encrypt(string plaintext)
    {
        var nonce = RandomNumberGenerator.GetBytes(12);
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[16];

        using var aesGcm = new AesGcm(_encryptionKey);

        aesGcm.Encrypt(
            nonce,
            plaintextBytes,
            ciphertext,
            tag
        );

        var result = new OmidBank.Common.Helper.AesGcmCiphertext(nonce, tag, ciphertext);
        return result.ToString();
    }

    public string Decrypt(string base64Ciphertext)
    {
        var cipherTextObj = OmidBank.Common.Helper.AesGcmCiphertext.FromBase64String(base64Ciphertext);

        var plaintextBytes = new byte[cipherTextObj.CiphertextBytes.Length];

        using var aesGcm = new AesGcm(_encryptionKey);

        aesGcm.Decrypt(
            cipherTextObj.Nonce,
            cipherTextObj.CiphertextBytes,
            cipherTextObj.Tag,
            plaintextBytes
        );

        return Encoding.UTF8.GetString(plaintextBytes);
    }
}