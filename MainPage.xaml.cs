using SunshineTailwinds.Waitlist.MAUI.ViewModels;

namespace SunshineTailwinds.Waitlist.MAUI;

public partial class MainPage : ContentPage
{
    private MainViewModel _viewModel;

    public MainPage()
    {
        InitializeComponent();

        _viewModel = new MainViewModel();

        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.LoadTables();
    }

    private void RefreshBindings()
    {
        BindingContext = null;
        BindingContext = _viewModel;
    }

    private void AddGuest_Clicked(
        object sender,
        EventArgs e)
    {
        int adults = 0;
        int children = 0;

        int.TryParse(txtAdults.Text, out adults);
        int.TryParse(txtChildren.Text, out children);

        _viewModel.AddGuest(
            txtGuestName.Text ?? "",
            adults,
            children,
            chkHighChair.IsChecked);

        txtGuestName.Text = "";
        txtAdults.Text = "";
        txtChildren.Text = "";

        RefreshBindings();
    }

    private async void AssignTable_Clicked(
    object sender,
    EventArgs e)
    {
        string result =
            _viewModel.AssignSelectedTable();

        if (!string.IsNullOrWhiteSpace(result))
        {
            await DisplayAlert(
                "Table Assignment",
                result,
                "OK");

            return;
        }

        RefreshBindings();
    }

    private void SeatGuest_Clicked(
        object sender,
        EventArgs e)
    {
        _viewModel.SeatSelectedGuest();

        RefreshBindings();
    }

    private void OrdersIn_Clicked(
        object sender,
        EventArgs e)
    {
        _viewModel.OrdersInSelectedGuest();

        RefreshBindings();
    }

    private void CloseParty_Clicked(
        object sender,
        EventArgs e)
    {
        _viewModel.CloseSelectedGuest();

        RefreshBindings();
    }

    private void RemoveGuest_Clicked(
        object sender,
        EventArgs e)
    {
        _viewModel.RemoveSelectedGuest();

        RefreshBindings();
    }
}