using DailyDuty.Classes;
using DailyDuty.Configs;
using DailyDuty.Windows;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiLib.Classes;
using KamiLib.CommandManager;
using KamiLib.Window;
using KamiToolKit.Classes.Controllers;
using KamiToolKit.Overlay;

namespace DailyDuty;

public static class System {
	public static ModuleController ModuleController { get; set; } = null!;
	public static SystemConfig SystemConfig { get; set; } = null!;
	public static PayloadController PayloadController { get; set; } = null!;
	public static WindowManager WindowManager { get; set; } = null!;
	public static CommandManager CommandManager { get; set; } = null!;
	public static Teleporter Teleporter { get; set; } = null!;
	public static ConfigurationWindow ConfigurationWindow { get; set; } = null!;
	public static TodoListController TodoListController { get; set; } = null!;
	public static TimersController TimersController { get; set; } = null!;
	public static TodoConfig TodoConfig { get; set; } = null!;
	public static TimersConfig TimersConfig { get; set; } = null!;
	public static AddonController<AddonContentsFinder> ContentsFinderController { get; set; } = null!;
	public static OverlayController OverlayController { get; set; } = null!;
	public static DtrController DtrController { get; set; } = null!;
}