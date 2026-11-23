using SunshineTailwinds.Waitlist.MAUI.ViewModels;
using SunshineTailwinds.Waitlist.MAUI.Models;
using SunshineTailwinds.Waitlist.MAUI.Services;

namespace SunshineTailwinds.Waitlist.MAUI.Views;

public partial class SeatingChartView : ContentView
{
    private readonly DatabaseService _database = new();
    public MainViewModel? ViewModel { get; set; }
    public MainPage? ParentPage { get; set; }

    private const double DesignWidth = 850;
    private const double DesignHeight = 850;

    public SeatingChartView()
    {
        InitializeComponent();

        SizeChanged += SeatingChartView_SizeChanged;
    }

    private void SeatingChartView_SizeChanged(
    object? sender,
    EventArgs e)
    {
        RefreshChart();
    }
    public void RefreshChart()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ChartLayout.Children.Clear();

            AddDemoTables();
        });
    }

    private void AddDemoTables()
    {
        // Upper right
        CreateTable("15", 520, 40);
        CreateTable("14", 620, 40);
        CreateTable("23", 720, 40);

        // 16 / 13
        CreateTable("16B", 520, 130);
        CreateTable("16A", 520, 195);

        CreateTable("13B", 720, 130);
        CreateTable("13A", 720, 195);

        // 10 / 11 / 12
        CreateTable("10B", 520, 300);
        CreateTable("10A", 520, 365);

        CreateTable("12", 720, 300);
        CreateTable("11", 720, 365);

        // 9A / 9B
        CreateTable("9A", 555, 435);
        CreateTable("9B", 645, 435);

        // Bottom row
        CreateTable("5A", 350, 510);
        CreateTable("5B", 450, 510);
        CreateTable("6", 550, 510);
        CreateTable("7", 650, 510);
        CreateTable("8", 750, 510);

        // Top row
        CreateTable("22", 0, 180);
        CreateTable("21", 110, 180);
        CreateTable("20", 220, 180);
        CreateTable("19", 320, 180);

        // CTR Stack
        CreateTable("CTR24", 0, 300);
        CreateTable("CTR25", 0, 370);
        CreateTable("CTR26", 0, 440);
        CreateTable("CTR27", 0, 510);

        // 1-4 Stack
        CreateTable("4", 220, 300);
        CreateTable("3", 220, 370);
        CreateTable("2", 220, 440);
        CreateTable("1", 220, 510);

    }

    private void CreateTable(
    string tableNumber,
    double x,
    double y)
    {
        double scale = Math.Min(
    ChartLayout.Width / DesignWidth,
    ChartLayout.Height / DesignHeight);

        if (scale <= 0)
        {
            scale = Width / DesignWidth;

            if (scale <= 0)
                scale = 1;
        }
        scale = Math.Min(scale, 1.0);
        scale *= 1.0;
        System.Diagnostics.Debug.WriteLine(
        $"Scale = {scale}, Width = {ChartLayout.Width}, Height = {ChartLayout.Height}");
        var guests = _database.GetAllGuests();

        var guestAtTable =
            guests.FirstOrDefault(g =>
                !string.IsNullOrWhiteSpace(g.TableNumber) &&
                g.TableNumber.Trim().Equals(
                    tableNumber,
                    StringComparison.OrdinalIgnoreCase));

        Color tableColor = Colors.LightGray;

        if (guestAtTable != null)
        {
            tableColor = guestAtTable.Status switch
            {
                "Reserved" => Color.FromArgb("#B3E5FC"),
                "Seated" => Color.FromArgb("#FFE082"),
                "Orders In" => Color.FromArgb("#F48FB1"),
                _ => Colors.LightGray
            };
        }

        Button button = new()
        {
            Text = tableNumber,
            WidthRequest = 85 * scale,
            HeightRequest = 60 * scale,
            BackgroundColor = tableColor
        };

        button.Clicked += Table_Clicked;

        AbsoluteLayout.SetLayoutBounds(
            button,
            new Rect(
                x * scale,
                y * scale,
                95 * scale,
                70 * scale));

        ChartLayout.Children.Add(button);
    }



    private async void Table_Clicked(
   object? sender,
   EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (ViewModel == null)
            return;

        if (ViewModel.SelectedGuest == null)
        {
            var occupiedGuest =
            _database.GetAllGuests()
            .FirstOrDefault(g =>
            g.TableNumber == button.Text &&
            (g.Status == "Seated" ||
            g.Status == "Orders In"));

            if (occupiedGuest != null)
            {
                if (ParentPage != null)
                {
                    await ParentPage.ShowGuestActionMenuAsync(
                        occupiedGuest);

                    RefreshChart();

                    return;
                }
            }
            string action =
            await Application.Current!
             .Windows[0]
             .Page!
             .DisplayActionSheetAsync(
             $"Table {button.Text}",
             "Cancel",
             null,
             "Quick Seat Walk-In");

            if (action != "Quick Seat Walk-In")
                return;

            var walkInGuest = new Guest
            {
                GuestName = $"Walk-In ({button.Text})",
                Adults = 0,
                Children = 0,
                HighChairRequired = false,
                Status = "Seated",
                TableNumber = button.Text,
                TimeAdded = DateTime.Now.ToString("h:mm tt"),
                TimeAddedDate = DateTime.Now,
                SortOrder = _database.GetGuests().Count + 1
            };

            _database.AddGuest(walkInGuest);

            await Application.Current!
                .Windows[0]
                .Page!
                .DisplayAlertAsync(
                    "Walk-In Seated",
                    $"{walkInGuest.GuestName} has been seated at Table {button.Text}.",
                    "OK");

            RefreshChart();

            return;
        }

        if (ViewModel.SelectedGuest == null)
            return;
        bool answer =
        await Application.Current!
        .Windows[0]
        .Page!
        .DisplayAlertAsync(
            "Assign & Seat",
            $"Seat {ViewModel.SelectedGuest!.GuestName} at Table {button.Text}?",
            "Yes",
            "No");

        if (!answer)
            return;

        var table =
            ViewModel.Tables.FirstOrDefault(
                t => t.Name == button.Text);

        if (table == null)
        {
            await Application.Current!
            .Windows[0]
            .Page!
            .DisplayAlertAsync(
                "Table Not Found",
                $"Table {button.Text} was not found.",
                "OK");

            return;
        }

        ViewModel.SelectedTable = table;

        string result = ViewModel.AssignSelectedTable();

        if (!string.IsNullOrWhiteSpace(result))
        {
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                 "Assign Table",
                result,
                "OK");

            return;
        }

        result = ViewModel.SeatSelectedGuest();

        if (!string.IsNullOrWhiteSpace(result))
        {
            await Application.Current!
                .Windows[0]
                .Page!
                .DisplayAlertAsync(
                    "Seat Guest",
                    result,
                    "OK");

            return;
        }

        ViewModel.SelectedGuest = null;

        RefreshChart();

    }
}