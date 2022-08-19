using CheapLoc;

namespace DailyDuty.System.Localization;

internal static class Strings
{
    public static Configuration Configuration { get; } = new();
    public static Status Status { get; } = new();
    public static Common Common { get; } = new();
    public static Command Command { get; } = new();
    public static Module Module { get; } = new();
    public static UserInterface UserInterface { get; } = new();
    public static Language Language { get; } = new();
}

public class Configuration
{
    public string Label => Loc.Localize("Configuration_Label", "Configuration");
    public string ModuleNotSelected => Loc.Localize("Configuration_ModuleNotSelected", "Select an item to configure in the left pane");
    public string Options => Loc.Localize("Configuration_Options", "Options");
    public string MarkCompleteWhen => Loc.Localize("Configuration_MarkCompleteWhen", "Mark Complete When");
    public string NotificationOptions => Loc.Localize("Configuration_NotificationOptions", "Notification Options");
    public string OnLogin => Loc.Localize("Configuration_OnLogin", "Send Notification on Login");
    public string OnZoneChange => Loc.Localize("Configuration_OnZoneChange", "Send Notification on Zone Change");
}

public class Status
{
    public string Label => Loc.Localize("Status_Label", "Status");
    public string ModuleStatus => Loc.Localize("Status_ModuleStatus", "Module Status");
}

public class Common
{
    public Expansion Expansion { get; } = new();

    public string Command => Loc.Localize("Command", "Command");
    public string Enabled => Loc.Localize("Enabled", "Enabled");
    public string Disabled => Loc.Localize("Disabled", "Disabled");
    public string Unknown => Loc.Localize("Unknown", "Unknown");
    public string Incomplete => Loc.Localize("Incomplete", "Incomplete");
    public string Unavailable => Loc.Localize("Unavailable", "Unavailable");
    public string Complete => Loc.Localize("Complete", "Complete");
    public string LessThanLabel => Loc.Localize("LessThanLabel", "Less Than");
    public string LessThanOrEqualLabel => Loc.Localize("LessThanOrEqualLabel", "Less Than or Equal To");
    public string EqualToLabel => Loc.Localize("EqualToLabel", "Equal To");
    public string Allowances => Loc.Localize("Allowances", "Allowances");
    public string Allowance => Loc.Localize("Allowance", "Allowance");
    public string Target => Loc.Localize("Target", "Target");
    public string Mode => Loc.Localize("Mode", "Mode");
    public string TopLeft => Loc.Localize("TopLeft", "Top Left");
    public string TopRight => Loc.Localize("TopRight", "Top Right");
    public string BottomLeft => Loc.Localize("BottomLeft", "Bottom Left");
    public string BottomRight => Loc.Localize("BottomRight", "Bottom Right");
    public string Header => Loc.Localize("Header", "Header");
    public string Daily => Loc.Localize("Daily", "Daily");
    public string Weekly => Loc.Localize("Weekly", "Weekly");
}

public class Expansion
{
    public string RealmReborn => Loc.Localize("Expansion_RealmReborn", "A Realm Reborn");
    public string Heavensward => Loc.Localize("Expansion_Heavensward", "Heavensward");
    public string Stormblood => Loc.Localize("Expansion_Stormblood", "Stormblood");
    public string Shadowbringers => Loc.Localize("Expansion_Shadowbringers", "Shadowbringers");
    public string Endwalker => Loc.Localize("Expansion_Endwalker", "Endwalker");
}

public class Command
{
    public MainWindow MainWindow { get; } = new();
    public Help Help { get; } = new();
}

public class MainWindow
{
}

public class Help
{
    public string Timers => Loc.Localize("Help_Timers", "\n/dd timers - Shows timer configuration window\n" +
                                                        "/dd timers help - Shows this help message\n" +
                                                        "/dd timers show - Shows the timers window\n" +
                                                        "/dd timers hide - Hides the timers window");

    public string Todo => Loc.Localize("Help_Todo", "\n/dd todo - Shows todo configuration window\n" +
                                                    "/dd todo help - Shows this help message\n" +
                                                    "/dd todo show - Shows the todo window\n" +
                                                    "/dd todo hide - Hides the todo window");

    public string Base => Loc.Localize("Help_Base", "Command Overview\n" +
                                                    "/dd - Show or Hide Main Window\n" +
                                                    "/dd timers help - Show timer sub-commands\n" +
                                                    "/dd todo help - Show todo sub-commands");
}

public class Module
{
    public BeastTribe BeastTribe { get; } = new();
    public CustomDelivery CustomDelivery { get; } = new();
    public DomanEnclave DomanEnclave { get; } = new();
    public DutyRoulette DutyRoulette { get; } = new();
    public FashionReport FashionReport { get; } = new();
    public HuntMarks HuntMarks { get; } = new();
    public JumboCactpot JumboCactpot { get; } = new();
    public Levequest Levequest { get; } = new();
    public MiniCactpot MiniCactpot { get; } = new();
    public TreasureMap TreasureMap { get; } = new();
    public WondrousTails WondrousTails { get; } = new();
}



public class BeastTribe
{
    public string Label => Loc.Localize("BeastTribe_Label", "Beast Tribe");
}

public class CustomDelivery
{
}

public class DomanEnclave
{
}

public class DutyRoulette
{
}

public class FashionReport
{

}

public class HuntMarks
{
    public string LevelOne => Loc.Localize("HuntMarks_LevelOne", "Level One");
    public string LevelTwo => Loc.Localize("HuntMarks_LevelTwo", "Level Two");
    public string LevelThree => Loc.Localize("HuntMarks_LevelThree", "Level Three");
    public string Elite => Loc.Localize("HuntMarks_Elite", "Elite");
}

public class JumboCactpot
{

}

public class Levequest
{

}

public class MiniCactpot
{

}

public class TreasureMap
{

}

public class WondrousTails
{

}

public class UserInterface
{
    public Todo Todo = new();
}

public class Todo
{
    public string MainOptions => Loc.Localize("Todo_MainOptions", "Main Options");
    public string TaskSelection => Loc.Localize("Todo_TaskSelection", "Task Selection");
    public string ShowDailyTasks => Loc.Localize("Todo_ShowDailyTasks", "Show Daily Tasks");
    public string ShowWeeklyTasks => Loc.Localize("Todo_ShowWeeklyTasks", "Show Weekly Tasks");
    public string TaskDisplay => Loc.Localize("Todo_TaskDisplay", "Task Display Options");
    public string HideCompletedTasks => Loc.Localize("Todo_HideCompletedTasks", "Hide Completed Tasks");
    public string HideUnavailable => Loc.Localize("Todo_HideUnavailable", "Hide Unavailable Tasks");
    public string WindowOptions => Loc.Localize("Todo_WindowOptions", "Window Options");
    public string HideWindowCompleted => Loc.Localize("Todo_HideWindowCompleted", "Hide Window When Complete");
    public string HideWindowInDuty => Loc.Localize("Todo_HideWindowInDuty", "Hide Window In Duty");
    public string PositionOptions => Loc.Localize("Todo_PositionOptions", "Window Position Options");
    public string LockWindow => Loc.Localize("Todo_LockWindow", "Lock Window Position");
    public string AutoResize => Loc.Localize("Todo_AutoResize", "Auto Resize");
    public string AnchorCorner => Loc.Localize("Todo_AnchorCorner", "Anchor Corner");
    public string Opacity => Loc.Localize("Todo_Opacity", "Opacity");
    public string ColorOptions => Loc.Localize("Todo_ColorOptions", "ColorOptions");
    public string DailyTasks => Loc.Localize("Todo_DailyTasks", "Daily Tasks");
    public string WeeklyTasks => Loc.Localize("Todo_WeeklyTasks", "Weekly Tasks");
    public string NoTasksEnabled => Loc.Localize("Todo_NoDailyTasksEnabled", "Enable a module in Configuration to track Tasks");
}

public class Language
{
    public string Changed => Loc.Localize("Language_Changed", "Language has been changed, a restart is required to update some strings");
}