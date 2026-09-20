using SQLite;
using SunshineTailwinds.Waitlist.MAUI.Models;

namespace SunshineTailwinds.Waitlist.MAUI.Services;

public class DatabaseService
{
    private SQLiteConnection _database;

    public DatabaseService()
    {
        string dbPath =
            Path.Combine(
                FileSystem.AppDataDirectory,
                "CafeWaitlist.db");

        _database =
            new SQLiteConnection(dbPath);

        _database.CreateTable<Guest>();
        _database.CreateTable<TableInfo>();

        SeedTables();
    }

    private void SeedTables()
    {
        if (_database.Table<TableInfo>().Any())
            return;

        List<string> defaultTables = new();

        for (int i = 1; i <= 23; i++)
        {
            defaultTables.Add(i.ToString());
        }

        defaultTables.AddRange(new[]
        {
            "9A",
            "9B",
            "10A",
            "10B",
            "16A",
            "16B",
            "CTR24",
            "CTR25",
            "CTR26",
            "CTR27",
            "TOGO",
            "GRUBHUB"
        });

        foreach (var table in defaultTables)
        {
            _database.Insert(
                new TableInfo
                {
                    Name = table,
                    IsActive = true
                });
        }
    }

    public List<TableInfo> GetAllTables()
    {
        return _database.Table<TableInfo>()
            .ToList();
    }

    public void EnableTable(int id)
    {
        var table =
            _database.Table<TableInfo>()
            .FirstOrDefault(t => t.Id == id);

        if (table == null)
            return;

        table.IsActive = true;

        _database.Update(table);
    }

    public void RenameTable(
    int id,
    string newName)
    {
        var table =
            _database.Table<TableInfo>()
            .FirstOrDefault(t => t.Id == id);

        if (table == null)
            return;

        table.Name = newName;

        _database.Update(table);
    }

    public List<TableInfo> GetTables()
    {
        var tables = _database.Table<TableInfo>()
            .Where(t => t.IsActive)
            .ToList();

        return tables
            .OrderBy(t =>
            {
                if (int.TryParse(t.Name, out int tableNumber))
                    return tableNumber;

                return 999;
            })
            .ThenBy(t => t.Name)
            .ToList();
    }

    public void AddTable(TableInfo table)
    {
        _database.Insert(table);
    }

    public void DisableTable(int id)
    {
        var table =
            _database.Table<TableInfo>()
            .FirstOrDefault(t => t.Id == id);

        if (table == null)
            return;

        table.IsActive = false;

        _database.Update(table);
    }

    public List<Guest> GetGuests()
    {
        return _database.Table<Guest>()
            .Where(g =>
                g.Status == "Waiting" ||
                g.Status == "Seated" ||
                g.Status == "Orders In")
            .ToList();
    }

    public int AddGuest(Guest guest)
    {
        return _database.Insert(guest);
    }

    public void UpdateGuest(Guest guest)
    {
        _database.Update(guest);
    }

    public void DeleteGuest(Guest guest)
    {
        _database.Delete(guest);
    }

    public void DeleteClosedGuests()
    {
        var closedGuests =
            _database.Table<Guest>()
            .Where(g => g.Status == "Closed")
            .ToList();

        foreach (var guest in closedGuests)
        {
            _database.Delete(guest);
        }
    }

    public void DeleteAllGuests()
    {
        _database.DeleteAll<Guest>();
    }
}