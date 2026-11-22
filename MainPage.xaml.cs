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

        txtAdults.Completed += (s, e) =>
        {
            txtAdults.Unfocus();
        };

        txtChildren.Completed += (s, e) =>
        {
            txtChildren.Unfocus();
        };

        Dispatcher.StartTimer(
            TimeSpan.FromMinutes(1),
            () =>
            {
                _viewModel.LoadGuests();

                RefreshBindings();

                return true;
            });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.LoadTables();

        SeatingChart.ViewModel = _viewModel;

        SeatingChart.ParentPage = this;
    }


    private void RefreshBindings()
    {
        BindingContext = null;
        BindingContext = _viewModel;

        SeatingChart.RefreshChart();
    }
    private void AddGuest_Clicked(
     object? sender,
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
        SeatingChart.RefreshChart();
    }

    private void ClearSelectedGuest_Clicked(
    object? sender,
    EventArgs e)
    {
        if (BindingContext is MainViewModel vm)
        {
            vm.SelectedGuest = null;
        }
    }

    private async void AddReservation_Clicked(
    object? sender,
    EventArgs e)
    {
        int adults = 0;
        int children = 0;

        int.TryParse(txtAdults.Text, out adults);
        int.TryParse(txtChildren.Text, out children);

        var reservationPage =
            new ReservationPage();

        await Navigation.PushModalAsync(
            reservationPage);

        await reservationPage.WaitForCloseAsync();

        if (!reservationPage.SelectedTime.HasValue)
            return;

        DateTime reservationTime =
            DateTime.Today +
            reservationPage.SelectedTime.Value;

        _viewModel.AddReservation(
            txtGuestName.Text ?? "",
            adults,
            children,
            chkHighChair.IsChecked,
            reservationTime);

        txtGuestName.Text = "";
        txtAdults.Text = "";
        txtChildren.Text = "";

        RefreshBindings();
    }

    private async void AssignTable_Clicked(
        object? sender,
        EventArgs e)
    {
        string result =
            _viewModel.AssignSelectedTable();

        if (!string.IsNullOrWhiteSpace(result))
        {
            await DisplayAlertAsync(
                "Table Assignment",
                result,
                "OK");

            return;
        }

        RefreshBindings();

        SeatingChart.RefreshChart();
    }


    private async void SeatGuest_Clicked(
        object? sender,
        EventArgs e)
    {
        string result =
            _viewModel.SeatSelectedGuest();

        if (!string.IsNullOrWhiteSpace(result))
        {
            await DisplayAlertAsync(
    "DEBUG",
    "SeatGuest_Clicked fired",
    "OK");

            return;
        }

        RefreshBindings();

        SeatingChart.RefreshChart();
    }

    private void OrdersIn_Clicked(
        object? sender,
        EventArgs e)
    {
        _viewModel.OrdersInSelectedGuest();

        RefreshBindings();

        SeatingChart.RefreshChart();
    }

    private void CloseParty_Clicked(
        object? sender,
        EventArgs e)
    {
        _viewModel.CloseSelectedGuest();

        RefreshBindings();

        SeatingChart.RefreshChart();
    }

    private void RemoveGuest_Clicked(
        object? sender,
        EventArgs e)
    {
        _viewModel.RemoveSelectedGuest();

        RefreshBindings();
    }

    private async void ClearGuests_Clicked(
        object? sender,
        EventArgs e)
    {
        bool answer =
        await DisplayAlertAsync(
            "Clear All Guests",
            "Are you sure you want to remove ALL guests from the waitlist?",
            "Yes",
            "No");

        if (!answer)
            return;

        _viewModel.ClearGuests();

        RefreshBindings();

        SeatingChart.RefreshChart();
    }

    public async Task ShowGuestActionMenuAsync(Guest guest)
    {
        _viewModel.SelectedGuest = guest;

        var actionPage = new GuestActionPage();

        await Navigation.PushModalAsync(actionPage);

        await actionPage.WaitForCloseAsync();

        string action = actionPage.SelectedAction;

        switch (action)
        {
            case "🍽️ Seat Guest":

                if (string.IsNullOrWhiteSpace(guest.TableNumber))
                {
                    await Task.Delay(250);

                    await Navigation.PushModalAsync(
                        new TableSelectionPage(
                            guest,
                            RefreshBindings,
                            true));

                    return;
                }

                SeatGuest_Clicked(null, EventArgs.Empty);
                break;

            case "🍳 Orders In":
                OrdersIn_Clicked(null, EventArgs.Empty);
                break;

            case "✅ Close Party":
                CloseParty_Clicked(null, EventArgs.Empty);
                break;

            case "Remove Guest":
                RemoveGuest_Clicked(null, EventArgs.Empty);
                break;

            case "Return To Waiting":
                _viewModel.ReturnGuestToWaiting();
                RefreshBindings();
                SeatingChart.RefreshChart();
                break;

            case "Edit Entry":

                await Task.Delay(250);

                await Navigation.PushModalAsync(
                    new EditGuestPage(
                        guest,
                        RefreshBindings));

                break;

            case "📝 Assign Table":

                await Task.Delay(250);

                await Navigation.PushModalAsync(
                    new TableSelectionPage(
                        guest,
                        RefreshBindings));

                break;

            case "Move Up":
                _viewModel.MoveSelectedGuestUp();
                RefreshBindings();
                break;

            case "Move Down":
                _viewModel.MoveSelectedGuestDown();
                RefreshBindings();
                break;

            case "⚙️ More Options":

                string moreAction =
                    await DisplayActionSheetAsync(
                        "More Options",
                        "Cancel",
                        null,
                        "Return To Waiting",
                        "Edit Entry",
                        "Move Up",
                        "Move Down",
                        "Remove Guest");

                switch (moreAction)
                {
                    case "Return To Waiting":
                        _viewModel.ReturnGuestToWaiting();
                        RefreshBindings();
                        break;

                    case "Edit Entry":

                        await Navigation.PushModalAsync(
                            new EditGuestPage(
                                guest,
                                RefreshBindings));

                        break;

                    case "Move Up":
                        _viewModel.MoveSelectedGuestUp();
                        RefreshBindings();
                        break;

                    case "Move Down":
                        _viewModel.MoveSelectedGuestDown();
                        RefreshBindings();
                        break;

                    case "Remove Guest":
                        _viewModel.RemoveSelectedGuest();
                        RefreshBindings();
                        break;
                }

                break;
        }
    }
    private async void GuestActionMenu_Clicked(
        object? sender,
        EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.BindingContext is not Guest guest)
            return;

        await ShowGuestActionMenuAsync(guest);
    }
     
}