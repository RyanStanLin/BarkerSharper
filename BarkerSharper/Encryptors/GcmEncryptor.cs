using System.Security.Cryptography;
using BarkerSharper.Data.Encryption;
using BarkerSharper.Exceptions;
using BarkerSharper.Interfaces;

namespace BarkerSharper.Encryptors;


internal class GcmEncryptor : IAeadEncryptor
{
        private readonly GcmStrategy _strategy;

        public GcmEncryptor(GcmStrategy strategy)
        {
            _strategy = strategy;
            ValidateKeyAndIV();
        }

        public byte[] Encrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> associatedData)
        {
            using var aesGcm = new AesGcm(_strategy.Key);
            var nonce = _strategy.IV;
            var tag = new byte[AesGcm.TagByteSizes.MaxSize];
            var ciphertext = new byte[data.Length];

            aesGcm.Encrypt(nonce, data, ciphertext, tag, associatedData);

            return ConcatenateArrays(nonce, ciphertext, tag);
        }

        public byte[] Decrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> associatedData)
        {
            var nonce = data[..AesGcm.NonceByteSizes.MaxSize];
            var tag = data.Slice(data.Length - AesGcm.TagByteSizes.MaxSize);
            var ciphertext = data.Slice(AesGcm.NonceByteSizes.MaxSize, data.Length - AesGcm.NonceByteSizes.MaxSize - AesGcm.TagByteSizes.MaxSize);

            using var aesGcm = new AesGcm(_strategy.Key);
            var decryptedData = new byte[ciphertext.Length];

            aesGcm.Decrypt(nonce, ciphertext, tag, decryptedData, associatedData);

            return decryptedData;
        }

        public async Task<byte[]> EncryptAsync(ReadOnlyMemory<byte> data, ReadOnlyMemory<byte> associatedData)
        {
            return await Task.Run(() => Encrypt(data.Span, associatedData.Span));
        }

        public async Task<byte[]> DecryptAsync(ReadOnlyMemory<byte> data, ReadOnlyMemory<byte> associatedData)
        {
            return await Task.Run(() => Decrypt(data.Span, associatedData.Span));
        }

        private static byte[] ConcatenateArrays(ReadOnlySpan<byte> first, ReadOnlySpan<byte> second, ReadOnlySpan<byte> third)
        {
            var result = new byte[first.Length + second.Length + third.Length];
            first.CopyTo(result);
            second.CopyTo(result.AsSpan(first.Length));
            third.CopyTo(result.AsSpan(first.Length + second.Length));
            return result;
        }

        private void ValidateKeyAndIV()
        {
            if (_strategy.Key == null || (_strategy.Key.Length * 8) != 128 && (_strategy.Key.Length * 8) != 192 && (_strategy.Key.Length * 8) != 256)
                throw new InvalidKeyException("Invalid key size. Expected 128, 192, or 256 bits.");
            if (_strategy.IV == null || _strategy.IV.Length != AesGcm.NonceByteSizes.MaxSize)
                throw new InvalidIVException("Invalid IV size. Expected 12 bytes (96 bits) for AES-GCM.");
        }
        
        public byte[] Encrypt(ReadOnlySpan<byte> data)
        {
            throw new NotSupportedException("AES-GCM requires associated data. Use the Encrypt method that takes associated data.");
        }

        public byte[] Decrypt(ReadOnlySpan<byte> data)
        {
            throw new NotSupportedException("AES-GCM requires associated data. Use the Decrypt method that takes associated data.");
        }

        public async Task<byte[]> EncryptAsync(ReadOnlyMemory<byte> data)
        {
            return await Task.FromException<byte[]>(new NotSupportedException("AES-GCM requires associated data. Use the EncryptAsync method that takes associated data."));
        }

        public async Task<byte[]> DecryptAsync(ReadOnlyMemory<byte> data)
        {
            return await Task.FromException<byte[]>(new NotSupportedException("AES-GCM requires associated data. Use the DecryptAsync method that takes associated data."));
        }
}