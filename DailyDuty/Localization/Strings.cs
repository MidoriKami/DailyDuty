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
            public readonly string AvailableNowLabel = Loc.Localize("AvailableNowLabel", "Available Now");

            public readonly string StyleOptionsLabel = Loc.Localize("StyleOptionsLabel", "Style Options");
            public readonly string OpacityLabel = Loc.Localize("OpacityLabel", "Opacity");

            public readonly string MinutesLabel = Loc.Localize("MinutesLabel", "Minutes");
            public readonly string SaveLabel = Loc.Localize("SaveLabel", "Save");
            public readonly string StyleLabel = Loc.Localize("StyleLabel", "Style");
            public readonly string AllowancesLabel = Loc.Localize("AllowancesLabel", "Allowances");
            public readonly string AllowancesRemainingLabel = Loc.Localize("AllowancesRemainingLabel", "Allowances Remaining");
            public readonly string LessThanLabel = Loc.Localize("LessThanLabel", "Less Than");
            public readonly string LessThanOrEqualLabel = Loc.Localize("LessThanOrEqualLabel", "Less Than or Equal To");
            public readonly string EqualToLabel = Loc.Localize("EqualToLabel", "Equal To");
            public readonly string NoneRecordedLabel = Loc.Localize("NoneRecordedLabel", "None Recorded");

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
            public readonly string NotificationsThrottleDescription = Loc.Localize("NotificationsThrottleDescription", "This setting controls how frequently Daily Duty is allowed to send you chat notifications.\n\nDefault: 5 Minutes");
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
            public readonly string DutyRouletteRoulettesRemainingSingular = Loc.Localize("DutyRouletteRoulettesRemainingSingular", "Roulette Remaining");
            public readonly string DutyRouletteClickableLinkDescription = Loc.Localize("DutyRouletteClickableLinkDescription", "Notifications can be clicked on to open the Duty Finder");

            public readonly string WondrousTailsNumStamps = Loc.Localize("WondrousTailsNumStamps", "Stamps"); 
            public readonly string WondrousTailsBookStatusLabel = Loc.Localize("WondrousTailsBookStatusLabel", "Book Status");
            public readonly string WondrousTailsLabel = Loc.Localize("WondrousTailsLabel", "Wondrous Tails");
            public readonly string WondrousTailsInformation = Loc.Localize("WondrousTailsInformation", "Wondrous Tails is a level 60 weekly activity that allows players to complete old content to earn rewards. With some randomized elements, Wondrous Tails is an interesting way to revisit old duties every week.\n\nWeekly rewards include half of a level worth's of experience to whichever job turns in the book, and selectable rewards such as tomestones or MGP vouchers.");
            public readonly string WondrousTailsAutomationInformation = Loc.Localize("WondrousTailsAutomationInformation", "Wondrous Tails data is extracted directly from game data, and will always be in sync with the current status of your book. No user interaction is required to keep data synchronized.");
            public readonly string WondrousTailsTechnicalDescription = Loc.Localize("WondrousTailsTechnicalDescription", "The game code does not clear the wondrous tails data when you complete your book even if you turn it in without 9 stamps.\n\nThis module will only consider a wondrous tails book complete once you have 9 stamps placed.");
            public readonly string WondrousTailsOpenBookClickableLinkDescription = Loc.Localize("WondrousTailsOpenBookClickableLinkDescription", "Notifications can be clicked on to open you Wondrous Tails book");
            public readonly string WondrousTailsUnavailableMessage = Loc.Localize("WondrousTailsUnavailableMessage", "This instance is available for a stamp if you re-roll it");
            public readonly string WondrousTailsUnavailableRerollMessage = Loc.Localize("WondrousTailsUnavailableRerollMessage", "You have {0} Re-Rolls Available");
            public readonly string WondrousTailsAvailableMessage = Loc.Localize("WondrousTailsAvailableMessage", "A stamp is already available for this instance");
            public readonly string WondrousTailsCompletableMessage = Loc.Localize("WondrousTailsCompletableMessage", "Completing this instance will reward you with a stamp");
            public readonly string WondrousTailsClaimableMessage = Loc.Localize("WondrousTailsClaimableMessage", "You can claim a stamp for the last instance");
            public readonly string WondrousTailsInstanceNotificationsLabel = Loc.Localize("WondrousTailsInstanceNotificationsLabel", "Duty Start/End Notification");
            public readonly string WondrousTailsInstanceNotificationsDescription = Loc.Localize("WondrousTailsInstanceNotificationsDescription", "Send notifications at the start of a duty if the duty is a part of your Wondrous Tails book\nAdditionally, sends notifications after completing a duty to remind you to collect your stamp");

            public readonly string TreasureMapLabel = Loc.Localize("TreasureMapLabel", "Treasure Map");
            public readonly string TreasureMapInformation = Loc.Localize("TreasureMapInformation", "Treasure Maps are items that can be gathered by Disciples of the Land once every 18 hours. These maps can then be opened as part of a mini-event for various rewards. Alternatively, these maps can be sold on the market board.\n\nYou can only hold one of each type of map at a time.");
            public readonly string TreasureMapAutomationInformation = Loc.Localize("TreasureMapAutomationInformation", "Treasure map data is gathered by reading chat information when the treasure map is gathered. If DailyDuty was installed after you collected a map, you must wait until you are able to collect a map for tracking to begin.");
            public readonly string TreasureMapAvailableMessage = Loc.Localize("TreasureMapAvailableMessage", "Treasure Map Available");
            public readonly string TreasureMapTimeUntilNextMap = Loc.Localize("TreasureMapTimeUntilNextMap", "Next Map");
            public readonly string TreasureMapStatusLabel = Loc.Localize("TreasureMapStatusLabel", "Treasure Map Status");

            public readonly string BeastTribeLabel = Loc.Localize("BeastTribeLabel", "Beast Tribe");
            public readonly string BeastTribeInformation = Loc.Localize("BeastTribeInformation", "Beast Tribes are a daily task that can be completed for experience rewards and to earn progress towards beast tribes affiliations unlocking unique dyes, glamours, mounts, and housing items.");
            public readonly string BeastTribeAutomationInformation = Loc.Localize("BeastTribeAutomationInformation", "Beast tribe allowance information is extracted directly from game data and will always be in sync with the current status of the game.");
            public readonly string BeastTribeTechnicalInformation = Loc.Localize("BeastTribeTechnicalInformation", "DailyDuty provides options to alert you when you are at a threshold of allowances, either by directly imputing the threshold, or by specifying how many beast tribes-worth of allowances you wish to track.\n\nUpon reaching a new tier in each beast tribe you get 3 additional allowances for that tribe, DailyDuty is unable to track this information and will not consider the extra allowances for tracking purposes.");
            public readonly string BeastTribeAllowancesRemainingLabel = Loc.Localize("BeastTribeAllowancesRemainingLabel", "Above Allowance Threshold");
            public readonly string BeastTribeMarkCompleteWhenLabel = Loc.Localize("BeastTribeMarkCompleteWhenLabel", "Mark Complete When");

            public readonly string LevequestLabel = Loc.Localize("LevequestLabel", "Levequest");
            public readonly string LevequestInformation = Loc.Localize("LevequestInformation", "Levequests are repeatable quests players can perform to earn experience, gil and items. Unlike regular quests, which can only be performed once per character, levequests can be performed again and again as long as the player has Leve allowances.");
            public readonly string LevequestAutomationInformation = Loc.Localize("LevequestAutomationInformation", "Levequest information is extracted directly from game data and will always be in sync with the current status of the game.");
            public readonly string LevequestTechnicalInformation = Loc.Localize("LevequestTechnicalInformation", "Once a levequest is accepted the allowance is spent, if you then cancel the levequest the allowance is not returned.");
            public readonly string LevequestAboveThresholdLabel = Loc.Localize("LevequestAboveThresholdLabel", "Above Allowance Threshold");
            public readonly string LevequestAcceptedLabel = Loc.Localize("LevequestAcceptedLabel", "Accepted");
            public readonly string NextAllowanceLabel = Loc.Localize("NextAllowanceLabel", "Next Allowances");

            public readonly string MiniCactpotLabel = Loc.Localize("MiniCactpotLabel", "Mini Cactpot");
            public readonly string MiniCactpotInformation = Loc.Localize("MiniCactpotInformation", "Mini Cactpot is a daily scratch-card like activity you can do three times per day. Each mini cactpot ticket awards a various amount of MGP up to 10,000 MGP per ticket.");
            public readonly string MiniCactpotAutomationInformation = Loc.Localize("MiniCactpotAutomationInformation", "Tracking is done by monitoring the opening of the Mini Cactpot ticket. If DailyDuty was installed after you did your daily Mini Cactpot, it will not be tracked.");
            public readonly string MiniCactpotTechnicalInformation = Loc.Localize("MiniCactpotTechnicalInformation", "The game removes your Mini Cactpot ticket allowance the moment you spend the MGP to purchase the ticket, and the window is opened, if your game is closed or crashes after the ticket window opened, that ticket will be lost.");
            public readonly string MiniCactpotTicketsRemainingLabel = Loc.Localize("MiniCactpotTicketsRemainingLabel", "Tickets Remaining");

            public readonly string CustomDeliveryLabel = Loc.Localize("CustomDeliveryLabel", "Custom Delivery");
            public readonly string CustomDeliveryInformation = Loc.Localize("CustomDeliveryInformation", "Custom Deliveries is a weekly DoH/DoL task you can complete for various crafters or gatherers scripts, exp rewards, various materia, folklore tomes, and more.");
            public readonly string CustomDeliveryAutomationInformation = Loc.Localize("CustomDeliveryAutomationInformation", "Custom Delivery information is gathered directly from game data, and will always be in sync with the current status of the game.");

            public readonly string DomanEnclaveLabel = Loc.Localize("DomanEnclaveLabel", "Doman Enclave");
            public readonly string DomanEnclaveInformation = Loc.Localize("DomanEnclaveInformation", "The Doman Enclave is a weekly task you can complete for gil rewards. You donate junk items in exchange for a multiple of their value in gil back, resulting in profit. The reward multiplier increases higher the more you progress the Doman Enclave.");
            public readonly string DomanEnclaveAutomationInformation = Loc.Localize("DomanEnclaveAutomationInformation", "Doman Enclave data is tracked automatically, but only while you are actually in the Doman Enclave area.");
            public readonly string DomanEnclaveTechnicalInformation = Loc.Localize("DomanEnclaveTechnicalInformation", "Once you visit the Doman Enclave, Daily Duty attempts to keep a copy of the status, and reset that status at the start of a new week. If the data somehow becomes out of sync, simply visit the Doman Enclave and it will automatically re-sync.");
            public readonly string DomanEnclaveGilRemainingLabel = Loc.Localize("DomanEnclaveGilRemainingLabel", "gil Remaining");
            public readonly string DomanEnclaveBudgetRemainingLabel = Loc.Localize("DomanEnclaveBudgetRemainingLabel", "Budget Remaining");
            public readonly string DomanEnclaveCurrentAllowanceLabel = Loc.Localize("DomanEnclaveCurrentAllowanceLabel", "Current Allowance");

            public readonly string FashionReportLabel = Loc.Localize("FashionReportLabel", "Fashion Report");
            public readonly string FashionReportInformation = Loc.Localize("FashionReportInformation", "Fashion Report is a weekly task you can complete for MGP rewards. Simply 'Present yourself for judging' once a week without any effort will reward you 10,000 MGP. You can attempt to create an outfit for a higher score for additional MGP rewards and titles.");
            public readonly string FashionReportAutomationInformation = Loc.Localize("FashionReportAutomationInformation", "Fashion Report data is collected when you present yourself for judging, the current high score and allowances remaining is extracted from the UI when the judging is complete.");
            public readonly string FashionReportTechnicalInformation = Loc.Localize("FashionReportTechnicalInformation", "It is not possible to update the fashion report data outside of judging due to the strange behavior of the data only being available if you speak to the NPC a second time after presenting for judging.\n\nThe Fashion Report status will show as 'Complete' while Fashion Report is unavailable, and 'Incomplete' once it is available if the completion conditions are not met.");
            public readonly string FashionReportHighestScoreLabel = Loc.Localize("FashionReportHighestScoreLabel", "Highest Score");
            public readonly string FashionReportAvailableLabel = Loc.Localize("FashionReportAvailableLabel", "Report Opens");

            public readonly string JumboCactpotLabel = Loc.Localize("JumboCactpotLabel", "Jumbo Cactpot");
            public readonly string JumboCactpotInformation = Loc.Localize("JumboCactpotInformation", "Jumbo Cactpot is a weekly task where you can purchase 3 lottery tickets. These tickets allow you to pick any 4-digit number you wish, and depending on how many numbers are matched at the drawing the following week, you can earn MGP.");
            public readonly string JumboCactpotAutomationInformation = Loc.Localize("JumboCactpotAutomationInformation", "Jumbo Cactpot data is collected when you purchase your tickets. If DailyDuty was not installed when you purchased your tickets it will be unable to track them for that week.");
            public readonly string JumboCactpotTechnicalInformation = Loc.Localize("JumboCactpotTechnicalInformation", "While DailyDuty displays the value of the tickets collected, the only information needed to track the status is how many tickets have been purchased.\n\nUpon weekly reset the stored tickets will be removed and you will be prompted to purchase new tickets, if you have any pending rewards you will have to collect them to be able to collect your new tickets.");
            public readonly string JumboCactpotTicketsLabel = Loc.Localize("JumboCactpotTicketsLabel", "Tickets");
            public readonly string JumboCactpotTicketsAvailableLabel = Loc.Localize("JumboCactpotTicketsAvailableLabel", "Tickets Available");
            public readonly string JumboCactpotTicketAvailableLabel = Loc.Localize("JumboCactpotTicketAvailableLabel", "Ticket Available");


        }

        public class TimersStrings
        {
            public readonly string TimersLabel = Loc.Localize("TimersLabel", "Timers");

            public readonly string DailyResetLabel = Loc.Localize("DailyResetTimerLabel", "Daily Reset");
            public readonly string FashionReportLabel = Loc.Localize("FashionReportTimerLabel", "Weekly Fashion Report");
            public readonly string JumboCactpotLabel = Loc.Localize("JumboCactpotTimerLabel", "Weekly Jumbo Cactpot");
            public readonly string LeveAllowanceLabel = Loc.Localize("LeveAllowanceTimerLabel", "Daily Leve Allowance");
            public readonly string TreasureMapLabel = Loc.Localize("TreasureMapTimerLabel", "Daily Treasure Map");
            public readonly string WeeklyResetLabel = Loc.Localize("WeeklyResetTimerLabel", "Weekly Reset");

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
