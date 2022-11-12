using IdentityModel.OidcClient;

namespace KWops.Mobile.Services.Identity;

public class IdentityModelLoginResult : ILoginResult
{
    private readonly LoginResult _result;

    public IdentityModelLoginResult(LoginResult result)
    {
        _result = result;
    }

    public IdentityModelLoginResult(string error, string errorDescription)
    {
        _result = new LoginResult(error, errorDescription);
    }

    public bool IsError => _result.IsError;
    public string AccessToken => _result.AccessToken;
    public string Error => _result.Error;
    public string ErrorDescription => _result.ErrorDescription;
}