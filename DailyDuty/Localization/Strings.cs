using System.Collections.Generic;
using CheapLoc;
// ReSharper disable MemberCanBeMadeStatic.Global

namespace DailyDuty.Localization;

internal static class Strings
{
    public static Configuration Configuration { get; } = new();
    public static Status Status { get; } = new();
    public static Common Common { get; } = new();
    public static Command Command { get; } = new();
    public static Module Module { get; } = new();
    public static UserInterface UserInterface { get; } = new();
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
    public string HideDisabled => Loc.Localize("Configuration_HideDisabled", "Hide Disabled");
    public string ShowDisabled => Loc.Localize("Configuration_ShowDisabled", "Show Disabled");
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
    public string Overriden => Loc.Localize("Overriden", "Overriden");
    public string LessThanLabel => Loc.Localize("LessThanLabel", "Less Than");
    public string LessThanOrEqualLabel => Loc.Localize("LessThanOrEqualLabel", "Less Than or Equal To");
    public string EqualToLabel => Loc.Localize("EqualToLabel", "Equal To");
    public string Allowances => Loc.Localize("Allowances", "Allowances");
    public string Target => Loc.Localize("Target", "Target");
    public string Mode => Loc.Localize("Mode", "Mode");
    public string TopLeft => Loc.Localize("TopLeft", "Top Left");
    public string TopRight => Loc.Localize("TopRight", "Top Right");
    public string BottomLeft => Loc.Localize("BottomLeft", "Bottom Left");
    public string BottomRight => Loc.Localize("BottomRight", "Bottom Right");
    public string Header => Loc.Localize("Header", "Header");
    public string Daily => Loc.Localize("Daily", "Daily");
    public string Weekly => Loc.Localize("Weekly", "Weekly");
    public string MessageTimeout => Loc.Localize("MessageTimeout", "Zone Change notifications will only appear once every five minutes\nThis is to prevent excessive chatlog spamming");
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
    public Help Help { get; } = new();
    public string InvalidCommand => Loc.Localize("Command_InvalidCommand", "Invalid Command");
}

public class Help
{
    public IEnumerable<string> TimersMessages => new[]
    {
        Loc.Localize("TimersMessages_Tag", "== Timers Commands =="),
        Loc.Localize("TimersMessages_Base", "/dd timers config - Shows timer configuration window"),
        Loc.Localize("TimersMessages_Help", "/dd help timers - Shows this help message"),
        Loc.Localize("TimersMessages_Show", "/dd timers show - Shows the timers window"),
        Loc.Localize("TimersMessages_Hide", "/dd timers hide - Hides the timers window"),
        Loc.Localize("TimersMessages_Toggle", "/dd timers toggle - Toggle the timers window"),
    };

    public IEnumerable<string> TodoMessages => new[]
    {
        Loc.Localize("TodoMessages_Tag", "== Todo Commands =="),
        Loc.Localize("TodoMessages_Base", "/dd todo config - Shows todo configuration window"),
        Loc.Localize("TodoMessages_Todo", "/dd todo - Displays all outstanding tasks"),
        Loc.Localize("TodoMessages_Help", "/dd help todo - Shows this help message"),
        Loc.Localize("TodoMessages_Show", "/dd todo show - Shows the todo window"),
        Loc.Localize("TodoMessages_Hide", "/dd todo hide - Hides the todo window"),
        Loc.Localize("TodoMessages_Toggle", "/dd todo toggle - Toggle the todo window"),
    };

    public IEnumerable<string> CoreMessages => new[]
    {
        Loc.Localize("CoreMessages_Tag", "== Main Commands =="),
        Loc.Localize("CoreMessages_Base", "/dd - Show or Hide Main Window"),
        Loc.Localize("CoreMessages_Timers", "/dd help timers - Show timer sub-commands"),
        Loc.Localize("CoreMessages_Todo", "/dd help todo - Show todo sub-commands"),
    };
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
    public ChallengeLog ChallengeLog { get; } = new();
    public Raids Raids { get; } = new();
    public FauxHollows FauxHollows { get; } = new ();
    public GrandCompany GrandCompany { get; } = new();
}

public class BeastTribe
{
    public string Label => Loc.Localize("BeastTribe_Label", "Beast Tribe");
    public string AllowancesRemaining => Loc.Localize("BeastTribe_AllowancesRemaining", "Allowances Remaining");
}

public class CustomDelivery
{
    public string Label => Loc.Localize("CustomDelivery_Label", "Custom Delivery");
    public string AllowancesRemaining => Loc.Localize("CustomDelivery_AllowancesRemaining", "Allowances Remaining");
}

public class DomanEnclave
{
    public string Label => Loc.Localize("DomanEnclave_Label", "Doman Enclave");
    public string GilRemaining => Loc.Localize("DomanEnclave_GilRemaining", "gil Remaining");
    public string UnknownStatus => Loc.Localize("DomanEnclave_UnknownStatus", "Status unknown, visit the Doman Enclave to initialize module");
    public string UnknownStatusLabel => Loc.Localize("DomanEnclave_UnknownStatusLabel", "Status Unknown");
    public string BudgetRemaining => Loc.Localize("DomanEnclave_BudgetRemaining", "Budget Remaining");
    public string CurrentAllowance => Loc.Localize("DomanEnclave_CurrentAllowance", "Current Allowance");
    public string ClickableLinkLabel => Loc.Localize("DomanEnclave_ClickableLinkLabel", "Clickable Link");
    public string ClickableLink => Loc.Localize("DomanEnclave_ClickableLink", "Notifications can be clicked on to teleport to the Doman Enclave");
}

public class DutyRoulette
{
    public string Label => Loc.Localize("DutyRoulette_Label", "Duty Roulette");
    public string RouletteSelection => Loc.Localize("DutyRoulette_RouletteSelection", "Select Roulettes to Track");
    public string ClickableLinkLabel => Loc.Localize("DutyRoulette_ClickableLinkLabel", "Clickable Link");
    public string ClickableLink => Loc.Localize("DutyRoulette_ClickableLink", "Notifications can be clicked on to open the Duty Finder");
    public string Remaining => Loc.Localize("DutyRoulette_Remaining", "Roulettes Remaining");
    public string HideExpertWhenCapped => Loc.Localize("DutyRoulette_HideExpertWhenCapped", "Expert Roulette Feature");
    public string HideExpertHelp => Loc.Localize("DutyRoulette_HideExpertHelp", "Marks the Expert Roulette as overriden when you are at your weekly tomestone cap");
    public string ExpertTomestones => Loc.Localize("DutyRoulette_ExpertTomestones", "Expert Tomestones");
    public string Overlay => Loc.Localize("DutyRoulette_Overlay", "Duty Finder Overlay");
    public string DutyComplete => Loc.Localize("DutyRoulette_DutyComplete", "Duty Complete");
    public string DutyIncomplete => Loc.Localize("DutyRoulette_DutyIncomplete", "Duty Incomplete");
    public string RouletteStatus => Loc.Localize("DutyRoulette_RouletteStatus", "Roulette Status");
    public string NoRoulettesTracked => Loc.Localize("DutyRoulette_NoRoulettesTracked", "No roulettes have been selected for tracking");
    public string Override => Loc.Localize("DutyRoulette_Override", "Duty Override");
    public string CompleteWhenCapped => Loc.Localize("DutyRoulette_CompleteWhenCapped", "Complete When Tomecapped");
    public string CompleteWhenCappedHelp => Loc.Localize("DutyRoulette_CompleteWhenCappedHelp", "Marks Duty Roulette Module as complete when you are at your weekly tomestone cap");
}

public class FashionReport
{
    public string Label => Loc.Localize("FashionReport_Label", "Fashion Report");
    public string CompletionCondition => Loc.Localize("FashionReport_CompletionCondition", "Completion Condition");
    public string ModeSingle => Loc.Localize("FashionReport_ModeSingle", "Single");
    public string Mode80Plus => Loc.Localize("FashionReport_Mode80Plus", "80 Plus");
    public string ModeAll => Loc.Localize("FashionReport_ModeAll", "All");
    public string ModeSingleHelp => Loc.Localize("FashionReport_ModeSingleHelp", "Only notify if no allowances have been spent this week and Fashion Report is available for turn-in");
    public string Mode80PlusHelp => Loc.Localize("FashionReport_Mode80PlusHelp", "Notify if any allowances remain this week, the highest score is below 80 and Fashion Report is available for turn-in");
    public string ModeAllHelp => Loc.Localize("FashionReport_ModeAllHelp", "Notify if any allowances remain this week and fashion report is available for turn-in");
    public string ClickableLinkLabel => Loc.Localize("FashionReport_ClickableLinkLabel", "Clickable Link");
    public string ClickableLink => Loc.Localize("FashionReport_ClickableLink", "Notifications can be clicked on to teleport to the Gold Saucer");
    public string AllowancesAvailable => Loc.Localize("FashionReport_AllowancesAvailable", "Allowances Available");
    public string HighestScore => Loc.Localize("FashionReport_HighestScore", "Highest Score");
    public string ReportOpen => Loc.Localize("FashionReport_ReportOpen", "Report Open");
    public string AvailableNow => Loc.Localize("FashionReport_AvailableNow", "Available Now");

}

public class HuntMarks
{
    public string LevelOne => Loc.Localize("HuntMarks_LevelOne", "Level One");
    public string LevelTwo => Loc.Localize("HuntMarks_LevelTwo", "Level Two");
    public string LevelThree => Loc.Localize("HuntMarks_LevelThree", "Level Three");
    public string Elite => Loc.Localize("HuntMarks_Elite", "Elite");
    public string DailyLabel => Loc.Localize("HuntMarks_DailyLabel", "Hunt Marks (Daily)");
    public string WeeklyLabel => Loc.Localize("HuntMarks_WeeklyLabel", "Hunt Marks (Weekly)");
    public string TrackedHunts => Loc.Localize("HuntMarks_TrackedHunts", "Tracked Hunts");
    public string Obtained => Loc.Localize("HuntMarks_Obtained", "Obtained");
    public string Unobtained => Loc.Localize("HuntMarks_Unobtained", "Unobtained");
    public string Killed => Loc.Localize("HuntMarks_Killed", "Killed");
    public string HuntsRemaining => Loc.Localize("HuntMarks_HuntsRemaining", "Hunts Remaining");
    public string TrackedHuntsStatus => Loc.Localize("HuntMarks_TrackedHuntsStatus", "Tracked Hunts Status");
    public string NoHuntsTracked => Loc.Localize("HuntMarks_NoHuntsTracked", "No hunts have been selected for tracking");
    public string ForceComplete => Loc.Localize("HuntMarks_ForceComplete", "Force Complete");
    public string ForceCompleteHelp => Loc.Localize("HuntMarks_ForceCompleteMouseover", "Override module status and mark complete\nHold Shift + Control while clicking");
    public string NoUndo => Loc.Localize("HuntMarks_NoUndo", "This action cannot be undone for this week");
}

public class JumboCactpot
{
    public string Label => Loc.Localize("JumboCactpot_Label", "Jumbo Cactpot");
    public string Tickets => Loc.Localize("JumboCactpot_Tickets", "Tickets");
    public string NextDrawing => Loc.Localize("JumboCactpot_NextDrawing", "Next Drawing");
    public string ClickableLink => Loc.Localize("JumboCactpot_ClickableLink", "Notifications can be clicked on to teleport to the Gold Saucer");
    public string ClickableLinkLabel => Loc.Localize("JumboCactpot_ClickableLinkLabel", "Clickable Link");
    public string TicketsAvailable => Loc.Localize("JumboCactpot_TicketsAvailable", "Tickets Available");
    public string NoTickets => Loc.Localize("JumboCactpot_NoTickets", "No Tickets");
}

public class Levequest
{
    public string Label => Loc.Localize("Levequest_Label", "Levequest");
    public string Accepted => Loc.Localize("Levequest_Accepted", "Accepted");
    public string NextAllowance => Loc.Localize("Levequest_NextAllowance", "Next Allowance");
    public string AllowancesRemaining => Loc.Localize("Levequest_AllowancesRemaining", "Allowances Remaining");
}

public class MiniCactpot
{
    public string Label => Loc.Localize("MiniCactpot_Label", "Mini Cactpot");
    public string ClickableLink => Loc.Localize("MiniCactpot_ClickableLink", "Notifications can be clicked on to teleport to the Gold Saucer");
    public string ClickableLinkLabel => Loc.Localize("MiniCactpot_ClickableLinkLabel", "Clickable Link");
    public string TicketsRemaining => Loc.Localize("MiniCactpot_TicketsRemaining", "Tickets Remaining");
}

public class TreasureMap
{
    public string Label => Loc.Localize("TreasureMap_Label", "Treasure Map");
    public string MapAvailable => Loc.Localize("TreasureMap_MapAvailable", "Map Available");
    public string NextMap => Loc.Localize("TreasureMap_NextMap", "Next Map");
}

public class WondrousTails
{
    public string Label => Loc.Localize("WondrousTails_Label", "Wondrous Tails");
    public string UnavailableMessage => Loc.Localize("WondrousTails_UnavailableMessage", "This instance is available for a stamp if you re-roll it");
    public string UnavailableRerollMessage => Loc.Localize("WondrousTails_UnavailableRerollMessage", "You have {0} Re-Rolls Available");
    public string AvailableMessage => Loc.Localize("WondrousTails_AvailableMessage", "A stamp is already available for this instance");
    public string CompletableMessage => Loc.Localize("WondrousTails_CompletableMessage", "Completing this instance will reward you with a stamp");
    public string ClaimableMessage => Loc.Localize("WondrousTails_ClaimableMessage", "You can claim a stamp for the last instance");
    public string BookAvailable => Loc.Localize("WondrousTails_BookAvailable", "Book Available");
    public string DutyNotifications => Loc.Localize("WondrousTails_DutyNotifications", "Duty Start/End Notifications");
    public string UnclaimedBookNotifications => Loc.Localize("WondrousTails_UnclaimedBookNotifications", "Unclaimed Book Warning");
    public string Stamps => Loc.Localize("WondrousTails_Stamps", "Stamps");
    public string Overlay => Loc.Localize("WondrousTails_Overlay", "Duty Finder Overlay");
    public string ClickableLink => Loc.Localize("WondrousTails_ClickableLink", "Notifications can be clicked on to open wondrous tails book, or to teleport to Idyllshire if you don't have a book.");
    public string ClickableLinkLabel => Loc.Localize("WondrousTails_ClickableLinkLabel", "Clickable Link");
    public string ResendNotification => Loc.Localize("WondrousTails_ResendNotification", "Resend after leaving duty");
}

public class ChallengeLog
{
    public string Label => Loc.Localize("ChallengeLog_Label", "Challenge Log");
    public string Battle => Loc.Localize("ChallengeLog_Battle", "Battle");
    public string CommendationLabel => Loc.Localize("ChallengeLog_CommendationLabel", "'Exercising the Right' Warning");
    public string Commendations => Loc.Localize("ChallengeLog_Commendations", "Exercising the Right");
    public string CommendationsRemaining => Loc.Localize("ChallengeLog_CommendationWarning", "Commendations Remaining");
    public string DungeonRouletteLabel => Loc.Localize("ChallengeLog_DungeonRouletteLabel", "'Feeling Lucky' Warning");
    public string DungeonRoulette => Loc.Localize("ChallengeLog_DungeonRoulette", "Feeling Lucky");
    public string DungeonRoulettesRemaining => Loc.Localize("ChallengeLog_DungeonRoulettesRemaining", "Dungeon Roulettes Remaining");
    public string DungeonMasterLabel => Loc.Localize("ChallengeLog_DungeonMasterLabel", "'Dungeon Master' Warning");
    public string DungeonMaster => Loc.Localize("ChallengeLog_DungeonMaster", "Dungeon Master");
    public string DungeonMasterRemaining => Loc.Localize("ChallengeLog_DungeonMasterRemaining", "Dungeons Remaining");
}

public class Raids
{
    public string AllianceLabel => Loc.Localize("AllianceRaids_Label", "Raids (Alliance)");
    public string NormalLabel => Loc.Localize("NormalRaids_Label", "Raids (Normal)");
    public string TrackedNormalRaids => Loc.Localize("Raids_TrackedNormalRaids", "Tracked Raids");
    public string Drops => Loc.Localize("Raids_Drops", "Drops");
    public string Regenerate => Loc.Localize("Raids_Regenerate", "Reset Tracked Raids");
    public string RegenerateHelp => Loc.Localize("Raids_RegenerateHelp", "Reload duty information, and reset settings\nHold Shift + Control while clicking");
    public string NoRaidsTracked => Loc.Localize("Raids_NoRaidsTracked", "No raids have been selected for tracking");
    public string RaidsRemaining => Loc.Localize("Raids_RaidsRemaining", "Raids Remaining");
    public string RaidRemaining => Loc.Localize("Raid_RaidsRemaining", "Raid Remaining");
    public string ClickableLinkLabel => Loc.Localize("Raids_ClickableLinkLabel", "Clickable Link");
    public string ClickableLink => Loc.Localize("Raids_ClickableLink", "Notifications can be clicked on to open the Duty Finder");
}

public class FauxHollows
{
    public string Label => Loc.Localize("UnrealTrial_Label", "Faux Hollows");
    public string TrialAvailable => Loc.Localize("UnrealTrial_TrialAvailable", "Unreal Trial Available");
    public string ClickableLinkLabel => Loc.Localize("UnrealTrial_ClickableLinkLabel", "Clickable Link");
    public string ClickableLink => Loc.Localize("UnrealTrial_ClickableLink", "Notifications can be clicked on to open the Party Finder");
    public string RetellingHelp => Loc.Localize("UnrealTrial_IncludeRetelling", "Require completing retelling to mark module as complete");
    public string Retelling => Loc.Localize("UnrealTrial_Retelling", "Include Retelling");
    public string Completions => Loc.Localize("UnrealTrial_Completions", "Completions");
}

public class GrandCompany
{
    public string SupplyLabel => Loc.Localize("GrandCompany_SupplyLabel", "Grand Company (Supply)");
    public string ProvisioningLabel => Loc.Localize("GrandCompany_Provisioning", "Grand Company (Provision)");
    public string NoJobsTracked => Loc.Localize("GrandCompany_NoJobsTracked", "No jobs have been selected for tracking");
    public string TrackedJobs => Loc.Localize("GrandCompany_TrackedJobs", "Tracked Jobs");
    public string SupplyNotification => Loc.Localize("GrandCompany_SupplyNotification", "Allowances Available");
    public string ProvisionNotification => Loc.Localize("GrandCompany_SupplyNotification", "Allowances Available");
    public string NextReset => Loc.Localize("GrandCompany_NextReset", "Next Reset");
}

public class UserInterface
{
    public Todo Todo = new();
    public Timers Timers = new();
    public Teleport Teleport = new();
}

public class Todo
{
    public string Label => Loc.Localize("Todo_Label", "Todo");
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
    public string LockWindow => Loc.Localize("Todo_LockWindow", "Lock Window Position");
    public string AutoResize => Loc.Localize("Todo_AutoResize", "Auto Resize");
    public string AnchorCorner => Loc.Localize("Todo_AnchorCorner", "Anchor Corner");
    public string Opacity => Loc.Localize("Todo_Opacity", "Opacity");
    public string DailyTasks => Loc.Localize("Todo_DailyTasks", "Daily Tasks");
    public string WeeklyTasks => Loc.Localize("Todo_WeeklyTasks", "Weekly Tasks");
    public string NoTasksEnabled => Loc.Localize("Todo_NoDailyTasksEnabled", "Enable a module in Configuration to track Tasks");
    public string UseLongLabel => Loc.Localize("Todo_UseLongLabel", "Use Long Label");
    public string CompleteCategory => Loc.Localize("Todo_CompleteCategory", "Show Completed Categories");
    public string AllTasksComplete => Loc.Localize("Todo_AllTasksComplete", "All Tasks Completed");
}

public class Timers
{
    public string MainOptions => Loc.Localize("Timers_MainOptions", "Main Options");
    public string WindowOptions => Loc.Localize("Timers_WindowOptions", "Window Options");
    public string HideWindowInDuty => Loc.Localize("Timers_HideWindowInDuty", "Hide Window In Duty");
    public string LockWindow => Loc.Localize("Timers_LockWindow", "Lock Window Position");
    public string Opacity => Loc.Localize("Timers_Opacity", "Opacity");

    public string NoTimersEnabledWarning => Loc.Localize("Timers_NoneEnabledWarning",
        "Enable which timers you would like to see here with '/dd timers' command");

    public string Label => Loc.Localize("Timers_Label", "Timers");
    public string EditTimer => Loc.Localize("Timers_EditTimer", "Edit Style");
    public string EditTimerTitle => Loc.Localize("Timers_EditTimerTitle", "Edit Timer Style");
    public string AutoResize => Loc.Localize("Timers_AutoResize", "Auto Resize");
    public string TimeDisplay => Loc.Localize("Timers_TimeDisplay", "Time Display Options");
    public string ColorOptions => Loc.Localize("Timers_ColorOptions", "Color Options");
    public string Background => Loc.Localize("Timers_BackgroundColor", "Background");
    public string Foreground => Loc.Localize("Timers_ForegroundColor", "Foreground");
    public string Text => Loc.Localize("Timers_TextColor", "Text");
    public string Time => Loc.Localize("Timers_TimeColor", "Time");
    public string SizeOptions => Loc.Localize("Timers_SizeOptions", "Size Options");
    public string StretchToFit => Loc.Localize("Timers_StretchToFit", "Stretch to Fit");
    public string Size => Loc.Localize("Timers_Size", "Size");
    public string AvailableNow => Loc.Localize("Timers_AvailableNow", "Available Now");
    public string Name => Loc.Localize("Timers_Name", "Display Name");
    public string EnableCustomName => Loc.Localize("Timers_EnableCustomName", "Enable Custom Name");
    public string TextOptions => Loc.Localize("Timers_TextOptions", "Text Options");
    public string HideLabel => Loc.Localize("Timers_HideLabel", "Hide Label");
    public string HideTime => Loc.Localize("Timers_HideTime", "Hide Time");
    public string HideCompleted => Loc.Localize("Timers_HideCompleted", "Hide Completed Tasks");
    public string HumanStyle => Loc.Localize("Timers_HumanStyle", "Human (e.g. '3 hours')");
    public string FullStyle => Loc.Localize("Timers_FullStyle", "Full (D.HH:MM:SS)");
    public string NoSecondsStyle => Loc.Localize("Timers_NoSecondsStyle", "No Seconds (D.HH:MM)");
    public string NumDays => Loc.Localize("Timers_NumDays", "{0} days");
    public string DayPlusHours => Loc.Localize("Timers_DayPlusHours", "{0} day, {1} hours");
    public string NumHours => Loc.Localize("Timers_NumHours", "{0} hours");
    public string NumMins => Loc.Localize("Timers_NumMins", "{0} minutes");
    public string NumSecs => Loc.Localize("Timers_NumSecs", "{0} seconds");
    public string Reset => Loc.Localize("Timers_Reset", "Reset to Default");
}

public class Teleport
{
    public string Label => Loc.Localize("Teleport_Label", "Teleport");
    public string Error => Loc.Localize("Teleport_Error", "Cannot teleport in this situation");
    public string Teleporting => Loc.Localize("Teleport_Teleporting", "Teleporting to '{0}'");
    public string CommunicationError => Loc.Localize("Teleport_CommunicationError", "To use the teleport function, you must install the \"Teleporter\" plugin");
    public string NotUnlocked => Loc.Localize("Teleport_NotUnlocked", "Destination Aetheryte is not unlocked, teleport cancelled");
}
