using System.Globalization;
using KamiToolKit;

namespace DailyDuty.Utilities;

/// <summary>
/// Utility class for managing localization and culture settings.
/// </summary>
public static class Localization {
    /// <summary>
    /// Sets the current culture info based on the provided language code.
    /// </summary>
    /// <param name="language">The language code (e.g., "ja", "de", "fr").</param>
    public static void SetCultureInfo(object? language) {
        var languageName = language?.ToString();

        Strings.Culture = languageName switch {
            "ja" => CultureInfo.GetCultureInfo("ja-JP"),
            "zh" => CultureInfo.GetCultureInfo("zh-CN"),
            "de" => CultureInfo.GetCultureInfo("de-DE"),
            "fr" => CultureInfo.GetCultureInfo("fr-FR"),
            _ => CultureInfo.GetCultureInfo("en-US"),
        };

        KamiToolKitLibrary.SetCurrentCulture(Strings.Culture);
    }
}
