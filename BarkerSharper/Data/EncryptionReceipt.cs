namespace BarkerSharper.Data;

public class EncryptionReceipt
{
    public EncryptionConfiguration Config { get; init; }
    public string Result { get; init; }

    public EncryptionReceipt(EncryptionConfiguration config, string result)
    {
        Config = config;
        Result = result;
    }
}