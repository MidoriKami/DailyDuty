using System;
using System.Globalization;
using DailyDuty.System.Localization;
using Dalamud.Logging;

namespace DailyDuty.System;

internal class LocalizationController : IDisposable
{
    private static LocalizationController? _instance;
    public static LocalizationController Instance => _instance ??= new LocalizationController();
    
    public void Initialize()
    {
        Strings.Culture = new CultureInfo(Service.PluginInterface.UiLanguage);

        Service.PluginInterface.LanguageChanged += OnLanguageChange;
    }

    public static void Cleanup()
    {
        _instance?.Dispose();
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
            Strings.Culture = new CultureInfo(languageCode);
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Unable to Load Localization");
        }
    }
}