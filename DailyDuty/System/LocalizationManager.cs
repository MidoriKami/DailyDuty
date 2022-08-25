using System;
using System.IO;
using CheapLoc;
using DailyDuty.System.Localization;
using DailyDuty.Utilities;
using Dalamud.Logging;

namespace DailyDuty.System;

internal class LocalizationManager : IDisposable
{
    private readonly Dalamud.Localization localization;
    private string lastLanguage;
    public LocalizationManager()
    {
        var assemblyLocation = Service.PluginInterface.AssemblyLocation.DirectoryName!;
        var filePath = Path.Combine(assemblyLocation, @"translations");
        localization = new Dalamud.Localization(filePath, "DailyDuty_");

        Loc.SetupWithFallbacks();

        var dalamudLanguage = Service.PluginInterface.UiLanguage;
        lastLanguage = dalamudLanguage;
        localization.SetupWithLangCode(dalamudLanguage);

        Service.PluginInterface.LanguageChanged += LoadLocalization;
    }

    public void Dispose()
    {
        Service.PluginInterface.LanguageChanged -= LoadLocalization;
    }

    public void LoadLocalization(string languageCode)
    {
        if (lastLanguage != languageCode)
        {
            try
            {
                Log.Verbose($"Loading Localization for {languageCode}");
                localization.SetupWithLangCode(languageCode);

                lastLanguage = languageCode;

                Chat.PrintError(Strings.Language.Changed);
            }
            catch (Exception ex)
            {
                PluginLog.Error(ex, "Unable to Load Localization");
            }
        }
    }
}