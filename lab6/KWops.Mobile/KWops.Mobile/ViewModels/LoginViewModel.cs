using KWops.Mobile.Services;
using KWops.Mobile.Services.Identity;
using System.Windows.Input;

namespace KWops.Mobile.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenProvider _tokenProvider;
        private readonly INavigationService _navigationService;
        private readonly IToastService _toastService;
        public ICommand LoginCommand { get; set; }

        public LoginViewModel(IIdentityService identityService, ITokenProvider tokenProvider, INavigationService navigationService, IToastService toastService)
        {
            _identityService = identityService;
            _tokenProvider = tokenProvider;
            _navigationService = navigationService;
            _toastService = toastService;

            LoginCommand = new Command(
                execute: async () =>
                {
                    if (!IsBusy)
                    {                        
                        IsBusy = true;
                        var loginResult = await _identityService.LoginAsync();
                        if (loginResult.AccessToken == null)
                        {
                            await _toastService.DisplayToastAsync(loginResult.ErrorDescription);
                        } 
                        else
                        {
                            _tokenProvider.AuthAccessToken = loginResult.AccessToken;
                            await _navigationService.NavigateAsync("TeamsPage");
                        }
                    }
                    IsBusy = false;
                },
                canExecute: () =>
                {                    
                    return !IsBusy;
                });

        }
    }
}
