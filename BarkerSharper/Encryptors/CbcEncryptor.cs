using System.Security.Cryptography;
using BarkerSharper.Data.Encryption;
using BarkerSharper.Exceptions;
using BarkerSharper.Interfaces;

namespace BarkerSharper.Encryptors;

internal class CbcEncryptor : IEncryptor
{
        private readonly CbcStrategy _strategy;

        public CbcEncryptor(CbcStrategy strategy)
        {
            _strategy = strategy;
        }

        public byte[] Encrypt(ReadOnlySpan<byte> data)
        {
            using Aes aes = CreateAes();
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            return Transform(data, encryptor);
        }

        public byte[] Decrypt(ReadOnlySpan<byte> data)
        {
            using Aes aes = CreateAes();
            using ICryptoTransform decryptor = aes.CreateDecryptor();
            return Transform(data, decryptor);
        }

        public async Task<byte[]> EncryptAsync(ReadOnlyMemory<byte> data)
        {
            using Aes aes = CreateAes();
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            return await TransformAsync(data, encryptor);
        }

        public async Task<byte[]> DecryptAsync(ReadOnlyMemory<byte> data)
        {
            using Aes aes = CreateAes();
            using ICryptoTransform decryptor = aes.CreateDecryptor();
            return await TransformAsync(data, decryptor);
        }

        private Aes CreateAes()
        {
            return new AesManaged
            {
                KeySize = _strategy.Key.Length * 8,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                Key = _strategy.Key,
                IV = _strategy.IV ?? throw new InvalidOperationException()
            };
        }

        /*
        [Obsolete]
        private void ValidateKeyAndIV()
        {
            if (_strategy.Key == null || (_strategy.Key.Length * 8) != 128 && (_strategy.Key.Length * 8) != 192 && (_strategy.Key.Length * 8) != 256)
                throw new InvalidKeyException("Invalid key size. Expected 128, 192, or 256 bits.");
            if (_strategy.IV == null || _strategy.IV.Length != 16)
                throw new InvalidIVException("Invalid IV size. Expected 16 bytes (128 bits).");
        }*/

        internal static byte[] Transform(ReadOnlySpan<byte> data, ICryptoTransform transform)
        {
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
            {
                cs.Write(data);
            } // Dispose of CryptoStream to flush the final block

            return ms.ToArray();
        }

        internal static async Task<byte[]> TransformAsync(ReadOnlyMemory<byte> data, ICryptoTransform transform)
        {
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
            {
                await cs.WriteAsync(data);
                await cs.FlushFinalBlockAsync();
            }

            return ms.ToArray();
        }
}