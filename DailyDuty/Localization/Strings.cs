using CheapLoc;

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

            public readonly string StyleOptionsLabel = Loc.Localize("StyleOptionsLabel", "Style Options");
            public readonly string OpacityLabel = Loc.Localize("OpacityLabel", "Opacity");

            public readonly string MinutesLabel = Loc.Localize("MinutesLabel", "Minutes");
            public readonly string SaveLabel = Loc.Localize("SaveLabel", "Save");
            public readonly string StyleLabel = Loc.Localize("StyleLabel", "Style");

            public readonly string RequiresTeleporterPluginDescription = Loc.Localize("RequiresTeleporterPluginDescription", "This feature requires the plugin 'Teleporter' to be installed to function.");
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
            public readonly string TasksTabDescription = Loc.Localize("TasksTabDescription", "View and configure daily and weekly tasks");
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
            public readonly string DailyDutySettingsLabel = Loc.Localize("DailyDutySettingsLabel", "DailyDuty");
            public readonly string NoSelectionDescription = Loc.Localize("NoSelectionInformation", "Select an item to configure in the left pane");
            public readonly string AboutTabLabel = Loc.Localize("AboutTabLabel", "About");
            public readonly string OptionsTabLabel = Loc.Localize("OptionsTabLabel", "Options");
            public readonly string StatusTabLabel = Loc.Localize("StatusTabLabel", "Status");
            public readonly string LogTabLabel = Loc.Localize("LogTabLabel", "Log");
            public readonly string NotificationsConfigurationLabel = Loc.Localize("NotificationsConfigurationLabel", "Notifications");
            public readonly string NotificationsThrottleLabel = Loc.Localize("NotificationsThrottleLabel", "Notifications Throttle");
            public readonly string NotificationsThrottleDescription = Loc.Localize("NotificationsThrottleDescription", "This setting controls how frequently Daily Duty is allowed to send you chat notifications.\n\nDefault: 5 Minutes.");
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
            public readonly string DutyRouletteRoulettesRemaining = Loc.Localize("DutyRouletteRoulettesRemaining", "roulettes remaining");
            public readonly string DutyRouletteRoulettesRemainingSingular = Loc.Localize("DutyRouletteRoulettesRemainingSingular", "roulette remaining");
            public readonly string DutyRouletteClickableLinkDescription = Loc.Localize("DutyRouletteClickableLinkDescription", "Notifications can be clicked on to open the Duty Finder");

            public readonly string WondrousTailsNumStamps = Loc.Localize("WondrousTailsNumStamps", "Stamps"); 
            public readonly string WondrousTailsBookStatusLabel = Loc.Localize("WondrousTailsBookStatusLabel", "Book Status");
            public readonly string WondrousTailsLabel = Loc.Localize("WondrousTailsLabel", "Wondrous Tails");
            public readonly string WondrousTailsInformation = Loc.Localize("WondrousTailsInformation", "Wondrous Tails is a level 60 weekly activity that allows players to complete old content to earn rewards. With some randomized elements, Wondrous Tails is an interesting way to revisit old duties every week.\n\nWeekly rewards include half of a level worth's of experience to whichever job turns in the book, and selectable rewards such as tomestones or MGP vouchers.");
            public readonly string WondrousTailsAutomationInformation = Loc.Localize("WondrousTailsAutomationInformation", "Wondrous Tails data is extracted directly from game data, and will always be in sync with the current status of your book. No user interaction is required to keep data synchronized.");
            public readonly string WondrousTailsTechnicalDescription = Loc.Localize("WondrousTailsTechnicalDescription", "The game code does not clear the wondrous tails data when you complete your book even if you turn it in without 9 stamps.\n\nThis module will only consider a wondrous tails book complete once you have 9 stamps placed.");
            public readonly string WondrousTailsOpenBookClickableLinkDescription = Loc.Localize("WondrousTailsOpenBookClickableLinkDescription", "Notifications can be clicked on to open you Wondrous Tails book");
            public readonly string WondrousTailsUnavailableMessage = Loc.Localize("WondrousTailsUnavailableMessage", "This instance is available for a stamp if you re-roll it");
            public readonly string WondrousTailsUnavailableRerollMessage = Loc.Localize("", "You have {0} Re-Rolls Available");
            public readonly string WondrousTailsAvailableMessage = Loc.Localize("WondrousTailsAvailableMessage", "A stamp is already available for this instance");
            public readonly string WondrousTailsCompletableMessage = Loc.Localize("WondrousTailsCompletableMessage", "Completing this instance will reward you with a stamp");
            public readonly string WondrousTailsClaimableMessage = Loc.Localize("WondrousTailsClaimableMessage", "You can claim a stamp for the last instance");
            public readonly string WondrousTailsInstanceNotificationsLabel = Loc.Localize("WondrousTailsInstanceNotificationsLabel", "Duty Start/End Notification");
            public readonly string WondrousTailsInstanceNotificationsDescription = Loc.Localize("WondrousTailsInstanceNotificationsDescription", "Send notifications at the start of a duty if the duty is a part of your Wondrous Tails book\nAdditionally, sends notifications after completing a duty to remind you to collect your stamp");

            public readonly string TreasureMapLabel = Loc.Localize("TreasureMapLabel", "Treasure Map");
            public readonly string TreasureMapInformation = Loc.Localize("TreasureMapInformation", "Treasure Maps are items that can be gathered by Disciples of the Land once every 18 hours. These maps can then be opened as part of a mini-event for various rewards. Alternatively, these maps can be sold on the marketboard.\n\nYou can only hold one of each type of map at a time.");
            public readonly string TreasureMapAutomationInformation = Loc.Localize("TreasureMapAutomationInformation", "Treasure map data is gathered by reading chat information when the treasure map is gathered. If DailyDuty was installed after you collected a map, you must wait until you are able to collect a map for tracking to begin.");
            public readonly string TreasureMapAvailableMessage = Loc.Localize("TreasureMapAvailableMessage", "treasure map available");
            public readonly string TreasureMapTimeUntilNextMap = Loc.Localize("TreasureMapTimeUntilNextMap", "Next Map");
            public readonly string TreasureMapStatusLabel = Loc.Localize("TreasureMapStatusLabel", "Treasure Map Status");
        }

        public class TimersStrings
        {
            public readonly string TimersLabel = Loc.Localize("TimersLabel", "Timers");

            public readonly string DailyResetLabel = Loc.Localize("DailyResetLabel", "Daily Reset");
            public readonly string FashionReportLabel = Loc.Localize("FashionReportLabel", "Weekly Fashion Report");
            public readonly string JumboCactpotLabel = Loc.Localize("JumboCactpotLabel", "Weekly Jumbo Cactpot");
            public readonly string LeveAllowanceLabel = Loc.Localize("LeveAllowanceLabel", "Daily Leve Allowance");
            public readonly string TreasureMapLabel = Loc.Localize("TreasureMapLabel", "Daily Treasure Map");
            public readonly string WeeklyResetLabel = Loc.Localize("WeeklyResetLabel", "Weekly Reset");

            public readonly string TimersBarColorLabel = Loc.Localize("TimersBarColorLabel", "Progress bar color");
            public readonly string TimersHideSecondsLabel = Loc.Localize("TimersHideSecondsLabel", "Hide seconds");
        }

        public static readonly TabStrings Tabs = new();
        public static readonly ConfigurationStrings Configuration = new();
        public static readonly FeaturesStrings Features = new();
        public static readonly CommonStrings Common = new();
        public static readonly ModuleStrings Module = new();
        public static readonly TimersStrings Timers = new();
    }
}
