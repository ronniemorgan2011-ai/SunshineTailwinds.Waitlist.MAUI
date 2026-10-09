using SunshineTailwinds.Waitlist.MAUI.Models;
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

    private async void SeatGuest_Clicked(
        object sender,
        EventArgs e)
    {
        string result =
            _viewModel.SeatSelectedGuest();

        if (!string.IsNullOrWhiteSpace(result))
        {
            await DisplayAlert(
                "Seat Guest",
                result,
                "OK");

            return;
        }

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

    private async void ClearGuests_Clicked(
        object sender,
        EventArgs e)
    {
        bool answer =
            await DisplayAlert(
                "Clear All Guests",
                "Are you sure you want to remove ALL guests from the waitlist?",
                "Yes",
                "No");

        if (!answer)
            return;

        _viewModel.ClearGuests();
    }

    private async void GuestActionMenu_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.BindingContext is not Guest guest)
            return;

        _viewModel.SelectedGuest = guest;

        string action =
            await DisplayActionSheet(
                $"What would you like to do with {guest.GuestName}?",
                "Cancel",
                null,
                "Pre-Seat Guest",
                "Seat Guest",
                "Orders In",
                "Paid",
                "Close Party",
                "Remove Guest");

        switch (action)
        {
            case "Seat Guest":
                SeatGuest_Clicked(sender, EventArgs.Empty);
                break;

            case "Orders In":
                OrdersIn_Clicked(sender, EventArgs.Empty);
                break;

            case "Close Party":
                CloseParty_Clicked(sender, EventArgs.Empty);
                break;

            case "Remove Guest":
                RemoveGuest_Clicked(sender, EventArgs.Empty);
                break;

            case "Pre-Seat Guest":

                await Navigation.PushModalAsync(
                    new TableSelectionPage(
                        guest,
                        RefreshBindings));
                break;

            case "Paid":
                _viewModel.MarkGuestPaid();
                RefreshBindings();
                break;
        }
    }
}