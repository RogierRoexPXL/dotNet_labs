namespace KWops.Mobile.Settings;

public interface IAppSettings
{
    string OidcAuthority { get; }
    string OidcClientId { get; }
    string OidcClientSecret { get; }
    string OidcScope { get; }
    string OidcRedirectUri { get; }

    string DevOpsBackendBaseUrl { get; }
}