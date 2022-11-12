namespace KWops.Mobile.Services.Identity;

public interface ILoginResult
{
    string AccessToken { get; }
    string Error { get; }
    string ErrorDescription { get; }
    bool IsError { get; }
}