namespace BarkerSharper.Interfaces;

public interface IAeadEncryptor : IEncryptor
{
    byte[] Encrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> associatedData);
    byte[] Decrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> associatedData);
    Task<byte[]> EncryptAsync(ReadOnlyMemory<byte> data, ReadOnlyMemory<byte> associatedData);
    Task<byte[]> DecryptAsync(ReadOnlyMemory<byte> data, ReadOnlyMemory<byte> associatedData);
}