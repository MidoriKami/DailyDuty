using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

[Category("Categories", 1)]
public interface ITodoCategories
{
    [BoolConfig("EnableDailyTasks")] 
    public bool DailyTasks { get; set; }
    
    [BoolConfig("EnableWeeklyTasks")]
    public bool WeeklyTasks { get; set; }
    
    [BoolConfig("EnableSpecialTasks")]
    public bool SpecialTasks { get; set; }
}