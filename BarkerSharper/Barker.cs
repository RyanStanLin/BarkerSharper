using System.Net;
using System.Text;
using BarkerSharper.Data;
using BarkerSharper.Model;
using BarkerSharper.Extensions;
using BarkerSharper.Factory;
using RestSharp;

namespace BarkerSharper;

/// <summary>
/// 封装了Bark消息推送服务的客户端类。
/// </summary>
/// <param name="barkConfiguration">Bark服务的配置信息。</param>
/// <exception cref="ArgumentNullException">当 barkConfiguration 为 null 时抛出。</exception>
public class Barker
{
    private readonly BarkConfiguration _barkConfiguration;

    public Barker(BarkConfiguration barkConfiguration)
    {
        if (barkConfiguration is null)
            throw new ArgumentNullException(nameof(_barkConfiguration));
        _barkConfiguration = barkConfiguration;
    }

    /// <summary>
    /// 同步发送通知。
    /// </summary>
    /// <typeparam name="T">通知消息的模型类型，必须继承自 BarkNotificationBaseModel。</typeparam>
    /// <param name="notificationModel">要发送的通知消息模型。</param>
    /// <returns>返回发送结果的回执信息 <see cref="BarkReceipt"/>。</returns>
    /// <exception cref="ArgumentNullException">当 notificationModel 为 null 时抛出。</exception>
    /// <exception cref="InvalidOperationException">当请求失败或响应内容为空时抛出。</exception>
    public BarkReceipt Bark<T>(T notificationModel) where T : BarkNotificationBaseModel
    {
        if(_barkConfiguration.IsBatch)
            notificationModel.DeviceKeys = _barkConfiguration.DeviceKeys;
        else
            notificationModel.DeviceKey = _barkConfiguration.DeviceKey;

        var request = _barkConfiguration.IsEncryptMessage ? BuildRequest(notificationModel, _barkConfiguration.EncryptionConfig!) : BuildRequest(notificationModel);
            
        var client = new RestClient(_barkConfiguration.BaseUrl);
        var response = client.Execute(request);

        return ParseResponse(response);
    }

    /// <summary>
    /// 异步发送通知。
    /// </summary>
    /// <typeparam name="T">通知消息的模型类型，必须继承自 BarkNotificationBaseModel。</typeparam>
    /// <param name="notificationModel">要发送的通知消息模型。</param>
    /// <returns>返回一个 Task，其 Result 属性为发送结果的回执信息 <see cref="BarkReceipt"/>。</returns>
    /// <exception cref="ArgumentNullException">当 notificationModel 为 null 时抛出。</exception>
    /// <exception cref="InvalidOperationException">当请求失败或响应内容为空时抛出。</exception>
    public async Task<BarkReceipt> BarkAsync<T>(T notificationModel) where T : BarkNotificationBaseModel
    {
        if(_barkConfiguration.IsBatch)
            notificationModel.DeviceKeys = _barkConfiguration.DeviceKeys;
        else
            notificationModel.DeviceKey = _barkConfiguration.DeviceKey;
        
        var request = _barkConfiguration.IsEncryptMessage ? BuildRequest(notificationModel, _barkConfiguration.EncryptionConfig!) : BuildRequest(notificationModel);
        
        var client = new RestClient(_barkConfiguration.BaseUrl);
        var response = await client.ExecuteAsync(request);

        return ParseResponse(response);
    }

    /// <summary>
    /// 构建发送通知的 RestRequest 对象。
    /// </summary>
    /// <typeparam name="T">通知消息的模型类型，必须继承自 BarkNotificationBaseModel。</typeparam>
    /// <param name="notificationModel">要发送的通知消息模型。</param>
    /// <returns>返回构建好的 RestRequest 对象。</returns>
    /// <exception cref="ArgumentNullException">当 notificationModel 为 null 时抛出。</exception>
    /// <exception cref="InvalidOperationException">当 notificationModel 验证失败时抛出。</exception>
    private RestRequest BuildRequest<T>(T notificationModel) where T : BarkNotificationBaseModel
    {
        if (notificationModel == null) throw new ArgumentNullException(nameof(notificationModel));

        notificationModel.Validate();

        var request = new RestRequest("push", Method.Post)
            .AddHeader(Constant.JSONKEYWORD_CONTENTTYPE,
                $"{Constant.JSONKEYWORD_RESPONSE_TYPEJSON}; {Constant.JSONKEYWORD_RESPONSE_CHARSET_UTF8}");

        var payload = notificationModel.ToJson();

        request.AddBody(payload);

        return request;
    }
    
    /// <summary>
    /// 构建发送加密的通知的 RestRequest 对象。
    /// </summary>
    /// <typeparam name="T">通知消息的模型类型，必须继承自 BarkNotificationBaseModel。</typeparam>
    /// <param name="notificationModel">要发送的通知消息模型。</param>
    /// <param name="encryptionConfiguration">加密配置</param>
    /// <returns>返回构建好的 RestRequest 对象。</returns>
    /// <exception cref="ArgumentNullException">当 notificationModel 为 null 时抛出。</exception>
    /// <exception cref="InvalidOperationException">当 notificationModel 验证失败时抛出。</exception>
    private RestRequest BuildRequest<T>(T notificationModel, EncryptionConfiguration encryptionConfiguration) where T : BarkNotificationBaseModel
    {
        if (notificationModel == null) throw new ArgumentNullException(nameof(notificationModel));

        notificationModel.Validate();

        var request = new RestRequest(notificationModel.DeviceKey, Method.Post);
        
        var payload = notificationModel.ToJson();
        EncryptionReceipt encryptionReceipt = AesEncryptionFactory.Encrypt(payload, encryptionConfiguration);

        request.AddParameter(Constant.ENCRYPT_CHIPETEXT_PROPERTY_KEY, encryptionReceipt.Result, ParameterType.GetOrPost);
        request.AddParameter(Constant.ENCRYPT_IV_PROPERTY_KEY, Encoding.UTF8.GetString(encryptionReceipt.Config.IV), ParameterType.GetOrPost);

        return request;
    }

    /// <summary>
    /// 解析 Bark 服务返回的响应。
    /// </summary>
    /// <param name="response">Bark 服务返回的 RestResponse 对象。</param>
    /// <returns>返回解析后的发送结果回执信息 <see cref="BarkReceipt"/>。</returns>
    /// <exception cref="InvalidOperationException">当请求失败或响应内容为空时抛出。</exception>
    private BarkReceipt ParseResponse(RestResponse response)
    {
        /*if (response.StatusCode != HttpStatusCode.OK)
            throw new InvalidOperationException(
                $"Request failed with status code: {response.StatusCode}. Content: {response.Content}");*/

        if (string.IsNullOrWhiteSpace(response.Content))
            throw new InvalidOperationException("Response content is empty.");

        var responseBody = BarkNotificationResponseModel.FromJson(response.Content);

        if (responseBody.Message == Constant.RESPONSE_SUCCESS_TEXT)
            return new BarkReceipt(isSucceed: true, timeStamp: responseBody.Timestamp.ToLocalDateTime());
        else
            return new BarkReceipt(isSucceed: false);
    }
}