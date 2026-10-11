using SunshineTailwinds.Waitlist.MAUI.Models;
using SunshineTailwinds.Waitlist.MAUI.Services;

namespace SunshineTailwinds.Waitlist.MAUI;

public partial class EditGuestPage : ContentPage
{
    private readonly Guest _guest;
    private readonly DatabaseService _database;
    private readonly Action _refreshAction;

    public EditGuestPage(
        Guest guest,
        Action refreshAction)
    {
        InitializeComponent();

        _guest = guest;

        _refreshAction = refreshAction;

        _database = new DatabaseService();

        txtGuestName.Text = guest.GuestName;
        txtAdults.Text = guest.Adults.ToString();
        txtChildren.Text = guest.Children.ToString();
        chkHighChair.IsChecked = guest.HighChairRequired;
    }

    private async void Save_Clicked(
        object sender,
        EventArgs e)
    {
        int adults = 0;
        int children = 0;

        int.TryParse(txtAdults.Text, out adults);
        int.TryParse(txtChildren.Text, out children);

        _guest.GuestName = txtGuestName.Text ?? "";
        _guest.Adults = adults;
        _guest.Children = children;
        _guest.HighChairRequired = chkHighChair.IsChecked;

        _database.UpdateGuest(_guest);

        _refreshAction?.Invoke();

        await Navigation.PopModalAsync();
    }

    private async void Cancel_Clicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}