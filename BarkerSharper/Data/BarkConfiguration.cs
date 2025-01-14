namespace BarkerSharper.Data;

/// <summary>
/// Bark 配置类，用于管理通知服务的基本配置，包括根 URL 和设备密钥。
/// </summary>
public class BarkConfiguration
{
    /// <summary>
    /// Bark 服务的根 URL，必须为绝对 URL。
    /// </summary>
    public Uri BaseUrl { get; init; }

    /// <summary>
    /// 用于设备身份验证的密钥
    /// </summary>
    public List<string> DeviceKeys { get; init; }
    public string DeviceKey { get; init; }
    
    internal readonly bool IsBatch;
    internal readonly bool IsEncryptMessage;
    
    public EncryptionConfiguration? EncryptionConfig { get; init; }

    /// <summary>
    /// 创建一个新的 Bark 配置实例。
    /// </summary>
    /// <param name="baseUrl">Bark 服务的根 URL。</param>
    /// <param name="deviceKeys">设备密钥，用于身份验证。</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="baseUrl"/> 或 <paramref name="deviceKeys"/> 为 null 时或 <paramref name="deviceKeys"/> 不包含任何项抛出。</exception>
    /// <exception cref="ArgumentException">当 <paramref name="baseUrl"/> 不是绝对 URL，或 <paramref name="deviceKeys"/> 为空字符串时抛出。</exception>
    public BarkConfiguration(Uri baseUrl, List<string> deviceKeys, EncryptionConfiguration? encryptionConfig = null)
    {
        IsBatch = true;
        BaseUrl = ValidateBaseUrl(baseUrl);
        DeviceKeys = ValidateDeviceKeys(deviceKeys);
        if (encryptionConfig is not null)
        {
            EncryptionConfig = encryptionConfig;
            IsEncryptMessage = true;
        }
    }
    
    public BarkConfiguration(Uri baseUrl, string deviceKey, EncryptionConfiguration? encryptionConfig = null)
    {
        IsBatch = false;
        BaseUrl = ValidateBaseUrl(baseUrl);
        DeviceKey = ValidateDeviceKey(deviceKey);
        if (encryptionConfig is not null)
        {
            EncryptionConfig = encryptionConfig;
            IsEncryptMessage = true;
        }
    }
    
    private static Uri ValidateBaseUrl(Uri baseUrl)
    {
        if (baseUrl is null)
            throw new ArgumentNullException(nameof(baseUrl), "BaseUrl 不能为空。");

        if (!baseUrl.IsAbsoluteUri)
            throw new ArgumentException("BaseUrl 必须是绝对 URL。", nameof(baseUrl));

        return baseUrl;
    }

    private static List<string> ValidateDeviceKeys(List<string> deviceKeys)
    {
        if (deviceKeys is null || deviceKeys.Any() is not true)
            throw new ArgumentNullException(nameof(deviceKeys), "DeviceKeys 不能为空。");

        foreach (var key in deviceKeys)
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("DeviceKeys 不能是空字符串或仅包含空白字符。", nameof(deviceKeys));

        return deviceKeys;
    }
    
    private static string ValidateDeviceKey(string deviceKey)
    {
        if (deviceKey is null || deviceKey.Any() is not true)
            throw new ArgumentNullException(nameof(deviceKey), "DeviceKey 不能为空。");
        
        if (string.IsNullOrWhiteSpace(deviceKey))
            throw new ArgumentException("DeviceKey 不能是空字符串或仅包含空白字符。", nameof(deviceKey));

        return deviceKey;
    }
}