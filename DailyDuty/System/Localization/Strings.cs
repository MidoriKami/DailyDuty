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
    public readonly string Label = Loc.Localize("Configuration_Label", "Configuration");
    public readonly string ModuleNotSelected = Loc.Localize("Configuration_ModuleNotSelected", "Select an item to configure in the left pane");
    public readonly string Options = Loc.Localize("Configuration_Options", "Options");
    public readonly string MarkCompleteWhen = Loc.Localize("Configuration_MarkCompleteWhen", "Mark Complete When");
    public readonly string NotificationOptions = Loc.Localize("Configuration_NotificationOptions", "Notification Options");
    public readonly string OnLogin = Loc.Localize("Configuration_OnLogin", "Send Notification on Login");
    public readonly string OnZoneChange = Loc.Localize("Configuration_OnZoneChange", "Send Notification on Zone Change");
}

public class Status
{
    public readonly string Label = Loc.Localize("Status_Label", "Status");
    public readonly string ModuleStatus = Loc.Localize("Status_ModuleStatus", "Module Status");
}

public class Common
{
    public Expansion Expansion { get; } = new();

    public readonly string Command = Loc.Localize("Command", "Command");
    public readonly string Enabled = Loc.Localize("Enabled", "Enabled");
    public readonly string Disabled = Loc.Localize("Disabled", "Disabled");
    public readonly string Unknown = Loc.Localize("Unknown", "Unknown");
    public readonly string Incomplete = Loc.Localize("Incomplete", "Incomplete");
    public readonly string Unavailable = Loc.Localize("Unavailable", "Unavailable");
    public readonly string Complete = Loc.Localize("Complete", "Complete");
    public readonly string LessThanLabel = Loc.Localize("LessThanLabel", "Less Than");
    public readonly string LessThanOrEqualLabel = Loc.Localize("LessThanOrEqualLabel", "Less Than or Equal To");
    public readonly string EqualToLabel = Loc.Localize("EqualToLabel", "Equal To");
    public readonly string Allowances = Loc.Localize("Allowances", "Allowances");
    public readonly string Allowance = Loc.Localize("Allowance", "Allowance");
    public readonly string Target = Loc.Localize("Target", "Target");
    public readonly string Mode = Loc.Localize("Mode", "Mode");
    public readonly string TopLeft = Loc.Localize("TopLeft", "Top Left");
    public readonly string TopRight = Loc.Localize("TopRight", "Top Right");
    public readonly string BottomLeft = Loc.Localize("BottomLeft", "Bottom Left");
    public readonly string BottomRight = Loc.Localize("BottomRight", "Bottom Right");
    public readonly string Header = Loc.Localize("Header", "Header");
    public readonly string Daily = Loc.Localize("Daily", "Daily");
    public readonly string Weekly = Loc.Localize("Weekly", "Weekly");
}

public class Expansion
{
    public readonly string RealmReborn = Loc.Localize("Expansion_RealmReborn", "A Realm Reborn");
    public readonly string Heavensward = Loc.Localize("Expansion_Heavensward", "Heavensward");
    public readonly string Stormblood = Loc.Localize("Expansion_Stormblood", "Stormblood");
    public readonly string Shadowbringers = Loc.Localize("Expansion_Shadowbringers", "Shadowbringers");
    public readonly string Endwalker = Loc.Localize("Expansion_Endwalker", "Endwalker");
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
    public readonly string Timers = Loc.Localize("Help_Timers", "\n/dd timers - Shows timer configuration window\n" +
                                                                "/dd timers help - Shows this help message\n" +
                                                                "/dd timers show - Shows the timers window\n" +
                                                                "/dd timers hide - Hides the timers window");

    public readonly string Todo = Loc.Localize("Help_Todo", "\n/dd todo - Shows todo configuration window\n" +
                                                            "/dd todo help - Shows this help message\n" +
                                                            "/dd todo show - Shows the todo window\n" +
                                                            "/dd todo hide - Hides the todo window");

    public readonly string Base = Loc.Localize("Help_Base", "Command Overview\n" +
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
    public readonly string Label = Loc.Localize("BeastTribe_Label", "Beast Tribe");
    public readonly string AllowancesRemaining = Loc.Localize("BeastTribe_AllowancesRemaining", "Allowances Remaining");
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
    public readonly string LevelOne = Loc.Localize("HuntMarks_LevelOne", "Level One");
    public readonly string LevelTwo = Loc.Localize("HuntMarks_LevelTwo", "Level Two");
    public readonly string LevelThree = Loc.Localize("HuntMarks_LevelThree", "Level Three");
    public readonly string Elite = Loc.Localize("HuntMarks_Elite", "Elite");
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
    public readonly Todo Todo = new();
    public readonly Timers Timers = new();
}

public class Todo
{
    public readonly string MainOptions = Loc.Localize("Todo_MainOptions", "Main Options");
    public readonly string TaskSelection = Loc.Localize("Todo_TaskSelection", "Task Selection");
    public readonly string ShowDailyTasks = Loc.Localize("Todo_ShowDailyTasks", "Show Daily Tasks");
    public readonly string ShowWeeklyTasks = Loc.Localize("Todo_ShowWeeklyTasks", "Show Weekly Tasks");
    public readonly string TaskDisplay = Loc.Localize("Todo_TaskDisplay", "Task Display Options");
    public readonly string HideCompletedTasks = Loc.Localize("Todo_HideCompletedTasks", "Hide Completed Tasks");
    public readonly string HideUnavailable = Loc.Localize("Todo_HideUnavailable", "Hide Unavailable Tasks");
    public readonly string WindowOptions = Loc.Localize("Todo_WindowOptions", "Window Options");
    public readonly string HideWindowCompleted = Loc.Localize("Todo_HideWindowCompleted", "Hide Window When Complete");
    public readonly string HideWindowInDuty = Loc.Localize("Todo_HideWindowInDuty", "Hide Window In Duty");
    public readonly string PositionOptions = Loc.Localize("Todo_PositionOptions", "Window Position Options");
    public readonly string LockWindow = Loc.Localize("Todo_LockWindow", "Lock Window Position");
    public readonly string AutoResize = Loc.Localize("Todo_AutoResize", "Auto Resize");
    public readonly string AnchorCorner = Loc.Localize("Todo_AnchorCorner", "Anchor Corner");
    public readonly string Opacity = Loc.Localize("Todo_Opacity", "Opacity");
    public readonly string ColorOptions = Loc.Localize("Todo_ColorOptions", "Color Options");
    public readonly string DailyTasks = Loc.Localize("Todo_DailyTasks", "Daily Tasks");
    public readonly string WeeklyTasks = Loc.Localize("Todo_WeeklyTasks", "Weekly Tasks");
    public readonly string NoTasksEnabled = Loc.Localize("Todo_NoDailyTasksEnabled", "Enable a module in Configuration to track Tasks");
    public readonly string UseLongLabel = Loc.Localize("Todo_UseLongLabel", "Use Long Label");
}

public class Timers
{
    public readonly string MainOptions = Loc.Localize("Timers_MainOptions", "Main Options");
    public readonly string WindowOptions = Loc.Localize("Timers_WindowOptions", "Window Options");
    public readonly string HideWindowInDuty = Loc.Localize("Timers_HideWindowInDuty", "Hide Window In Duty");
    public readonly string PositionOptions = Loc.Localize("Timers_PositionOptions", "Window Position Options");
    public readonly string LockWindow = Loc.Localize("Timers_LockWindow", "Lock Window Position");
    public readonly string Opacity = Loc.Localize("Timers_Opacity", "Opacity");
    public readonly string NoTimersEnabledWarning = Loc.Localize("Timers_NoneEnabledWarning", "Enable which timers you would like to see here with '/dd timers' command");
    public readonly string Label = Loc.Localize("Timers_Label", "Timers");
    public readonly string EditTimer = Loc.Localize("Timers_EditTimer", "Edit Style");
    public readonly string EditTimerTitle = Loc.Localize("Timers_EditTimerTitle", "Edit Timer Style");
    public readonly string AutoResize = Loc.Localize("Timers_AutoResize", "Auto Resize");

}


public class Language
{
    public readonly string Changed = Loc.Localize("Language_Changed", "Language has been changed, a restart is required for this to take effect");
}