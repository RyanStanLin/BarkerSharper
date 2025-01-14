![BarkerSharperTitle](https://github.com/user-attachments/assets/d0531c58-3e76-4799-a337-35d5befa5308)
# BarkerSharper

[BarkerSharper] 是 [Bark](https://github.com/finb/bark) 在.net中的api封装库

## 速览

- 适配最新的 [Bark] 服务端。
- 实现了 [Bark] 的 `AES128,192,256加密` 和 `批量发送`
  - `Http Adapter` 用来进行发送操作。
  - `Websocket Adapter` 用来进行接收操作。
- [bark-server](https://github.com/Finb/bark-server) 作为推送系统。
- 支持Async调用。
- 提供了简单的调用方式

## 安装

- 使用 Nuget 安装(推荐)
  - Nuget 包管理器:
    ```Install-Package BarkerSharper```
  - .NET CLI:
    ```dotnet add package BarkerSharper```
  - **或者在 IDE 的可视化界面搜索`BarkerSharper`安装最新版。**
- 自己克隆这个仓库的默认分支，然后自己编译，然后自己添加 dll 引用。

## 快速开始

`Barker`类构造函数接受一个`BarkConfiguration`配置参数: 
- 必填字段为:
	- **`baseUrl`** : `Uri` Bark服务端地址，如`https://api.day.app`
    - **`deviceKey`** : `string` 或 **`deviceKeys`** : `List<string>` 以实现批量发送
- 选填字段为:
	- **`encryptionConfig`** : `EncryptionConfiguration` AES加密配置

实例化 `Barker` 后使用 `Bark` 或 `BarkAsync` 来同步或异步发送信息。 

```cs
public BarkReceipt Bark<T>(T notificationModel) where T : BarkNotificationBaseModel
 in class BarkerSharper.Barker
```

你可以选择 `BarkNotificationExtendedModel` 或 `BarkNotificationBaseModel`（基础模型） 消息类实例化来建立信息。

`BarkNotificationExtendedModel` 为派生类，支持添加其他消息属性。

下面的例子就是使用AES256 CBC加密,向两台设备 `WHkMpzrC4…` 和 `whKmPZRc5…` 发送

```cs
//若批量发送，否则不需要传List，具体请看BarkConfiguration的实例化
List<string> devices = new();
        devices.Add("WHkMpzrC4mTnuagU49Cvnm");
        devices.Add("whKmPZRc5MtNUAGu47cVNM");

//若使用AES加密，密钥及偏移量
byte[] key = Encoding.UTF8.GetBytes("10101010101010101010101010101010");
byte[] iv = Encoding.UTF8.GetBytes("0000000000000000");

//若使用AES加密，请实例化AES加密配置EncryptionConfiguration
var encryptionConfiguration = new EncryptionConfiguration
{
    KeySize = AesKeySize.Aes256,	//加密方式Aes128，Aes192，Aes256
    CipherMode = AesCipherMode.CBC,//目前仅支持CBC及ECB
    Key = key,
    IV = iv,
    Encoding = Encoding.UTF8
};

var barkerConfig = new BarkConfiguration(
	baseUrl: new Uri("https://api.day.app"),
	deviceKeys: devices,						//单设备请赋值deviceKey参数

	encryptionConfig: encryptionConfiguration);//可选参数，若传加密配置，则默认信息加密

Barker barker = new Barker(barkerConfig);

//使用基础模型，仅可用Title和Body
barker.Bark(new BarkNotificationBaseModel()
{
	Body = "Hello, Body!",
	Title = "Hello, BarkerSharper!"
});

//使用拓展模型，支持所有参数
barker.Bark(new BarkNotificationExtendedModel()
{
	Body = "Hello, BarkerSharper!",
	Title = "Hi!",
	Level = NotificationLevel.Critical,		//Level参数具体请看Bark文档，NotificationLevel已实现
	Sound = NotificationSound.PaymentSuccess,//可用Sound具体请看Bark文档，NotificationSound已实现
	Group = "HelloGroup"
});
```
