namespace BarkerSharper.Exceptions;
public class EncryptionException : Exception
{
    public EncryptionException(string message, Exception innerException = null) : base(message, innerException) { }
}

public class InvalidKeyException : EncryptionException
{
    public InvalidKeyException(string message) : base(message) { }
}

public class InvalidIVException : EncryptionException
{
    public InvalidIVException(string message) : base(message) { }
}