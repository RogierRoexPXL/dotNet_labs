namespace KWops.Mobile.Services.Identity;

public class TokenProvider : ITokenProvider
{
    private const string AccessToken = "access_token";

    public string AuthAccessToken
    {
        get => Preferences.Get(AccessToken, string.Empty);
        set => Preferences.Set(AccessToken, value);
    }
}