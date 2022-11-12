namespace KWops.Mobile.Exceptions;

public class BackendHttpException : HttpRequestException
{
    public System.Net.HttpStatusCode HttpCode { get; }

    public BackendHttpException(System.Net.HttpStatusCode code) : this(code, null, null)
    {
    }

    public BackendHttpException(System.Net.HttpStatusCode code, string message) : this(code, message, null)
    {
    }

    public BackendHttpException(System.Net.HttpStatusCode code, string message, Exception inner) : base(message, inner)
    {
        HttpCode = code;
    }
}