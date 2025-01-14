namespace BarkerSharper.Data;

public class BarkReceipt
{
    public BarkReceipt(bool isSucceed, DateTime? timeStamp = null)
    {
        IsSucceed = isSucceed;

        if (isSucceed && timeStamp is null)
        {
            throw new ArgumentNullException(nameof(timeStamp), "Succeed but with null timestamp.");
        }
    }

    public bool IsSucceed { get; set; }
}