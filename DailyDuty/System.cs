using DailyDuty.Classes;
using DailyDuty.Windows;

namespace DailyDuty;

public static class System {
	public static ModuleManager ModuleManager { get; set; } = null!;
	public static PayloadController PayloadController { get; set; } = null!;
    public static ModuleBrowserWindow ConfigurationWindow { get; set; } = null!;
	public static SystemConfig? SystemConfig { get; set; }
}
