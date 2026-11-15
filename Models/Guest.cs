using SQLite;
using Microsoft.Maui.Graphics;

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
    public DateTime? OrdersInDate { get; set; }
    
    public DateTime? ReservationDateTime { get; set; }
    public int SortOrder { get; set; }



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
    [Ignore]
    public string GuestDisplay
    {
        get
        {
            string displayName = GuestName;

            if (Status == "Reserved" &&
                ReservationDateTime.HasValue)
            {
                displayName +=
                    $" (RES. @ {ReservationDateTime.Value:h:mm tt})";
            }

            if (HighChairRequired)
            {
                displayName += " 🪑";
            }

            return displayName;
        }
    }

    [Ignore]
    public Color StatusColor =>
 Status switch
 {
     "Reserved" => Color.FromArgb("#B3E5FC"),
     "Seated" => Color.FromArgb("#FFE082"),
     "Orders In" => Color.FromArgb("#F48FB1"),
     _ => Colors.Transparent
 };
}