using KWops.Mobile.Models;
using System.Collections.ObjectModel;
using KWops.Mobile.Services;
using KWops.Mobile.Services.Backend;

namespace KWops.Mobile.ViewModels;

public class TeamsViewModel : BaseViewModel
{
    private readonly ITeamsService _teamsService;
    private readonly IToastService _toastService;
    private readonly INavigationService _navigationService;

    public ObservableCollection<Team> Items { get; }
    public Command LoadTeamsCommand { get; }
    public Command<Team> ItemTapped { get; }

    public TeamsViewModel(ITeamsService teamsService,
        IToastService toastService,
        INavigationService navigationService)
    {
        _teamsService = teamsService;
        _toastService = toastService;
        _navigationService = navigationService;
        Title = "Team Overview";
        Items = new ObservableCollection<Team>();
        LoadTeamsCommand = new Command(async () => await ExecuteLoadTeamsCommand());

        ItemTapped = new Command<Team>(OnTeamSelected);
    }

    async Task ExecuteLoadTeamsCommand()
    {
        IsBusy = true;

        try
        {
            Items.Clear();
            IReadOnlyList<Team> allTeams = await _teamsService.GetAllTeamsAsync();
            foreach (var team in allTeams)
            {
                Items.Add(team);
            }
        }
        catch (Exception ex)
        {
            await _toastService.DisplayToastAsync(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void OnAppearing()
    {
        IsBusy = true;
    }

    async void OnTeamSelected(Team team)
    {
        if (team == null)
        {
            return;
        }

        await _navigationService.NavigateRelativeAsync("TeamDetailPage");
        MessagingCenter.Send(this, "TeamSelected", team);
    }
}