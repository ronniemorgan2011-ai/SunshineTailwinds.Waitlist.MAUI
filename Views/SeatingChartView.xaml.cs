using SunshineTailwinds.Waitlist.MAUI.ViewModels;
using SunshineTailwinds.Waitlist.MAUI.Models;
using SunshineTailwinds.Waitlist.MAUI.Services;

namespace SunshineTailwinds.Waitlist.MAUI.Views;

public partial class SeatingChartView : ContentView
{
    private readonly DatabaseService _database = new();
    public MainViewModel? ViewModel { get; set; }

    public SeatingChartView()
    {
        InitializeComponent();

        AddDemoTables();

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
        CreateTable("15", 560, 40);
        CreateTable("14", 660, 40);
        CreateTable("23", 760, 40);

        // 16/13 Columns
        CreateTable("16B", 560, 130);
        CreateTable("16A", 560, 220);

        CreateTable("13B", 760, 130);
        CreateTable("13A", 760, 220);

        // 10 / 11 / 12 Area
        CreateTable("10B", 660, 320);
        CreateTable("10A", 660, 410);

        CreateTable("12", 760, 320);
        CreateTable("11", 760, 410);

        // 9A / 9B
        CreateTable("9A", 660, 560);
        CreateTable("9B", 760, 560);

        // Bottom row
        CreateTable("5A", 560, 720);
        CreateTable("5B", 660, 720);
        CreateTable("6", 760, 720);
        CreateTable("7", 860, 720);
        CreateTable("8", 960, 720);

        // Top Row
        CreateTable("22", 140, 180);
        CreateTable("21", 240, 180);
        CreateTable("20", 340, 180);
        CreateTable("19", 440, 180);

        // C tables
        CreateTable("CTR24", 180, 420);
        CreateTable("CTR25", 180, 510);
        CreateTable("CTR26", 180, 600);
        CreateTable("CTR27", 180, 690);

        // 1-4 stack
        CreateTable("4", 420, 540);
        CreateTable("3", 420, 610);
        CreateTable("2", 420, 680);
        CreateTable("1", 420, 750);

    }

    private void CreateTable(
    string tableNumber,
    double x,
    double y)
    {
        var guests = _database.GetGuests();

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
            WidthRequest = 90,
            HeightRequest = 65,
            BackgroundColor = tableColor
        };

        button.Clicked += Table_Clicked;

        AbsoluteLayout.SetLayoutBounds(
            button,
            new Rect(x, y, 90, 65));

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
            await Application.Current!
            .Windows[0]
            .Page!
            .DisplayAlertAsync(
                "No Guest Selected",
                "Please select a guest from the waitlist first.",
                "OK");

            return;
        }
        string guestName = ViewModel.SelectedGuest.GuestName;

        bool answer =
        await Application.Current!
        .Windows[0]
        .Page!
        .DisplayAlertAsync(
            "Assign & Seat",
            $"Seat {ViewModel.SelectedGuest.GuestName} at Table {button.Text}?",
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

        RefreshChart();
             
    }
}