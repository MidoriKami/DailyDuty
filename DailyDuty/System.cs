using DailyDuty.Classes;
using DailyDuty.ConfigurationWindow;

namespace DailyDuty;

public static class System {
	public static ModuleManager ModuleManager { get; set; } = null!;
	public static PayloadController PayloadController { get; set; } = null!;
    public static ConfigWindow ConfigurationWindow { get; set; } = null!;
	public static SystemConfig? SystemConfig { get; set; }
}
