using DailyDuty.Classes;
using DailyDuty.Classes.TodoList;
using DailyDuty.Models;
using DailyDuty.Views;
using KamiLib.CommandManager;
using KamiLib.Window;
using KamiToolKit;

namespace DailyDuty;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public static class System {
	public static ModuleController ModuleController { get; set; }
	public static SystemConfig SystemConfig { get; set; }
	public static LocalizationController LocalizationController { get; set; }
	public static PayloadController PayloadController { get; set; }
	public static WindowManager WindowManager { get; set; }
	public static CommandManager CommandManager { get; set; }
	public static TeleporterController TeleporterController { get; set; }
	public static NativeController NativeController { get; set; }
	public static ConfigurationWindow ConfigurationWindow { get; set; }
	public static TodoListController TodoListController { get; set; }
	public static TodoConfig TodoConfig { get; set; }
}