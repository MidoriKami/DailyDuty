using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

[Category("TodoDisplayEnable", -1)]
public interface ITodoDisplayEnable
{
    [BoolConfig("Enable")]
    public bool Enable { get; set; }
    
    [BoolConfig("PreviewMode")]
    public bool PreviewMode { get; set; }
}