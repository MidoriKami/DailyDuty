using System;
using System.Collections.Generic;
using System.IO;
using Dalamud.Logging;
using KamiLib;
using KamiLib.CommandSystem;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace DailyDuty.System;

public class LocalizationManager : IDisposable
{
    private readonly Dalamud.Localization? localization;

    public LocalizationManager()
    {
        KamiCommon.CommandManager.AddCommand(new LocalizationCommand(this));
        
        var assemblyLocation = Service.PluginInterface.AssemblyLocation.DirectoryName!;
        var filePath = Path.Combine(assemblyLocation, @"translations");

        localization = new Dalamud.Localization(filePath, $"{KamiCommon.PluginName}_");
        localization.SetupWithLangCode(Service.PluginInterface.UiLanguage);

        Service.PluginInterface.LanguageChanged += OnLanguageChange;
    }

    private void ExportLocalization()
    {
        try
        {
            localization?.ExportLocalizable();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Error exporting localization files");
        }
    }

    public void Dispose()
    {
        Service.PluginInterface.LanguageChanged -= OnLanguageChange;
    }

    private void OnLanguageChange(string languageCode)
    {
        try
        {
            PluginLog.Information($"Loading Localization for {languageCode}");
            localization?.SetupWithLangCode(languageCode);
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Unable to Load Localization");
        }
    }
    
    private class LocalizationCommand : IPluginCommand
    {
        private static LocalizationManager? _localization;
        
        public LocalizationCommand(LocalizationManager manager)
        {
            _localization = manager;
        }
        
        public string CommandArgument => "loc";

        public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
        {
            new SubCommand
            {
                CommandKeyword = "generate",
                CommandAction = () =>
                {
                    _localization?.ExportLocalization();
                    Chat.Print("Command", "Generating Localization File");
                },
                Hidden = true,
            },
        };
    }
}