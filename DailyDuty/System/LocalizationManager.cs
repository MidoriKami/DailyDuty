using System;
using System.IO;
using Dalamud.Logging;

namespace DailyDuty.System;

internal class LocalizationManager : IDisposable
{
    private readonly Dalamud.Localization localization;

    public LocalizationManager()
    {
        var assemblyLocation = Service.PluginInterface.AssemblyLocation.DirectoryName!;
        var filePath = Path.Combine(assemblyLocation, @"translations");

        localization = new Dalamud.Localization(filePath, "DailyDuty_");
        localization.SetupWithLangCode(Service.PluginInterface.UiLanguage);

        Service.PluginInterface.LanguageChanged += LoadLocalization;

        #if DEBUG
        localization.ExportLocalizable();
        #endif
    }
    
    public void Dispose()
    {
        Service.PluginInterface.LanguageChanged -= LoadLocalization;
    }

    public void LoadLocalization(string languageCode)
    {
        try
        {
            PluginLog.Information($"Loading Localization for {languageCode}");
            localization.SetupWithLangCode(languageCode);
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Unable to Load Localization");
        }
    }
}