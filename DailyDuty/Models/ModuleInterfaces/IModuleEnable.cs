using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models;

[Category("ModuleEnable")]
public interface IModuleEnable
{
    [BoolConfig("Enable")]
    public bool ModuleEnabled { get; set; }
}