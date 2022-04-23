using CheapLoc;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Localization
{
    internal static class Strings
    {
        public class CommonStrings
        {
            public readonly string InformationLabel = Loc.Localize("InformationLabel", "Information");
            public readonly string AutomationInformationLabel = Loc.Localize("AutomationInformationLabel", "Data Collection");
            public readonly string TechnicalInformationLabel = Loc.Localize("TechnicalInformationLabel", "Technical Information");
            public readonly string EnabledLabel = Loc.Localize("EnabledLabel", "Enabled");
            public readonly string EmptyContentsLabel = Loc.Localize("EmptyContentsLabel", "No information to display");
            public readonly string CurrentStatusLabel = Loc.Localize("CurrentStatusLabel", "Current Status");
            public readonly string CompletionStatusLabel = Loc.Localize("CompletionStatusLabel", "Completion Status");
            public readonly string NotificationOptionsLabel = Loc.Localize("NotificationOptionsLabel", "Notification Options");
            public readonly string ClickableLinkLabel = Loc.Localize("ClickableLinkLabel", "Clickable Link");

            public readonly string NotifyOnLoginLabel = Loc.Localize("NotifyOnLoginLabel", "Send Notifications on Login");
            public readonly string NotifyOnLoginHelpText = Loc.Localize("NotifyOnLoginHelpText", "If this module is incomplete or has relevant messages send a notification when you login to a character.");
            public readonly string NotifyOnZoneChangeLabel = Loc.Localize("NotifyOnZoneChangeLabel", "Send Notifications on Zone Change");
            public readonly string NotifyOnZoneChangeHelpText = Loc.Localize("NotifyOnZoneChangeHelpText", "If this module is incomplete or has relevant messages send a notification when you change zones.");

            public readonly string CompleteLabel = Loc.Localize("CompleteLabel", "Complete");
            public readonly string IncompleteLabel = Loc.Localize("IncompleteLabel", "Incomplete");
            public readonly string AllTasksLabel = Loc.Localize("AllTasksLabel", "All Tasks Status");
        }

        public class TabStrings
        {
            public readonly string FeaturesTabLabel = Loc.Localize("FeaturesTabLabel", "Features");
            public readonly string FeaturesTabDescription = Loc.Localize("FeaturesTabDescription", "Main features of DailyDuty to add QoL");
            public readonly string DebugTabLabel = Loc.Localize("DebugTabLabel", "Debug");
            public readonly string DebugTabDescription = Loc.Localize("DebugTabString", "Settings for debugging or showing more information");
            public readonly string SettingsTabLabel = Loc.Localize("SettingsTabLabel", "Settings");
            public readonly string SettingsTabDescription = Loc.Localize("SettingsTabDescription", "Settings specific to this window");
            public readonly string TasksTabLabel = Loc.Localize("TasksTabLabel", "Tasks");
            public readonly string TasksTabDescription = Loc.Localize("TasksTabDescription", "View and Configure Daily and Weekly tasks");
            public readonly string TimersTabLabel = Loc.Localize("TimersTabLabel", "Timers");
            public readonly string TimersTabDescription = Loc.Localize("TimersTabDescription", "Vew time until various daily and weekly tasks reset");
        }

        public class FeaturesStrings
        {
            public readonly string DutyRouletteDutyFinderOverlayLabel = Loc.Localize("DutyRouletteDutyFinderOverlayLabel", "Duty Roulette Duty Finder Overlay");
            public readonly string DutyRouletteDutyFinderOverlayAutomationInformation = Loc.Localize("DutyRouletteDutyFinderOverlayAutomationInformation", "The 'Duty Roulette' status data is gathered directly from internal game data and will always be precisely in sync with the game.");
            public readonly string DutyRouletteDutyFinderOverlayInformationDisclaimer = Loc.Localize("DutyRouletteDutyFinderOverlayInformationDisclaimer", "Requires Task Module 'Duty Roulette' to be enabled to function");
            public readonly string DutyRouletteDutyFinderOverlayDescription = Loc.Localize("DutyRouletteDutyFinderOverlayDescription", "Changes the colors of the duty roulette text in the Duty Finder to reflect the daily completion status.");
            public readonly string DutyRouletteDutyFinderOverlayTechnicalDescription = Loc.Localize("DutyRouletteDutyFinderOverlayTechnicalDescription", "Use the 'Duty Roulette' module to configure which roulettes are colored.\n\nYou can also set the 'complete' and 'incomplete' colors in the 'Duty Roulette' module\n\nRoulettes that are not tracked will not be colored regardless of their completion status.");
            
            public readonly string WondrousTailsDutyFinderOverlayLabel = Loc.Localize("WondrousTailsDutyFinderOverlayLabel", "Wondrous Tails Duty Finder Overlay");
            public readonly string WondrousTailsDutyFinderOverlayDescription = Loc.Localize("WondrousTailsDutyFinderOverlayDescription", "Adds a clover icon to duties in the Duty Finder if that duty is in your Wondrous Tails book.\n\nThe clover will be golden if that duty is able to be completed to reward you a stamp.\n\nThe clover will be hollow if that duty has already been claimed for a stamp.");
            public readonly string WondrousTailsDutyFinderOverlayAutomationInformation = Loc.Localize("WondrousTailsDutyFinderOverlayAutomationInformation", "The Wondrous Tails duty availability is read directly from game data.\n\nThe Duty Finder may need to be opened and then closed again to refresh the displayed data.");
            public readonly string WondrousTailsDutyFinderOverlayTechnicalDescription = Loc.Localize("WondrousTailsDutyFinderOverlayTechnicalDescription", "The game code does not clear the task data when you complete your book for the week. This will cause the Duty Finder to reflect the status of the last obtained book.");

        }

        public class ConfigurationStrings
        {
            public readonly string DailyDutySettingsLabel = Loc.Localize("DailyDutySettingsLabel", "DailyDuty Settings");
            public readonly string NoSelectionDescription = Loc.Localize("NoSelectionInformation", "Select an item to configure in the left pane");
            public readonly string AboutTabLabel = Loc.Localize("AboutTabLabel", "About");
            public readonly string OptionsTabLabel = Loc.Localize("OptionsTabLabel", "Options");
            public readonly string StatusTabLabel = Loc.Localize("StatusTabLabel", "Status");
            public readonly string LogTabLabel = Loc.Localize("LogTabLabel", "Log");
        }

        public class ModuleStrings
        {
            public readonly string DutyRouletteLabel = Loc.Localize("DutyRouletteLabel", "Duty Roulette");
            public readonly string DutyRouletteTrackedRoulettesLabel = Loc.Localize("DutyRouletteTrackedRoulettesLabel", "Select Roulettes to Track");
            public readonly string DutyRouletteInformation = Loc.Localize("DutyRouletteInformation", "The Duty Roulette is a daily task that awards experience and gil once a day per completion.\n\nAdditional rewards such as various tomestones are rewarded depending on roulette type and level of job you queue as.");
            public readonly string DutyRouletteAutomationInformation = Loc.Localize("DutyRouletteAutomationInformation", "This module is entirely automatic, no user interaction is required.\n\nThe completion status of each roulette is gathered direct from the game data.");
            public readonly string DutyRouletteTechnicalInformation = Loc.Localize("DutyRouletteTechnicalInformation", "You can configure this module to alert you when the specified duties have not yet been completed today.\n\nOny the duties selected for tracking in the Options tab will be considered for notifications.\n\nSome listed roulettes may not be available due to not being unlocked or due to being unavailable until a later patch. It is not recommended to track roulettes you do not have access to.");
            public readonly string DutyRouletteNothingTrackedDescriptionWarning = Loc.Localize("DutyRouletteNothingTrackedDescriptionWarning", "There are no roulettes tracked");
            public readonly string DutyRouletteNothingTrackedDescription = Loc.Localize("DutyRouletteNothingTrackedDescription", "Select roulettes to track in 'Options'");
            public readonly string DutyRouletteRoulettesRemaining = Loc.Localize("DutyRouletteRoulettesRemaining", "Roulettes Remaining");
            public readonly string DutyRouletteClickableLinkDescription = Loc.Localize("DutyRouletteClickableLinkDescription", "Notifications can be clicked on to open the Duty Finder.");
        }

        public static readonly TabStrings Tabs = new();
        public static readonly ConfigurationStrings Configuration = new();
        public static readonly FeaturesStrings Features = new();
        public static readonly CommonStrings Common = new();
        public static readonly ModuleStrings Module = new();
    }
}
