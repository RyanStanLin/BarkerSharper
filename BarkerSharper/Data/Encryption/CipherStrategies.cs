using System.Security.Cryptography;
using BarkerSharper.Encryptors;
using BarkerSharper.Exceptions;
using BarkerSharper.Interfaces;

namespace BarkerSharper.Data.Encryption;

public abstract record CipherStrategy(byte[] Key, byte[]? IV = null)
{
    public abstract IEncryptor CreateEncryptor();
}

public record CbcStrategy(byte[] Key, byte[] IV) : CipherStrategy(Key, IV)
{
    public override IEncryptor CreateEncryptor() => new CbcEncryptor(this); // 正确：返回 CbcEncryptor
}

public record EcbStrategy(byte[] Key) : CipherStrategy(Key)
{
    public override IEncryptor CreateEncryptor() => new EcbEncryptor(this); // 正确：返回 EcbEncryptor
}

public record GcmStrategy : CipherStrategy
{
    public override IEncryptor CreateEncryptor() => new GcmEncryptor(this); // 正确：返回 GcmEncryptor

    // 构造函数不需要返回类型，且应该进行 IV 长度验证
    public GcmStrategy(byte[] Key, byte[] IV) : base(Key, IV)
    {
        if (IV == null || IV.Length != AesGcm.NonceByteSizes.MaxSize)
        {
            throw new InvalidIVException("Invalid IV size. Expected 12 bytes (96 bits) for AES-GCM.");
        }
    }
}