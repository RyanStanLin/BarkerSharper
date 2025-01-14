using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BarkerSharper.Data.Encryption;
using BarkerSharper.Exceptions;
using BarkerSharper.Interfaces;

namespace BarkerSharper.Encryptors;

internal class EcbEncryptor : IEncryptor
{
    private readonly EcbStrategy _strategy;

    public EcbEncryptor(EcbStrategy strategy)
    {
        _strategy = strategy;
        ValidateKey();
    }

    public byte[] Encrypt(ReadOnlySpan<byte> data)
    {
        using Aes aes = CreateAes();
        using ICryptoTransform encryptor = aes.CreateEncryptor();
        return CbcEncryptor.Transform(data, encryptor);
    }

    public byte[] Decrypt(ReadOnlySpan<byte> data)
    {
        using Aes aes = CreateAes();
        using ICryptoTransform decryptor = aes.CreateDecryptor();
        return CbcEncryptor.Transform(data, decryptor);
    }

    public async Task<byte[]> EncryptAsync(ReadOnlyMemory<byte> data)
    {
        using Aes aes = CreateAes();
        using ICryptoTransform encryptor = aes.CreateEncryptor();
        return await CbcEncryptor.TransformAsync(data, encryptor);
    }

    public async Task<byte[]> DecryptAsync(ReadOnlyMemory<byte> data)
    {
        using Aes aes = CreateAes();
        using ICryptoTransform decryptor = aes.CreateDecryptor();
        return await CbcEncryptor.TransformAsync(data, decryptor);
    }

    private Aes CreateAes()
    {
        return new AesManaged
        {
            KeySize = _strategy.Key.Length * 8,
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7,
            Key = _strategy.Key
        };
    }

    private void ValidateKey()
    {
        if (_strategy.Key == null || (_strategy.Key.Length * 8) != 128 && (_strategy.Key.Length * 8) != 192 && (_strategy.Key.Length * 8) != 256)
            throw new InvalidKeyException("Invalid key size. Expected 128, 192, or 256 bits.");
    }
}