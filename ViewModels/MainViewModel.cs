using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using SunshineTailwinds.Waitlist.MAUI.Models;
using SunshineTailwinds.Waitlist.MAUI.Services;

namespace SunshineTailwinds.Waitlist.MAUI.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly DatabaseService _database;

    private Guest? _selectedGuest;
    private TableInfo? _selectedTable;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }

    public ObservableCollection<Guest> Guests { get; set; }

    public ObservableCollection<TableInfo> Tables { get; set; }

    public Guest? SelectedGuest
    {
        get => _selectedGuest;
        set
        {
            _selectedGuest = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentWaitMinutes));
        }
    }
    

    public TableInfo? SelectedTable
    {
        get => _selectedTable;
        set
        {
            _selectedTable = value;
            OnPropertyChanged();
        }
    }

    public int WaitingCount =>
        Guests.Count(g => g.Status == "Waiting");

    public int SeatedCount =>
        Guests.Count(g => g.Status == "Seated");

    public int GuestCount =>
        Guests.Count;

    public int OccupiedCount =>
        Guests.Count(g =>
            !string.IsNullOrWhiteSpace(g.TableNumber));
            public int CurrentWaitMinutes =>
            SelectedGuest?.WaitMinutes ?? 0;
            public int LongestWaitMinutes =>
            Guests.Any()
            ? Guests.Max(g => g.WaitMinutes)
            : 0;

    public MainViewModel()
    {
        _database = new DatabaseService();

        Guests = new ObservableCollection<Guest>();
        Tables = new ObservableCollection<TableInfo>();

        LoadGuests();
        LoadTables();
    }

    public void LoadTables()
    {
        Tables.Clear();

        foreach (var table in _database.GetTables())
        {
            Tables.Add(table);
        }

        OnPropertyChanged(nameof(Tables));
    }

    public void LoadGuests()
    {
        Guests.Clear();

        foreach (var guest in _database.GetGuests())
        {
            Guests.Add(guest);
        }

        OnPropertyChanged(nameof(WaitingCount));
        OnPropertyChanged(nameof(SeatedCount));
        OnPropertyChanged(nameof(GuestCount));
        OnPropertyChanged(nameof(OccupiedCount));
        OnPropertyChanged(nameof(SelectedGuest));
        OnPropertyChanged(nameof(CurrentWaitMinutes));
        OnPropertyChanged(nameof(LongestWaitMinutes));
    }

    public void AddGuest(
        string guestName,
        int adults,
        int children,
        bool highChairRequired)
    {
        Guest guest = new Guest
        {
            GuestName = guestName,
            Adults = adults,
            Children = children,
            HighChairRequired = highChairRequired,
            Status = "Waiting",
            TableNumber = "",
            TimeAdded = DateTime.Now.ToShortTimeString(),
            TimeAddedDate = DateTime.Now
        };

        _database.AddGuest(guest);

        LoadGuests();
    }

    public string AssignSelectedTable()
    {
        if (SelectedGuest == null)
            return "No guest selected.";

        if (SelectedTable == null)
            return "No table selected.";

        bool tableAlreadyAssigned =
            Guests.Any(g =>
                g.Id != SelectedGuest.Id &&
                g.TableNumber == SelectedTable.Name);

        if (tableAlreadyAssigned)
        {
            return $"Table {SelectedTable.Name} is already assigned.";
        }

        SelectedGuest.TableNumber = SelectedTable.Name;

        _database.UpdateGuest(SelectedGuest);

        LoadGuests();

        return "";
    }

    public void SeatSelectedGuest()
    {
        if (SelectedGuest == null)
            return;

        SelectedGuest.Status = "Seated";

        _database.UpdateGuest(SelectedGuest);

        LoadGuests();
    }

    public void OrdersInSelectedGuest()
    {
        if (SelectedGuest == null)
            return;

        SelectedGuest.Status = "Orders In";

        _database.UpdateGuest(SelectedGuest);

        LoadGuests();
    }

    public void CloseSelectedGuest()
    {
        if (SelectedGuest == null)
            return;

        SelectedGuest.Status = "Closed";

        _database.UpdateGuest(SelectedGuest);

        LoadGuests();
    }

    public void RemoveSelectedGuest()
    {
        if (SelectedGuest == null)
            return;

        _database.DeleteGuest(SelectedGuest);

        SelectedGuest = null;

        LoadGuests();
    }
}