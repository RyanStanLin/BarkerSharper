namespace BarkerSharper.Interfaces;

public interface IEncryptor
{
    byte[] Encrypt(ReadOnlySpan<byte> data);
    byte[] Decrypt(ReadOnlySpan<byte> data);
    Task<byte[]> EncryptAsync(ReadOnlyMemory<byte> data);
    Task<byte[]> DecryptAsync(ReadOnlyMemory<byte> data);
}