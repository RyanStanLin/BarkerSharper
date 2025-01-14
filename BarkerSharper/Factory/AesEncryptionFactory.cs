using System.Security.Cryptography;
using BarkerSharper.Data;
using BarkerSharper.Data.Encryption;
using BarkerSharper.Encryptors;
using BarkerSharper.Extensions;
using BarkerSharper.Interfaces;


namespace BarkerSharper.Factory;

public static class AesEncryptionFactory
{
    public static IEncryptor CreateEncryptor(CipherStrategy strategy)
    {
        return strategy.CreateEncryptor();
    }

        public static IEncryptor CreateAes128CbcEncryptor(byte[] key, byte[] iv) => CreateEncryptor(new CbcStrategy(key, iv));
        public static IEncryptor CreateAes192CbcEncryptor(byte[] key, byte[] iv) => CreateEncryptor(new CbcStrategy(key, iv));
        public static IEncryptor CreateAes256CbcEncryptor(byte[] key, byte[] iv) => CreateEncryptor(new CbcStrategy(key, iv));
        public static IEncryptor CreateAes128EcbEncryptor(byte[] key) => CreateEncryptor(new EcbStrategy(key));
        public static IEncryptor CreateAes192EcbEncryptor(byte[] key) => CreateEncryptor(new EcbStrategy(key));
        public static IEncryptor CreateAes256EcbEncryptor(byte[] key) => CreateEncryptor(new EcbStrategy(key));
        public static IAeadEncryptor CreateAes128GcmEncryptor(byte[] key, byte[] iv) => (IAeadEncryptor)CreateEncryptor(new GcmStrategy(key, iv));
        public static IAeadEncryptor CreateAes192GcmEncryptor(byte[] key, byte[] iv) => (IAeadEncryptor)CreateEncryptor(new GcmStrategy(key, iv));
        public static IAeadEncryptor CreateAes256GcmEncryptor(byte[] key, byte[] iv) => (IAeadEncryptor)CreateEncryptor(new GcmStrategy(key, iv));

        public static EncryptionReceipt Encrypt(string plainText, EncryptionConfiguration config)
    {
        IEncryptor encryptor = CreateEncryptor(config);
        string encryptedText = encryptor.EncryptToBase64String(plainText, config.Encoding);
        return new EncryptionReceipt(config, encryptedText);
    }

    public static async Task<EncryptionReceipt> EncryptAsync(string plainText, EncryptionConfiguration config)
    {
        IEncryptor encryptor = CreateEncryptor(config);
        string encryptedText = await encryptor.EncryptToBase64StringAsync(plainText, config.Encoding);
        return new EncryptionReceipt(config, encryptedText);
    }

    public static EncryptionReceipt Decrypt(string cipherText, EncryptionConfiguration config)
    {
        IEncryptor encryptor = CreateEncryptor(config);
        string decryptedText = encryptor.DecryptFromBase64String(cipherText, config.Encoding);
        return new EncryptionReceipt(config, decryptedText);
    }

    public static async Task<EncryptionReceipt> DecryptAsync(string cipherText, EncryptionConfiguration config)
    {
        IEncryptor encryptor = CreateEncryptor(config);
        string decryptedText = await encryptor.DecryptFromBase64StringAsync(cipherText, config.Encoding);
        return new EncryptionReceipt(config, decryptedText);
    }

    private static IEncryptor CreateEncryptor(EncryptionConfiguration config)
    {
        switch (config.CipherMode)
        {
            case AesCipherMode.CBC:
                return config.KeySize switch
                {
                    AesKeySize.Aes128 => CreateAes128CbcEncryptor(config.Key, config.IV),
                    AesKeySize.Aes192 => CreateAes192CbcEncryptor(config.Key, config.IV),
                    AesKeySize.Aes256 => CreateAes256CbcEncryptor(config.Key, config.IV),
                    _ => throw new ArgumentOutOfRangeException("Invalid key size for CBC mode.")
                };
            case AesCipherMode.ECB:
                return config.KeySize switch
                {
                    AesKeySize.Aes128 => CreateAes128EcbEncryptor(config.Key),
                    AesKeySize.Aes192 => CreateAes192EcbEncryptor(config.Key),
                    AesKeySize.Aes256 => CreateAes256EcbEncryptor(config.Key),
                    _ => throw new ArgumentOutOfRangeException("Invalid key size for ECB mode.")
                };
            /*
            case AesCipherMode.GCM:
                return config.KeySize switch
                {
                    AesKeySize.Aes128 => CreateAes128GcmEncryptor(config.Key, config.IV),
                    AesKeySize.Aes192 => CreateAes192GcmEncryptor(config.Key, config.IV),
                    AesKeySize.Aes256 => CreateAes256GcmEncryptor(config.Key, config.IV),
                    _ => throw new ArgumentOutOfRangeException("Invalid key size for GCM mode.")
                };*/
            default:
                throw new ArgumentException("Invalid cipher mode.");
        }
    }

        public static byte[] GenerateRandomKey(this IEncryptor encryptor, int keySize)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = keySize;
                aes.GenerateKey();
                return aes.Key;
            }
        }

        public static byte[] GenerateRandomIV(this IEncryptor encryptor)
        {
            // 针对 GCM 模式生成 12 字节的 IV
            if (encryptor is GcmEncryptor)
            {
                var nonce = new byte[AesGcm.NonceByteSizes.MaxSize]; // 12 bytes
                RandomNumberGenerator.Fill(nonce);
                return nonce;
            }
            else // 其他模式保持 16 字节
            {
                using (Aes aes = Aes.Create())
                {
                    aes.GenerateIV();
                    return aes.IV;
                }
            }
        }
}