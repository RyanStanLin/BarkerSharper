namespace BarkerSharper.Exceptions;
public class EncryptionException(string message, Exception? innerException = null) : Exception(message, innerException);

public class InvalidKeyException(string message) : EncryptionException(message);

public class InvalidIvException(string message) : EncryptionException(message);