using System.Text;

namespace BarkerSharper.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// 将字符串转换为16进制表示
    /// </summary>
    /// <param name="input">待转换的字符串</param>
    /// <param name="encoding">指定编码（默认UTF-8）</param>
    /// <returns>转换后的16进制字符串</returns>
    public static string ToHex(this string input, Encoding? encoding = null)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));
        encoding ??= Encoding.UTF8;

        byte[] bytes = encoding.GetBytes(input);
        StringBuilder hexBuilder = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            hexBuilder.AppendFormat("{0:X2}", b); // 转为两位大写16进制
        }
        return hexBuilder.ToString();
    }

    /// <summary>
    /// 从16进制字符串还原原始字符串
    /// </summary>
    /// <param name="hex">16进制字符串</param>
    /// <param name="encoding">指定编码（默认UTF-8）</param>
    /// <returns>还原后的字符串</returns>
    public static string FromHex(this string hex, Encoding? encoding = null)
    {
        if (string.IsNullOrEmpty(hex)) throw new ArgumentException("Hex string cannot be null or empty", nameof(hex));
        if (hex.Length % 2 != 0) throw new FormatException("Hex string must have an even length.");
        encoding ??= Encoding.UTF8;

        byte[] bytes = new byte[hex.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }

        return encoding.GetString(bytes);
    }
}
