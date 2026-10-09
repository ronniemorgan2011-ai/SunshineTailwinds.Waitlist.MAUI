using SunshineTailwinds.Waitlist.MAUI.Models;
using SunshineTailwinds.Waitlist.MAUI.Services;

namespace SunshineTailwinds.Waitlist.MAUI;

public partial class TableSelectionPage : ContentPage
{
    private readonly DatabaseService _database;
    private readonly Guest _guest;
    private readonly Action _refreshAction;

    public TableSelectionPage(
        Guest guest,
        Action refreshAction)
    {
        InitializeComponent();

        _database = new DatabaseService();

        _guest = guest;

        _refreshAction = refreshAction;

        lblTitle.Text =
            $"Select Table For {_guest.GuestName}";

        LoadTables();
    }

    private void LoadTables()
    {
        TablesContainer.Children.Clear();

        var tables =
            _database.GetTables();

        var guests =
            _database.GetGuests();

        foreach (var table in tables)
        {
            var occupyingGuest =
                guests.FirstOrDefault(g =>
                    g.TableNumber == table.Name &&
                    (g.Status == "Seated" ||
                     g.Status == "Orders In"||
                     g.Status == "Paid"));

            var button = new Button
            {
                Text = table.Name,
                WidthRequest = 75,
                HeightRequest = 55,
                Margin = 5
            };

            if (occupyingGuest != null)
            {
                if (occupyingGuest.Status == "Seated")
                {
                    button.BackgroundColor =
                        Color.FromArgb("#FFE082");
                }
                else if (occupyingGuest.Status == "Orders In")
                {
                    button.BackgroundColor =
                        Color.FromArgb("#F48FB1");
                }
                else if (occupyingGuest.Status == "Paid")
                {
                    button.BackgroundColor =
                        Color.FromArgb("#81C784");
                }
            }

            button.Clicked += async (s, e) =>
            {
                var guests = _database.GetGuests();

                bool occupied =
                    guests.Any(g =>
                        g.Id != _guest.Id &&
                        g.TableNumber == table.Name &&
                        (g.Status == "Seated" ||
                         g.Status == "Orders In"));

                bool activeGuest =
                    _guest.Status == "Seated" ||
                    _guest.Status == "Orders In";

                if (activeGuest && occupied)
                {
                    await DisplayAlert(
                        "Table Occupied",
                        $"Table {table.Name} is currently occupied.",
                        "OK");

                    return;
                }

                _guest.TableNumber = table.Name;

                _database.UpdateGuest(_guest);

                _refreshAction?.Invoke();

                await Navigation.PopModalAsync();
            };

            TablesContainer.Children.Add(button);
        }
    }

    private async void Cancel_Clicked(
        object sender,
        EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}