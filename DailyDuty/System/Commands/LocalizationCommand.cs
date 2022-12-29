using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using KamiLib.Utilities;

namespace DailyDuty.System.Commands
{
    internal class LocalizationCommand : IPluginCommand
    {
        public string CommandArgument => "loc";

        public void Execute(string? additionalArguments)
        {
            switch (additionalArguments)
            {
                case "generate":
                    Chat.Print("Command", "Generating Localization File");
                    Service.LocalizationManager.ExportLocalization();
                    break;
                
                default:
                    Chat.Print("Command", "Invalid Localization Command");
                    break;
            }
        }
    }
}
