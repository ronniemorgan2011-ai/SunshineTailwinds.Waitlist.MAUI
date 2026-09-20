using SQLite;

namespace SunshineTailwinds.Waitlist.MAUI.Models;

public class TableInfo
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public bool IsActive { get; set; } = true;
}