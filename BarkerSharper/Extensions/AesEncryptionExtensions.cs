using System;
using System.Text;
using System.Threading.Tasks;
using BarkerSharper.Interfaces;

namespace BarkerSharper.Extensions;

public static class AesEncryptionExtensions
{
    public static string EncryptToBase64String(this IEncryptor encryptor, string plainText, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var plainBytes = encoding.GetBytes(plainText);
        var encryptedBytes = encryptor.Encrypt(plainBytes);
        return Convert.ToBase64String(encryptedBytes);
    }

    public static string DecryptFromBase64String(this IEncryptor encryptor, string cipherText, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var cipherBytes = Convert.FromBase64String(cipherText);
        var decryptedBytes = encryptor.Decrypt(cipherBytes);
        return encoding.GetString(decryptedBytes);
    }

    public static async Task<string> EncryptToBase64StringAsync(this IEncryptor encryptor, string plainText, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var plainBytes = encoding.GetBytes(plainText);
        var encryptedBytes = await encryptor.EncryptAsync(plainBytes);
        return Convert.ToBase64String(encryptedBytes);
    }

    public static async Task<string> DecryptFromBase64StringAsync(this IEncryptor encryptor, string cipherText, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var cipherBytes = Convert.FromBase64String(cipherText);
        var decryptedBytes = await encryptor.DecryptAsync(cipherBytes);
        return encoding.GetString(decryptedBytes);
    }

    // GCM 模式的扩展方法
    public static string EncryptToBase64String(this IAeadEncryptor encryptor, string plainText, string associatedData, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var plainBytes = encoding.GetBytes(plainText);
        var associatedBytes = encoding.GetBytes(associatedData);
        var encryptedBytes = encryptor.Encrypt(plainBytes, associatedBytes);
        return Convert.ToBase64String(encryptedBytes);
    }

    public static string DecryptFromBase64String(this IAeadEncryptor encryptor, string cipherText, string associatedData, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var cipherBytes = Convert.FromBase64String(cipherText);
        var associatedBytes = encoding.GetBytes(associatedData);
        var decryptedBytes = encryptor.Decrypt(cipherBytes, associatedBytes);
        return encoding.GetString(decryptedBytes);
    }

    public static async Task<string> EncryptToBase64StringAsync(this IAeadEncryptor encryptor, string plainText, string associatedData, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var plainBytes = encoding.GetBytes(plainText);
        var associatedBytes = encoding.GetBytes(associatedData);
        var encryptedBytes = await encryptor.EncryptAsync(plainBytes, associatedBytes);
        return Convert.ToBase64String(encryptedBytes);
    }

    public static async Task<string> DecryptFromBase64StringAsync(this IAeadEncryptor encryptor, string cipherText, string associatedData, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var cipherBytes = Convert.FromBase64String(cipherText);
        var associatedBytes = encoding.GetBytes(associatedData);
        var decryptedBytes = await encryptor.DecryptAsync(cipherBytes, associatedBytes);
        return encoding.GetString(decryptedBytes);
    }
}