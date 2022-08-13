using System;
using System.IO;
using CheapLoc;
using DailyDuty.Utilities;
using Dalamud.Logging;

namespace DailyDuty.System;

internal class LocalizationManager : IDisposable
{
    public Dalamud.Localization Localization;

    public LocalizationManager()
    {
        Log.Verbose("Constructing LocalizationManager");

        var assemblyLocation = Service.PluginInterface.AssemblyLocation.DirectoryName!;
        var filePath = Path.Combine(assemblyLocation, @"translations");

        Localization = new Dalamud.Localization(filePath, "NoTankYou_");

        Loc.SetupWithFallbacks();

        var dalamudLanguage = Service.PluginInterface.UiLanguage;
        LoadLocalization(dalamudLanguage);

        Service.PluginInterface.LanguageChanged += LoadLocalization;
    }

    public void Dispose()
    {
        Service.PluginInterface.LanguageChanged -= LoadLocalization;
    }

    public void LoadLocalization(string languageCode)
    {
        try
        {
            Log.Verbose($"Loading Localization for {languageCode}");
            Localization.SetupWithLangCode(languageCode);

        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Unable to Load Localization");
        }
    }


}