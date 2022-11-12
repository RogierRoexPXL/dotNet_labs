using KWops.Mobile.ViewModels;

namespace KWops.Mobile.Views;

public partial class TeamsPage : ContentPage
{
    private TeamsViewModel _viewModel;
    public TeamsPage(TeamsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        Routing.RegisterRoute("TeamDetailPage", typeof(TeamDetailPage));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();

    }
}