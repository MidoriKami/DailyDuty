using CheapLoc;

namespace DailyDuty.System.Localization;

internal static class Strings
{
    public static Configuration Configuration { get; } = new();
    public static Common Common { get; } = new();
    public static Command Command { get; } = new();
    public static Module Module { get; } = new();
    public static UserInterface UserInterface { get; } = new();
}

#region Configuration
public class Configuration
{

}
#endregion


#region Common
public class Common
{
    public string Information => Loc.Localize("Information", "Information");
    public string AutomationInformation => Loc.Localize("AutomationInformation", "Data Collection");
    public string TechnicalInformation => Loc.Localize("TechnicalInformation", "Technical Information");
}
#endregion

#region Commands
public class Command
{
    public static MainWindow MainWindow { get; } = new();
}

public class MainWindow
{

}
#endregion

#region Modules
public class Module
{
    public static BeastTribe BeastTribe { get; } = new();
    public static CustomDelivery CustomDelivery { get; } = new();
    public static DomanEnclave DomanEnclave { get; } = new();
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
#endregion

#region UserInterface

public class UserInterface
{

}
#endregion