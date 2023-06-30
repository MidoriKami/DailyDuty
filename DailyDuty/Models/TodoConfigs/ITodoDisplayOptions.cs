using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

[Category("DisplayOptions", 2)]
public interface ITodoDisplayOptions
{
    [BoolConfig("Background")] 
    public bool BackgroundImage { get; set; }

    [BoolConfig("ShowHeaders")]
    public bool ShowHeaders { get; set; }

    [BoolConfig("HideInQuestEvent")]
    public bool HideDuringQuests { get; set; }
    
    [BoolConfig("HideInDuties")]
    public bool HideInDuties { get; set; }
}