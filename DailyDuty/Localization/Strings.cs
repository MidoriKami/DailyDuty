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
            
            public readonly string TaskDisplayOptionsLabel = Loc.Localize("TaskDisplayOptionsLabel", "Task Display Options");
            public readonly string TaskHeaderColorLabel = Loc.Localize("TaskHeaderColorLabel", "Task Category Color");
            public readonly string TaskIncompleteTaskColorLabel = Loc.Localize("TaskIncompleteTaskColorLabel", "Incomplete Task Color");
            public readonly string TaskCompleteTaskColorLabel = Loc.Localize("TaskCompleteTaskColorLabel", "Complete Task Color");

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
            public readonly string ClickthroughLabel = Loc.Localize("ClickthroughLabel", "Clickthrough");

            public readonly string MinutesLabel = Loc.Localize("MinutesLabel", "Minutes");
            public readonly string StyleLabel = Loc.Localize("StyleLabel", "Style");
            public readonly string AllowancesLabel = Loc.Localize("AllowancesLabel", "Allowances");
            public readonly string LessThanLabel = Loc.Localize("LessThanLabel", "Less Than");
            public readonly string LessThanOrEqualLabel = Loc.Localize("LessThanOrEqualLabel", "Less Than or Equal To");
            public readonly string EqualToLabel = Loc.Localize("EqualToLabel", "Equal To");
            public readonly string NoneRecordedLabel = Loc.Localize("NoneRecordedLabel", "None Recorded");

            public readonly string RealmRebornLabel = Loc.Localize("RealmRebornLabel", "A Realm Reborn");
            public readonly string HeavenswardLabel = Loc.Localize("HeavenwardLabel", "Heavensward");
            public readonly string StormbloodLabel = Loc.Localize("StormbloodLabel", "Stormblood");
            public readonly string ShadowbringersLabel = Loc.Localize("ShadowbringersLabel", "Shadowbringers");
            public readonly string EndwalkerLabel = Loc.Localize("EndwalkerLabel", "Endwalker");

            public readonly string WeeklyTasksLabel = Loc.Localize("WeeklyTasksLabel", "Weekly Tasks");
            public readonly string DailyTasksLabel = Loc.Localize("DailyTasksLabel", "Daily Tasks");

            public readonly string TopLeftLabel = Loc.Localize("TopLeftLabel", "Top Left");
            public readonly string TopRightLabel = Loc.Localize("TopRightLabel", "Top Right");
            public readonly string BottomLeftLabel = Loc.Localize("BottomLeftLabel", "Bottom Left");
            public readonly string BottomRightLabel = Loc.Localize("BottomRightLabel", "Bottom Right");
            public readonly string AutoResizeLabel = Loc.Localize("AutoResizeLabel", "Auto Resize");
            public readonly string ManualSizeLabel = Loc.Localize("ManualSizeLabel", "Manual Size");

            public readonly string LevelOneLabel = Loc.Localize("LevelOneLabel", "Level 1");
            public readonly string LevelTwoLabel = Loc.Localize("LevelTwoLabel", "Level 2");
            public readonly string LevelThreeLabel = Loc.Localize("LevelThreeLabel", "Level 3");
            public readonly string EliteLabel = Loc.Localize("EliteLabel", "Elite");

            public readonly string ApplyAllLabel = Loc.Localize("ApplyAllLabel", "Apply All");

            public readonly string TeleportLabel = Loc.Localize("TeleportLabel", "Teleport");

            public readonly string ResetLabel = Loc.Localize("ResetLabel", "Reset");
            public readonly string ResetAllLabel = Loc.Localize("ResetAllLabel", "Reset All");
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
            public readonly string TimersTabDescription = Loc.Localize("TimersTabDescription", "View time until various daily and weekly tasks reset");
        }

        public class FeaturesStrings
        {
            public readonly string DutyRouletteDutyFinderOverlayLabel = Loc.Localize("DutyRouletteDutyFinderOverlayLabel", "Duty Roulette Duty Finder Overlay");
            public readonly string DutyRouletteDutyFinderOverlayAutomationInformation = Loc.Localize("DutyRouletteDutyFinderOverlayAutomationInformation", "The 'Duty Roulette' status is gathered directly from internal game data and will always be precisely in sync with the game.");
            public readonly string DutyRouletteDutyFinderOverlayInformationDisclaimer = Loc.Localize("DutyRouletteDutyFinderOverlayInformationDisclaimer", "Requires task module 'Duty Roulette' to be enabled to function");
            public readonly string DutyRouletteDutyFinderOverlayDescription = Loc.Localize("DutyRouletteDutyFinderOverlayDescription", "Changes the colors of the 'Duty Roulette' text in the Duty Finder to reflect the daily completion status.");
            public readonly string DutyRouletteDutyFinderOverlayTechnicalDescription = Loc.Localize("DutyRouletteDutyFinderOverlayTechnicalDescription", "Use the 'Duty Roulette' module to configure which roulettes are colored.\n\nYou can also set the 'complete' and 'incomplete' colors in the 'Duty Roulette' module\n\nRoulettes that are not tracked will not be colored, regardless of their completion status.");

            public readonly string WondrousTailsDutyFinderOverlayLabel = Loc.Localize("WondrousTailsDutyFinderOverlayLabel", "Wondrous Tails Duty Finder Overlay");
            public readonly string WondrousTailsDutyFinderOverlayDescription = Loc.Localize("WondrousTailsDutyFinderOverlayDescription", "Adds a clover icon next to the duties in the Duty Finder, if that duty is in your Wondrous Tails book.\n\nThe clover will be golden if that duty is able to be completed to reward you a stamp.\n\nThe clover will be hollow if that duty has already been claimed for a stamp.");
            public readonly string WondrousTailsDutyFinderOverlayAutomationInformation = Loc.Localize("WondrousTailsDutyFinderOverlayAutomationInformation", "The Wondrous Tails duties are read directly from game data.\n\nThe Duty Finder may need to be opened and closed again to refresh the displayed data.");
            public readonly string WondrousTailsDutyFinderOverlayTechnicalDescription = Loc.Localize("WondrousTailsDutyFinderOverlayTechnicalDescription", "The game code will not clear the task's data when you complete your book for the week. This will cause the Duty Finder to reflect the status of the last obtained book.");

            public readonly string TodoWindowLabel = Loc.Localize("TodoWindowLabel", "Todo Window");
            public readonly string TodoWindowTaskSelectionLabel = Loc.Localize("TodoWindowTaskSelectionLabel", "Task Selection");
            public readonly string TodoWindowShowDailyLabel = Loc.Localize("TodoWindowShowDailyLabel", "Show Daily Tasks");
            public readonly string TodoWindowShowWeeklyLabel = Loc.Localize("TodoWindowShowWeeklyLabel", "Show Weekly Tasks");
            public readonly string TodoWindowHideWindowWhenLabel = Loc.Localize("TodoWindowHideWindowWhenLabel", "Hide Window When");
            public readonly string TodoWindowHideWhenCompleteLabel = Loc.Localize("TodoWindowHideWhenCompleteLabel", "All Tasks are Complete");
            public readonly string TodoWindowShowWhenCompleteLabel = Loc.Localize("TodoWindowShowWhenCompleteLabel", "Show Completed Tasks");
            public readonly string TodoWindowHideInDutyLabel = Loc.Localize("TodoWindowHideInDutyLabel", "In a Duty");
            public readonly string TodoWindowResizeStyleLabel = Loc.Localize("TodoWindowResizeStyleLabel", "Window Resize Options");
            public readonly string TodoWindowInformation = Loc.Localize("TodoWindowInformation", "The Todo window is a small window that shows your tracked tasks. This window is helpful for figuring out what you still need to do at a quick glance.");
            public readonly string TodoWindowEnableExpandedInfo = Loc.Localize("TodoWindowEnableExpandedInfo", "Expanded Todo Display");

            public readonly string TimersWindowEnableTimersWarning = Loc.Localize("TimersWindowEnableTimersWarning", "Enable which timers you would like to see here in 'Timers Window Configuration'");
            public readonly string TimersWindowLabel = Loc.Localize("TimersWindowLabel", "Timers Window");
            public readonly string TimersWindowInformation = Loc.Localize("TimersWindowInformation", "The timers window allows you to view the time until various resets as a handy user interface element.");
            public readonly string TimersWindowTimersEnableLabel = Loc.Localize("TimersWindowTimersEnableLabel", "Enable Timers");
            public readonly string TimersWindowForegroundColorLabel = Loc.Localize("TimersWindowForegroundColorLabel", "Foreground Color");
            public readonly string TimersWindowBackgroundColorLabel = Loc.Localize("TimersWindowBackgroundColorLabel", "Background Color");
            public readonly string TimersWindowTextColorLabel = Loc.Localize("TimersWindowTextColorLabel", "Text Color");
            public readonly string TimersWindowTimeColorLabel = Loc.Localize("TimersWindowTimeColorLabel", "Time Color");
            public readonly string TimersWindowFitToWindowLabel = Loc.Localize("TimersWindowFitToWindowLabel", "Fit to Window");
            public readonly string TimersWindowSizeLabel = Loc.Localize("TimersWindowSizeLabel", "Size");

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
            public readonly string NotificationsThrottleDescription = Loc.Localize("NotificationsThrottleDescription", "This setting controls the frequency of chat notifications from DailyDuty.\n\nDefault: 5 Minutes");
            public readonly string NotificationsDelayLabel = Loc.Localize("NotificationsDelayLabel", "Notifications Delay");
            public readonly string NotificationsDelayDescription = Loc.Localize("NotificationsDelayDescription", "Prevents notifications from showing until after the selected weekday. The start of the week is 'Tuesday'");
            public readonly string LanguageLabel = Loc.Localize("LanguageLabel", "Language");
            public readonly string LanguageSelectLabel = Loc.Localize("LanguageSelectLabel", "Language Select");
        }

        public class ModuleStrings
        {
            public readonly string DutyRouletteLabel = Loc.Localize("DutyRouletteLabel", "Duty Roulette");
            public readonly string DutyRouletteTrackedRoulettesLabel = Loc.Localize("DutyRouletteTrackedRoulettesLabel", "Select Roulettes to Track");
            public readonly string DutyRouletteInformation = Loc.Localize("DutyRouletteInformation", "The Duty Roulette is a daily task that awards experience and gil once a day per completion.\n\nAdditional rewards such as various tomestones and grand company seals are rewarded depending on roulette type and level of job you queue as.");
            public readonly string DutyRouletteAutomationInformation = Loc.Localize("DutyRouletteAutomationInformation", "This module is entirely automatic, no user interaction is required.\n\nThe completion status of each roulette is gathered directly from the game's data.");
            public readonly string DutyRouletteTechnicalInformation = Loc.Localize("DutyRouletteTechnicalInformation", "You can configure this module to alert you when specified duties have not yet been completed today.\n\nOnly the duties selected for tracking in the Options tab will give notifications.\n\nSome listed roulettes may not be available due to not being unlocked or due to being unavailable until a later patch. It is not recommended to track roulettes you do not have access to.");
            public readonly string DutyRouletteNothingTrackedDescriptionWarning = Loc.Localize("DutyRouletteNothingTrackedDescriptionWarning", "There are no roulettes tracked");
            public readonly string DutyRouletteNothingTrackedDescription = Loc.Localize("DutyRouletteNothingTrackedDescription", "Select roulettes to track in 'Options'");
            public readonly string DutyRouletteRoulettesRemaining = Loc.Localize("DutyRouletteRoulettesRemaining", "Roulettes Remaining");
            public readonly string DutyRouletteRoulettesRemainingSingular = Loc.Localize("DutyRouletteRoulettesRemainingSingular", "Roulette Remaining");
            public readonly string DutyRouletteClickableLinkDescription = Loc.Localize("DutyRouletteClickableLinkDescription", "Notifications can be clicked on to open the Duty Finder");
            public readonly string DutyRouletteExpandedInfo = Loc.Localize("DutyRouletteExpandedInfo", "Displays individual incomplete duties in the todo window.");

            public readonly string WondrousTailsNumStamps = Loc.Localize("WondrousTailsNumStamps", "Stamps"); 
            public readonly string WondrousTailsBookStatusLabel = Loc.Localize("WondrousTailsBookStatusLabel", "Book Status");
            public readonly string WondrousTailsLabel = Loc.Localize("WondrousTailsLabel", "Wondrous Tails");
            public readonly string WondrousTailsInformation = Loc.Localize("WondrousTailsInformation", "Wondrous Tails is a level 60 weekly activity that allows players to complete old content to earn rewards. With some randomized elements, Wondrous Tails is an interesting way to revisit these older duties every week.\n\nWeekly rewards include half of a level worth's of experience to whichever job turns in the book, and selectable rewards such as tomestones or MGP vouchers.");
            public readonly string WondrousTailsAutomationInformation = Loc.Localize("WondrousTailsAutomationInformation", "Wondrous Tails data is extracted directly from the game's data, and will always be in sync with the current status of your book. No user interaction is required to keep this data synchronized.");
            public readonly string WondrousTailsTechnicalDescription = Loc.Localize("WondrousTailsTechnicalDescription", "The game code will not clear the Wondrous Tails data when you complete your book, even if you turn it in without 9 stamps.\n\nThis module will only consider a Wondrous Tails book complete once you have 9 stamps placed.");
            public readonly string WondrousTailsOpenBookClickableLinkDescription = Loc.Localize("WondrousTailsOpenBookClickableLinkDescription", "Notifications can be clicked on to open your Wondrous Tails book");
            public readonly string WondrousTailsUnavailableMessage = Loc.Localize("WondrousTailsUnavailableMessage", "This instance is available for a stamp if you re-roll it");
            public readonly string WondrousTailsUnavailableRerollMessage = Loc.Localize("WondrousTailsUnavailableRerollMessage", "You have {0} Re-Rolls Available");
            public readonly string WondrousTailsAvailableMessage = Loc.Localize("WondrousTailsAvailableMessage", "A stamp is already available for this instance");
            public readonly string WondrousTailsCompletableMessage = Loc.Localize("WondrousTailsCompletableMessage", "Completing this instance will reward you with a stamp");
            public readonly string WondrousTailsClaimableMessage = Loc.Localize("WondrousTailsClaimableMessage", "You can claim a stamp for the last instance");
            public readonly string WondrousTailsInstanceNotificationsLabel = Loc.Localize("WondrousTailsInstanceNotificationsLabel", "Duty Start/End Notification");
            public readonly string WondrousTailsInstanceNotificationsDescription = Loc.Localize("WondrousTailsInstanceNotificationsDescription", "Send notifications at the start of a duty, if that duty is a part of your Wondrous Tails book\nAdditionally, it will send notifications after completing a duty, reminding you to collect your stamp");
            public readonly string WondrousTailsStickerAvailableNotificationLabel = Loc.Localize("WondrousTailsStickerAvailableNotificationLabel", "Sticker Available Warning");
            public readonly string WondrousTailsStickerAvailableNotificationDescription = Loc.Localize("WondrousTailsStickerAvailableNotificationDescription", "Send a notification on zone change if any tasks in your book are currently available to claim a sticker.");
            public readonly string WondrousTailsStickersAvailableNotification = Loc.Localize("WondrousTailsStickersAvailableNotification", "Stickers Available");

            public readonly string TreasureMapLabel = Loc.Localize("TreasureMapLabel", "Treasure Map");
            public readonly string TreasureMapInformation = Loc.Localize("TreasureMapInformation", "Treasure Maps are gatherable items with the Disciples of Land jobs once every 18 hours. These maps can be opened as part of a mini-event for various rewards. Alternatively, these maps can be sold on the market board.\n\nYou can only hold one of each type of map at a time.");
            public readonly string TreasureMapAutomationInformation = Loc.Localize("TreasureMapAutomationInformation", "Treasure map data is gathered by reading chat information when the treasure map is gathered. If DailyDuty was installed after you have collected a map, you must wait until you are able to collect another map for the tracking to begin.");
            public readonly string TreasureMapAvailableMessage = Loc.Localize("TreasureMapAvailableMessage", "Treasure Map Available");
            public readonly string TreasureMapTimeUntilNextMap = Loc.Localize("TreasureMapTimeUntilNextMap", "Next Map");
            public readonly string TreasureMapStatusLabel = Loc.Localize("TreasureMapStatusLabel", "Treasure Map Status");
            public readonly string TreasureMapReSyncInformation = Loc.Localize("TreasureMapReSyncInformation", "You can force a re-sync of Treasure Map by opening the 'Timers' menu");

            public readonly string BeastTribeLabel = Loc.Localize("BeastTribeLabel", "Beast Tribe");
            public readonly string BeastTribeInformation = Loc.Localize("BeastTribeInformation", "Beast Tribes are a daily task that can be completed for experience rewards and to earn progress towards beast tribes affiliations that will unlock unique dyes, glamours, mounts, and housing items.");
            public readonly string BeastTribeAutomationInformation = Loc.Localize("BeastTribeAutomationInformation", "Beast tribe allowance information is extracted directly from the game's data and will always be in sync with the current status of the game.");
            public readonly string BeastTribeTechnicalInformation = Loc.Localize("BeastTribeTechnicalInformation", "DailyDuty provides options to alert you when you are at a threshold of allowances, either by directly imputing the threshold, or by specifying how many beast tribes-worth of allowances you wish to track.\n\nUpon reaching a new tier in each beast tribe you get 3 additional allowances for that tribe, DailyDuty is unable to track this information and will not consider the extra allowances for tracking purposes.");
            public readonly string BeastTribeAllowancesRemainingLabel = Loc.Localize("BeastTribeAllowancesRemainingLabel", "Above Allowance Threshold");
            public readonly string BeastTribeMarkCompleteWhenLabel = Loc.Localize("BeastTribeMarkCompleteWhenLabel", "Mark Complete When");

            public readonly string LevequestLabel = Loc.Localize("LevequestLabel", "Levequest");
            public readonly string LevequestInformation = Loc.Localize("LevequestInformation", "Levequests are repeatable quests players can perform to earn experience, gil and items. Unlike regular quests, which can only be performed once per character, levequests can be repeated as long as the player has Leve allowances.");
            public readonly string LevequestAutomationInformation = Loc.Localize("LevequestAutomationInformation", "Levequest information is extracted directly from the game's data and will always be in sync with the current status of the game.");
            public readonly string LevequestTechnicalInformation = Loc.Localize("LevequestTechnicalInformation", "Once a levequest is accepted the allowance is spent, if you cancel the levequest the allowance is not returned.");
            public readonly string LevequestAboveThresholdLabel = Loc.Localize("LevequestAboveThresholdLabel", "Above Allowance Threshold");
            public readonly string LevequestAcceptedLabel = Loc.Localize("LevequestAcceptedLabel", "Accepted");
            public readonly string NextAllowanceLabel = Loc.Localize("NextAllowanceLabel", "Next Allowances");

            public readonly string MiniCactpotLabel = Loc.Localize("MiniCactpotLabel", "Mini Cactpot");
            public readonly string MiniCactpotInformation = Loc.Localize("MiniCactpotInformation", "Mini Cactpot is a daily scratch-card like activity you can do three times per day. Each Mini Cactpot ticket awards a various amount of MGP up to 10,000 MGP per ticket.");
            public readonly string MiniCactpotAutomationInformation = Loc.Localize("MiniCactpotAutomationInformation", "Tracking is done by monitoring the opening of the Mini Cactpot ticket. If DailyDuty was installed after you have completed your daily Mini Cactpot, it will not be tracked.");
            public readonly string MiniCactpotTechnicalInformation = Loc.Localize("MiniCactpotTechnicalInformation", "The game removes your Mini Cactpot ticket allowance when the window is opened and you spend the MGP to purchase the ticket. If your game is closed or crashes after the ticket window opened, that ticket will be lost.");
            public readonly string MiniCactpotClickableLinkDescription = Loc.Localize("MiniCactpotClickableLinkDescription", "Notifications can be clicked on to teleport to the Gold Saucer");
            public readonly string MiniCactpotReSyncInformation = Loc.Localize("MiniCactpotReSyncInformation", "You can force a re-sync of Mini Cactpot allowances by speaking to the 'Mini Cactpot Broker' and selecting 'Purchase a Mini Cactpot ticket'");
            public readonly string MiniCactpotAllowancesLabel = Loc.Localize("MiniCactpotAllowancesLabel", "Allowances Available");

            public readonly string CustomDeliveryLabel = Loc.Localize("CustomDeliveryLabel", "Custom Delivery");
            public readonly string CustomDeliveryInformation = Loc.Localize("CustomDeliveryInformation", "Custom Deliveries is a weekly DoH/DoL task you can complete for various crafting or gathering scripts, exp rewards, various materia, folklore tomes, and more.");
            public readonly string CustomDeliveryAutomationInformation = Loc.Localize("CustomDeliveryAutomationInformation", "Custom Delivery information is gathered directly from the game's data, and will always be in sync with the current status of the game.");
            public readonly string CustomDeliveryAllowancesLabel = Loc.Localize("CustomDeliveryAllowancesLabel", "Allowances Available");

            public readonly string DomanEnclaveLabel = Loc.Localize("DomanEnclaveLabel", "Doman Enclave");
            public readonly string DomanEnclaveInformation = Loc.Localize("DomanEnclaveInformation", "The Doman Enclave is a weekly task you can complete for gil rewards. You donate junk items in exchange for a multiple of their value in gil back, resulting in profit. The reward multiplier increases higher the more you progress through the Doman Enclave.");
            public readonly string DomanEnclaveAutomationInformation = Loc.Localize("DomanEnclaveAutomationInformation", "Doman Enclave data is tracked automatically, but only while you are actually in the Doman Enclave area.");
            public readonly string DomanEnclaveTechnicalInformation = Loc.Localize("DomanEnclaveTechnicalInformation", "Once you visit the Doman Enclave, DailyDuty attempts to keep a copy of the status, and will reset that status at the start of a new week. If the data somehow becomes out of sync, simply re-visit the Doman Enclave and it will automatically re-sync.");
            public readonly string DomanEnclaveGilRemainingLabel = Loc.Localize("DomanEnclaveGilRemainingLabel", "gil Remaining");
            public readonly string DomanEnclaveBudgetRemainingLabel = Loc.Localize("DomanEnclaveBudgetRemainingLabel", "Budget Remaining");
            public readonly string DomanEnclaveCurrentAllowanceLabel = Loc.Localize("DomanEnclaveCurrentAllowanceLabel", "Current Allowance");
            public readonly string DomanEnclaveClickableLinkDescription = Loc.Localize("DomanEnclaveClickableLinkDescription", "Notifications can be clicked on to teleport to the Doman Enclave");
            public readonly string DomanEnclaveInitializationWarning = Loc.Localize("DomanEnclaveInitializationWarning", "You can re-sync Doman Enclave data by visiting the Doman Enclave");

            public readonly string FashionReportLabel = Loc.Localize("FashionReportLabel", "Fashion Report");
            public readonly string FashionReportInformation = Loc.Localize("FashionReportInformation", "Fashion Report is a weekly task you can complete for MGP. Simply 'Present yourself for judging' once a week without any effort and be rewarded with 10,000 MGP. You can attempt to create an outfit for a higher score, additional MGP, and titles.");
            public readonly string FashionReportAutomationInformation = Loc.Localize("FashionReportAutomationInformation", "Fashion Report data is collected when you present yourself for judging. The current high score and allowances remaining is extracted from the UI when the judging is complete.");
            public readonly string FashionReportTechnicalInformation = Loc.Localize("FashionReportTechnicalInformation", "Data is collected both when you interact with 'Masked Rose' and when you are actually presenting yourself for judging. This should keep the data in sync with the game data at all times.\n\nFashion Report status will show as 'Complete' while Fashion Report is unavailable, and 'Incomplete' once it is available or the completion conditions are not met.");
            public readonly string FashionReportHighestScoreLabel = Loc.Localize("FashionReportHighestScoreLabel", "Highest Score");
            public readonly string FashionReportAvailableLabel = Loc.Localize("FashionReportAvailableLabel", "Report Open");
            public readonly string FashionReportSingleModeLabel = Loc.Localize("FashionReportSingleModeLabel", "Single");
            public readonly string FashionReportEightyPlusLabel = Loc.Localize("FashionReportEightyPlusLabel", "80 Plus");
            public readonly string FashionReportAllLabel = Loc.Localize("FashionReportAllLabel", "All");
            public readonly string FashionReportSingleModeDescription = Loc.Localize("FashionReportSingleModeDescription", "Only notify if no allowances have been spent this week and Fashion Report is available for turn-in");
            public readonly string FashionReportEightyPlusDescription = Loc.Localize("FashionReportEightyPlusDescription", "Notify if any allowances remain this week, the highest score is below 80 and Fashion Report is available for turn-in");
            public readonly string FashionReportAllDescription = Loc.Localize("FashionReportAllDescription", "Notify if any allowances remain this week and fashion report is available for turn-in");
            public readonly string FashionReportClickableLinkDescription = Loc.Localize("FashionReportClickableLinkDescription", "Notifications can be clicked on to teleport to the Gold Saucer");
            public readonly string FashionReportReSyncInformation = Loc.Localize("FashionReportReSyncInformation", "You can re-sync Fashion Report data by speaking to 'Masked Rose'");
            public readonly string FashionReportAllowancesLabel = Loc.Localize("FashionReportAllowancesLabel", "Allowances Available");

            public readonly string JumboCactpotLabel = Loc.Localize("JumboCactpotLabel", "Jumbo Cactpot");
            public readonly string JumboCactpotInformation = Loc.Localize("JumboCactpotInformation", "Jumbo Cactpot is a weekly task where you can purchase 3 lottery tickets. These tickets allow you to pick any 4-digit number you wish, and depending on how many numbers are matched at the drawing the following week, you can earn MGP.");
            public readonly string JumboCactpotAutomationInformation = Loc.Localize("JumboCactpotAutomationInformation", "Jumbo Cactpot data is collected when you purchase your tickets. If DailyDuty was not installed when you purchased your tickets it will be unable to track your tickets for that week.");
            public readonly string JumboCactpotTechnicalInformation = Loc.Localize("JumboCactpotTechnicalInformation", "While DailyDuty displays the value of the tickets collected, the only information needed to track the status is how many tickets have been purchased.\n\nUpon weekly reset the stored tickets will be removed and you will be prompted to purchase new tickets. If you have any pending rewards, you will have to collect your tickets in order to purchase new tickets.\n\nDue to how the fallback re-sync system works, if you speak to the 'Jumbo Cactpot Broker' before collecting your rewards, last-weeks tickets will get re-logged and the task will be marked as complete. Speaking to the 'Jumbo Cactpot Broker' after collecting your rewards will re-sync the status back to incomplete.");
            public readonly string JumboCactpotTicketsLabel = Loc.Localize("JumboCactpotTicketsLabel", "Tickets");
            public readonly string JumboCactpotTicketsAvailableLabel = Loc.Localize("JumboCactpotTicketsAvailableLabel", "Tickets Available");
            public readonly string JumboCactpotTicketAvailableLabel = Loc.Localize("JumboCactpotTicketAvailableLabel", "Ticket Available");
            public readonly string JumboCactpotNextDrawingLabel = Loc.Localize("JumboCactpotNextDrawingLabel", "Next Drawing");
            public readonly string JumboCactpotClickableLinkDescription = Loc.Localize("JumboCactpotClickableLinkDescription", "Notifications can be clicked on to teleport to the Gold Saucer");
            public readonly string JumboCactpotReSyncInformation = Loc.Localize("JumboCactpotReSyncInformation", "You can force a re-sync of ticket data by speaking to the 'Jumbo Cactpot Broker'");

            public readonly string HuntMarksWeeklyLabel = Loc.Localize("HuntMarksWeeklyLabel", "Hunt Marks (Weekly)");
            public readonly string HuntMarksWeeklyInformation = Loc.Localize("HuntMarksWeeklyInformation", "Weekly Hunts are also know as 'Elite Hunts', these are Weekly Hunts you can do for various hunt currencies such as Allied Seals, Centurio Seals, and Sacks of Nuts.\n\nThese currencies can be exchanged for various rewards. A reward that is of note is the Aetheryte Tickets, which can be used instead of paying gil for aetheryte teleports.");
            public readonly string HuntMarksAutomationInformation = Loc.Localize("HuntMarksAutomationInformation", "Hunt data is tracked by observing changes in the games stored data. If DailyDuty was not installed when you obtained a hunt mark then it can not be tracked correctly.");
            public readonly string HuntMarksTechnicalInformation = Loc.Localize("HuntMarksTechnicalInformation", "While the data is always in sync with the game, the game does not clear this data on reset. Instead, it only clears the data when you pick up the new hunt mark item.\n\nDailyDuty will reset its copy of the tracked data each week and attempt to start tracking when a new hunt mark is picked up.");
            public readonly string HuntMarksHuntsRemainingLabel = Loc.Localize("HuntMarksHuntsRemainingLabel", "Hunts Remaining");
            public readonly string HuntMarksTrackedHuntsLabel = Loc.Localize("HuntMarksTrackedHuntsLabel", "Tracked Hunts");
            public readonly string HuntMarksTrackLabel = Loc.Localize("HuntMarksTrackLabel", "Track Hunt");
            public readonly string HuntMarksMarkAvailableLabel = Loc.Localize("HuntMarksMarkAvailableLabel", "Mark Available");
            public readonly string HuntMarksMarkObtainedLabel = Loc.Localize("HuntMarksMarkObtainedLabel", "Mark Obtained");
            public readonly string HuntMarksMarkKilledLabel = Loc.Localize("HuntMarksMarkKilledLabel", "Mark Killed");
            public readonly string HuntMarksTargetName = Loc.Localize("HuntMarksTargetName", "Target Name");
            public readonly string HuntMarksTargetLocation = Loc.Localize("HuntMarksTargetLocation", "Target Location");
            public readonly string HuntMarksNoHuntsTracked = Loc.Localize("HuntMarksNoHuntsTracked", "No Hunts Tracked");
            public readonly string HuntMarksTrackAllLabel = Loc.Localize("HuntMarksTrackAllLabel", "Track All");
            public readonly string HuntMarksUntrackAllLabel = Loc.Localize("HuntMarksUntrackAllLabel", "Untrack All");

            public readonly string HuntMarksDailyLabel = Loc.Localize("HuntMarksDailyLabel", "Hunt Marks (Daily)");
            public readonly string HuntMarksDailyInformation = Loc.Localize("HuntMarksDailyInformation", "Daily Hunts are low rank hunts that are available each day. These hunts involve killing a specific number of fairly common enemies.");


        }

        public class CommandStrings
        {
            public readonly string TimersCommand = Loc.Localize("TimersCommand", "timers");
            public readonly string TodoCommand = Loc.Localize("TodoCommand", "todo");

            public readonly string On = Loc.Localize("On", "on");
            public readonly string Enable = Loc.Localize("Enable", "enable");
            public readonly string Show = Loc.Localize("Show", "show");
            public readonly string Off = Loc.Localize("Off", "off");
            public readonly string Disable = Loc.Localize("Disable", "disable");
            public readonly string Hide = Loc.Localize("Hide", "hide");
            public readonly string Close = Loc.Localize("Close", "close");
            public readonly string Open = Loc.Localize("Open", "open");
            public readonly string Toggle = Loc.Localize("Toggle", "toggle");
            public readonly string Help = Loc.Localize("Help", "help");
            public readonly string Core = Loc.Localize("Core", "Core");

            public readonly string TimersShowHelp = Loc.Localize("TimersShowHelp", "\n/dd timers - Shows this help message\n/dd timers help - Shows this help message\n/dd timers (on | enable | show | open) - Shows the timers window\n/dd timers (off | disable | hide | close) - Hides the timers window\n/dd timers (toggle) - Toggle the timers window");
            public readonly string TodoShowHelp = Loc.Localize("TodoShowHelp", "\n/dd todo - Shows this help message\n/dd todo help - Shows this help message\n/dd todo (on | enable | show | open) - Shows the todo window\n/dd todo (off | disable | hide | close) - Hides the todo window\n/dd todo (toggle) - Toggle the todo window");

            public readonly string HelpCommands = Loc.Localize("HelpCommands", "Command Overview\n/dd - Show or Hide Main Window\n/dd timers help - Show timer sub-commands\n/dd todo help - Show todo sub-commands");
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

            public readonly string DailyResetShortLabel = Loc.Localize("DailyResetShortLabel", "Daily");
            public readonly string FashionReportShortLabel = Loc.Localize("FashionReportShortLabel", "Fashion");
            public readonly string JumboCactpotShortLabel = Loc.Localize("JumboCactpotShortLabel", "Jumbo");
            public readonly string LeveAllowanceShortLabel = Loc.Localize("LeveAllowanceShortLabel", "Leve");
            public readonly string TreasureMapShortLabel = Loc.Localize("TreasureMapShortLabel", "Map");
            public readonly string WeeklyResetShortLabel = Loc.Localize("WeeklyResetShortLabel", "Weekly");

            public readonly string TimersBarColorLabel = Loc.Localize("TimersBarColorLabel", "Progress bar color");
            public readonly string TimersHideSecondsLabel = Loc.Localize("TimersHideSecondsLabel", "Hide seconds");
        }

        public static TabStrings Tabs = new();
        public static ConfigurationStrings Configuration = new();
        public static FeaturesStrings Features = new();
        public static CommonStrings Common = new();
        public static ModuleStrings Module = new();
        public static TimersStrings Timers = new();
        public static CommandStrings Command = new();
    }
}
