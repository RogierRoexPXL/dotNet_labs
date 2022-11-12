using KWops.Mobile.Exceptions;
using KWops.Mobile.Models;
using KWops.Mobile.Services.Identity;
using KWops.Mobile.Settings;

namespace KWops.Mobile.Services.Backend;

public class TeamsService : ITeamsService
{
    private readonly IBackendService _backend;
    private readonly IAppSettings _appSettings;
    private readonly ITokenProvider _tokenProvider;
    private readonly INavigationService _navigationService;

    public TeamsService(IBackendService backend,
        IAppSettings appSettings,
        ITokenProvider tokenProvider,
        INavigationService navigationService)
    {
        _backend = backend;
        _appSettings = appSettings;
        _tokenProvider = tokenProvider;
        _navigationService = navigationService;
    }

    public async Task<IReadOnlyList<Team>> GetAllTeamsAsync()
    {
        try
        {
            List<Team> teams = await _backend.GetAsync<List<Team>>($"{_appSettings.DevOpsBackendBaseUrl}/teams", _tokenProvider.AuthAccessToken);
            return teams.OrderBy(t => t.Name).ToList();
        }
        catch (BackendAuthenticationException)
        {
            await _navigationService.NavigateAsync("LoginPage");
            return new List<Team>();
        }
    }
}