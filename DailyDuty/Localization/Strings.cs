using CheapLoc;

namespace DailyDuty.Localization
{
    internal static class Strings
    {
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
            public readonly string InformationLabel = Loc.Localize("InformationLabel", "Information");
            public readonly string WondrousTailsDutyFinderOverlayLabel = Loc.Localize("WondrousTailsDutyFinderOverlayLabel", "Wondrous Tails Duty Finder Overlay");

            public readonly string DutyRouletteDutyFinderOverlayLabel = Loc.Localize("DutyRouletteDutyFinderOverlayLabel", "Duty Roulette Duty Finder Overlay");

            public readonly string DutyRouletteDutyFinderOverlayAutomationLabel =
                Loc.Localize("DutyRouletteDutyFinderOverlayAutomationLabel", "Data Collection");

            public readonly string DutyRouletteDutyFinderOverlayAutomationInformation = Loc.Localize(
                "DutyRouletteDutyFinderOverlayAutomationInformation",
                "The 'Duty Roulette' status data is gathered directly from internal game data and will always be precisely in sync with the game.");

            public readonly string DutyRouletteDutyFinderOverlayInformationDisclaimer =
                Loc.Localize("DutyRouletteDutyFinderOverlayInformationDisclaimer",
                    "* Requires 'Duty Roulette' to be enabled to function");

            public readonly string DutyRouletteDutyFinderOverlayDescription = Loc.Localize(
                "DutyRouletteDutyFinderOverlayDescription",
                "Changes the colors of the duty roulette text in the Duty Finder to reflect the daily completion status.");
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

        public static readonly TabStrings Tabs = new();
        public static readonly ConfigurationStrings Configuration = new();
        public static readonly FeaturesStrings Features = new();
    }
}
