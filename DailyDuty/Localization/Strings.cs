using CheapLoc;

namespace DailyDuty.Localization;

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
    public readonly string Label = Loc.Localize("CustomDelivery_Label", "Custom Delivery");
    public readonly string AllowancesRemaining = Loc.Localize("CustomDelivery_AllowancesRemaining", "Allowances Remaining");
}

public class DomanEnclave
{
    public readonly string Label = Loc.Localize("DomanEnclave_Label", "Doman Enclave");
    public readonly string GilRemaining = Loc.Localize("DomanEnclave_GilRemaining", "gil Remaining");
    public readonly string UnknownStatus = Loc.Localize("DomanEnclave_UnknownStatus", "Status unknown, visit the Doman Enclave to initialize module");
    public readonly string UnknownStatusLabel = Loc.Localize("DomanEnclave_UnknownStatusLabel", "Status Unknown");
    public readonly string BudgetRemaining = Loc.Localize("DomanEnclave_BudgetRemaining", "Budget Remaining");
    public readonly string CurrentAllowance = Loc.Localize("DomanEnclave_CurrentAllowance", "Current Allowance");
    public readonly string ClickableLinkLabel = Loc.Localize("DomanEnclave_ClickableLinkLabel", "Clickable Link");
    public readonly string ClickableLink = Loc.Localize("DomanEnclave_ClickableLink", "Notifications can be clicked on to teleport to the Doman Enclave");
}

public class DutyRoulette
{
    public readonly string Label = Loc.Localize("DutyRoulette_Label", "Duty Roulette");
    public readonly string RouletteSelection = Loc.Localize("DutyRoulette_RouletteSelection", "Select Roulettes to Track");
    public readonly string ClickableLinkLabel = Loc.Localize("DutyRoulette_ClickableLinkLabel", "Clickable Link");
    public readonly string ClickableLink = Loc.Localize("DutyRoulette_ClickableLink", "Notifications can be clicked on to open the Duty Finder");
    public readonly string Remaining = Loc.Localize("DutyRoulette_Remaining", "Roulettes Remaining");
    public readonly string HideExpertWhenCapped = Loc.Localize("DutyRoulette_HideExpertWhenCapped", "Hide Expert when Tomecapped");
    public readonly string ExpertTomestones = Loc.Localize("DutyRoulette_ExpertTomestones", "Expert Tomestones");
    public readonly string Overlay = Loc.Localize("DutyRoulette_Overlay", "Duty Finder Overlay");
    public readonly string DutyComplete = Loc.Localize("DutyRoulette_DutyComplete", "Duty Complete");
    public readonly string DutyIncomplete = Loc.Localize("DutyRoulette_DutyIncomplete", "Duty Incomplete");
    public readonly string RouletteStatus = Loc.Localize("DutyRoulette_RouletteStatus", "Roulette Status");
    public readonly string NoRoulettesTracked = Loc.Localize("DutyRoulette_NoRoulettesTracked", "No roulettes have been selected for tracking");
}

public class FashionReport
{
    public readonly string Label = Loc.Localize("FashionReport_Label", "Fashion Report");
    public readonly string CompletionCondition = Loc.Localize("FashionReport_CompletionCondition", "Completion Condition");
    public readonly string ModeSingle = Loc.Localize("FashionReport_ModeSingle", "Single");
    public readonly string Mode80Plus = Loc.Localize("FashionReport_Mode80Plus", "80 Plus");
    public readonly string ModeAll = Loc.Localize("FashionReport_ModeAll", "All");
    public readonly string ModeSingleHelp = Loc.Localize("FashionReport_ModeSingleHelp", "Only notify if no allowances have been spent this week and Fashion Report is available for turn-in");
    public readonly string Mode80PlusHelp = Loc.Localize("FashionReport_Mode80PlusHelp", "Notify if any allowances remain this week, the highest score is below 80 and Fashion Report is available for turn-in");
    public readonly string ModeAllHelp = Loc.Localize("FashionReport_ModeAllHelp", "Notify if any allowances remain this week and fashion report is available for turn-in");
    public readonly string ClickableLinkLabel = Loc.Localize("FashionReport_ClickableLinkLabel", "Clickable Link");
    public readonly string ClickableLink = Loc.Localize("FashionReport_ClickableLink", "Notifications can be clicked on to teleport to the Gold Saucer");
    public readonly string AllowancesAvailable = Loc.Localize("FashionReport_AllowancesAvailable", "Allowances Available");
    public readonly string HighestScore = Loc.Localize("FashionReport_HighestScore", "Highest Score");
    public readonly string ReportOpen = Loc.Localize("FashionReport_ReportOpen", "Report Open");
    public readonly string AvailableNow = Loc.Localize("FashionReport_AvailableNow", "Available Now");

}

public class HuntMarks
{
    public readonly string LevelOne = Loc.Localize("HuntMarks_LevelOne", "Level One");
    public readonly string LevelTwo = Loc.Localize("HuntMarks_LevelTwo", "Level Two");
    public readonly string LevelThree = Loc.Localize("HuntMarks_LevelThree", "Level Three");
    public readonly string Elite = Loc.Localize("HuntMarks_Elite", "Elite");
    public readonly string DailyLabel = Loc.Localize("HuntMarks_DailyLabel", "Hunt Marks (Daily)");
    public readonly string WeeklyLabel = Loc.Localize("HuntMarks_WeeklyLabel", "Hunt Marks (Weekly)");
    public readonly string TrackedHunts = Loc.Localize("HuntMarks_TrackedHunts", "Tracked Hunts");
    public readonly string Obtained = Loc.Localize("HuntMarks_Obtained", "Obtained");
    public readonly string Unobtained = Loc.Localize("HuntMarks_Unobtained", "Unobtained");
    public readonly string Killed = Loc.Localize("HuntMarks_Killed", "Killed");
    public readonly string HuntsRemaining = Loc.Localize("HuntMarks_HuntsRemaining", "Hunts Remaining");
    public readonly string TrackedHuntsStatus = Loc.Localize("HuntMarks_TrackedHuntsStatus", "Tracked Hunts Status");
    public readonly string NoHuntsTracked = Loc.Localize("HuntMarks_NoHuntsTracked", "No hunts have been selected for tracking");
}

public class JumboCactpot
{
    public readonly string Label = Loc.Localize("JumboCactpot_Label", "Jumbo Cactpot");
    public readonly string Tickets = Loc.Localize("JumboCactpot_Tickets", "Tickets");
    public readonly string NextDrawing = Loc.Localize("JumboCactpot_NextDrawing", "Next Drawing");
    public readonly string ClickableLink = Loc.Localize("JumboCactpot_ClickableLink", "Notifications can be clicked on to teleport to the Gold Saucer");
    public readonly string ClickableLinkLabel = Loc.Localize("JumboCactpot_ClickableLinkLabel", "Clickable Link");
    public readonly string TicketsAvailable = Loc.Localize("JumboCactpot_TicketsAvailable", "Tickets Available");
    public readonly string NoTickets = Loc.Localize("JumboCactpot_NoTickets", "No Tickets");
}

public class Levequest
{
    public readonly string Label = Loc.Localize("Levequest_Label", "Levequest");
    public readonly string Accepted = Loc.Localize("Levequest_Accepted", "Accepted");
    public readonly string NextAllowance = Loc.Localize("Levequest_NextAllowance", "Next Allowance");
    public readonly string AllowancesRemaining = Loc.Localize("Levequest_AllowancesRemaining", "Allowances Remaining");
}

public class MiniCactpot
{
    public readonly string Label = Loc.Localize("MiniCactpot_Label", "Mini Cactpot");
    public readonly string ClickableLink = Loc.Localize("MiniCactpot_ClickableLink", "Notifications can be clicked on to teleport to the Gold Saucer");
    public readonly string ClickableLinkLabel = Loc.Localize("MiniCactpot_ClickableLinkLabel", "Clickable Link");
    public readonly string TicketsRemaining = Loc.Localize("MiniCactpot_TicketsRemaining", "Tickets Remaining");

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
    public readonly Teleport Teleport = new();
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
    public readonly string LockWindow = Loc.Localize("Todo_LockWindow", "Lock Window Position");
    public readonly string AutoResize = Loc.Localize("Todo_AutoResize", "Auto Resize");
    public readonly string AnchorCorner = Loc.Localize("Todo_AnchorCorner", "Anchor Corner");
    public readonly string Opacity = Loc.Localize("Todo_Opacity", "Opacity");
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
    public readonly string LockWindow = Loc.Localize("Timers_LockWindow", "Lock Window Position");
    public readonly string Opacity = Loc.Localize("Timers_Opacity", "Opacity");
    public readonly string NoTimersEnabledWarning = Loc.Localize("Timers_NoneEnabledWarning", "Enable which timers you would like to see here with '/dd timers' command");
    public readonly string Label = Loc.Localize("Timers_Label", "Timers");
    public readonly string EditTimer = Loc.Localize("Timers_EditTimer", "Edit Style");
    public readonly string EditTimerTitle = Loc.Localize("Timers_EditTimerTitle", "Edit Timer Style");
    public readonly string AutoResize = Loc.Localize("Timers_AutoResize", "Auto Resize");
    public readonly string TimeDisplay = Loc.Localize("Timers_TimeDisplay", "Time Display Options");
    public readonly string ColorOptions = Loc.Localize("Timers_ColorOptions", "Color Options");
    public readonly string Background = Loc.Localize("Timers_BackgroundColor", "Background");
    public readonly string Foreground = Loc.Localize("Timers_ForegroundColor", "Foreground");
    public readonly string Text = Loc.Localize("Timers_TextColor", "Text");
    public readonly string Time = Loc.Localize("Timers_TimeColor", "Time");
    public readonly string SizeOptions = Loc.Localize("Timers_SizeOptions", "Size Options");
    public readonly string StretchToFit = Loc.Localize("Timers_StretchToFit", "Stretch to Fit");
    public readonly string Size = Loc.Localize("Timers_Size", "Size");
}

public class Teleport
{
    public readonly string Label = Loc.Localize("Teleport_Label", "Teleport");
    public readonly string Error = Loc.Localize("Teleport_Error", "Cannot teleport in this situation");
    public readonly string Teleporting = Loc.Localize("Teleport_Teleporting", "Teleporting to '{0}'");
    public readonly string CommunicationError = Loc.Localize("Teleport_CommunicationError", "To use the teleport function, you must install the \"Teleporter\" plugin");
    public readonly string NotUnlocked = Loc.Localize("Teleport_NotUnlocked", "Destination Aetheryte is not unlocked, teleport cancelled");
}

public class Language
{
    public readonly string Changed = Loc.Localize("Language_Changed", "Language has been changed, a restart is required for this to take effect");
}