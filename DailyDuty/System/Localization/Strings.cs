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
}

public class Status
{
    public string Label => Loc.Localize("Status_Label", "Status");
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
    public string Timers => Loc.Localize("Help_Timers", "\n/dd timers - Shows this help message\n" +
                                                               "/dd timers help - Shows this help message\n" +
                                                               "/dd timers show - Shows the timers window\n" +
                                                               "/dd timers hide - Hides the timers window\n" +
                                                               "/dd timers toggle - Toggle the timers window");

    public string Todo => Loc.Localize("Help_Todo", "\n/dd todo - Shows this help message\n" +
                                                           "/dd todo help - Shows this help message\n" +
                                                           "/dd todo show - Shows the todo window\n" +
                                                           "/dd todo hide - Hides the todo window\n" +
                                                           "/dd todo toggle - Toggle the todo window");

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
}

public class Language
{
    public string Changed => Loc.Localize("Language_Changed", "Language has been changed, a restart is required to update some strings");
}
