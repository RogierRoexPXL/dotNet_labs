using System;
using System.Threading.Tasks;
using IdentityModel.OidcClient;
using KWops.Mobile.Settings;

namespace KWops.Mobile.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly IAppSettings _appSettings;

    public IdentityService(IAppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public async Task<ILoginResult> LoginAsync()
    {
        var options = new OidcClientOptions
        {
            Authority = _appSettings.OidcAuthority,
            ClientId = _appSettings.OidcClientId,
            ClientSecret = _appSettings.OidcClientSecret,
            Scope = _appSettings.OidcScope,
            RedirectUri = _appSettings.OidcRedirectUri,
            Browser = new WebAuthenticatorBrowser()
        };

        var client = new OidcClient(options);
#if DEBUG
        client.Options.Policy.Discovery.RequireHttps = false;
#endif
        try
        {
            LoginResult result = await client.LoginAsync(new LoginRequest());
            return new IdentityModelLoginResult(result);

        }
        catch (Exception e)
        {
            return new IdentityModelLoginResult("Unexpected error", e.Message);
        }
    }
}