using SunshineTailwinds.Waitlist.MAUI.ViewModels;

namespace SunshineTailwinds.Waitlist.MAUI;

public partial class DashboardPage : ContentPage
{
    private MainViewModel _viewModel;

    public DashboardPage()
    {
        InitializeComponent();

        _viewModel = new MainViewModel();

        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.LoadGuests();
    }
}