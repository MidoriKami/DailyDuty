using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

[Category("LabelText", 4)]
public interface ITodoLabelText
{
    [StringShortConfig("DailyTasksLabel", true)]
    public string DailyLabel { get; set; }
    
    [StringShortConfig("WeeklyTasksLabel", true)]
    public string WeeklyLabel { get; set; }
    
    [StringShortConfig("SpecialTasksLabel", true)]
    public string SpecialLabel { get; set; }
}