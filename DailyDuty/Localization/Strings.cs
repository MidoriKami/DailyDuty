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
            public readonly string WondrousTailsDutyFinderOverlayLabel = Loc.Localize("WondrousTailsDutyFinderOverlayLabel", "Wondrous Tails Duty Finder Overlay");
            public readonly string DutyRouletteDutyFinderOverlayLabel = Loc.Localize("DutyRouletteDutyFinderOverlayLabel", "Duty Roulette Duty Finder Overlay");
            public readonly string DutyRouletteDutyFinderOverlayAutomationInformation = Loc.Localize("DutyRouletteDutyFinderOverlayAutomationInformation", "The 'Duty Roulette' status data is gathered directly from internal game data and will always be precisely in sync with the game.");
            public readonly string DutyRouletteDutyFinderOverlayInformationDisclaimer = Loc.Localize("DutyRouletteDutyFinderOverlayInformationDisclaimer", "Requires 'Duty Roulette' to be enabled to function");
            public readonly string DutyRouletteDutyFinderOverlayDescription = Loc.Localize("DutyRouletteDutyFinderOverlayDescription", "Changes the colors of the duty roulette text in the Duty Finder to reflect the daily completion status.");
            public readonly string DutyRouletteDutyFinderOverlayTechnicalDescription = Loc.Localize("DutyRouletteDutyFinderOverlayTechnicalDescription", "Use the 'Duty Roulette' module to configure which roulettes are colored.\n\nYou can also set the 'complete' and 'incomplete' colors in the 'Duty Roulette' module\n\nRoulettes that are not tracked will not be colored regardless of their completion status.");
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
            public readonly string DutyRouletteAutomationInformation = Loc.Localize("DutyRouletteAutomationInformation", "This module is entirely automatic, no user interaction is required.\n\nThe completion status of each roulette is gathered direct from the gamedata.");
            public readonly string DutyRouletteTechnicalInformation = Loc.Localize("DutyRouletteTechnicalInformation", "You can configure this module to alert you when the specified duties have not yet been completed today\n\nOny the duties selected for tracking in the Options tab will be considered for notifications\n\nSome listed roulettes may not be available due to not being unlocked or due to being unavailable until a later patch. It is not recommended to track roulettes you do not have access to.");
        }

        public static readonly TabStrings Tabs = new();
        public static readonly ConfigurationStrings Configuration = new();
        public static readonly FeaturesStrings Features = new();
        public static readonly CommonStrings Common = new();
        public static readonly ModuleStrings Module = new();
    }
}
