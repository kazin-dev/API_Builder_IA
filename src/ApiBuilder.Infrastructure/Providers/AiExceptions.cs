namespace ApiBuilder.Infrastructure;

public class AiRateLimitException : Exception
{
    public AiRateLimitException(string message) : base(message) { }
}

public class AiProviderException : Exception
{
    public AiProviderException(string message) : base(message) { }
}
