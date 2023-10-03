using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

[Category("LabelText", 4)]
public interface ITodoLabelText
{
    [StringShortConfig("DailyTasksLabel")]
    public string DailyLabel { get; set; }
    
    [StringShortConfig("WeeklyTasksLabel")]
    public string WeeklyLabel { get; set; }
    
    [StringShortConfig("SpecialTasksLabel")]
    public string SpecialLabel { get; set; }
}