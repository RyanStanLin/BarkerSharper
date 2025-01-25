using System.Security.Cryptography;
using System.Text;

namespace BarkerSharper.Data;

public class EncryptionConfiguration
{
    public AesKeySize KeySize { get; init; }
    public AesCipherMode CipherMode { get; init; }
    public required byte[] Key { get; init; }
    public required byte[] IV { get; init; }
    public byte[]? AssociatedData { get; init; }
    public Encoding? Encoding { get; init; } = Encoding.UTF8; // 默认编码
}

public enum AesKeySize
{
    Aes128 = 128,
    Aes192 = 192,
    Aes256 = 256
}

public enum AesCipherMode
{
    CBC = CipherMode.CBC,
    ECB = CipherMode.ECB,
    
    [Obsolete("GCM is not avaliable now.",error:true)]
    GCM = 10 
}