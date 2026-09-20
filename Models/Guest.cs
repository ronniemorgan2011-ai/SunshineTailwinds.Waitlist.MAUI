using SQLite;

namespace SunshineTailwinds.Waitlist.MAUI.Models;

public class Guest
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string GuestName { get; set; } = "";

    public int Adults { get; set; }

    public int Children { get; set; }

    public bool HighChairRequired { get; set; }

    public string Status { get; set; } = "Waiting";

    public string TableNumber { get; set; } = "";

    public string TimeAdded { get; set; } = "";

    public DateTime TimeAddedDate { get; set; }

    [Ignore]
    public int WaitMinutes
    {
        get
        {
            if (TimeAddedDate.Year < 2000)
                return 0;

            return (int)(DateTime.Now - TimeAddedDate)
                .TotalMinutes;
        }
    }
    [Ignore]
    public string HighChairDisplay =>
        HighChairRequired ? "✓" : "";
}