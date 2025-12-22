namespace DailyDuty.Utilities;

/// <summary>
/// Configuration File Utilities
/// </summary>
public static class Config {
    public static string CharacterConfigPath => FileHelpers.GetFileInfo( FileHelpers.GetCharacterPath()).FullName;

    /// <summary>
    /// Loads a configuration file from PluginConfigs\DailyDuty\{FileName}
    /// Creates a `new T()` if the file can't be loaded
    /// </summary>
    public static T LoadConfig<T>(string fileName) where T : new()
        => FileHelpers.LoadFile<T>(FileHelpers.GetFileInfo(fileName).FullName);
    
    /// <summary>
    /// Loads a character specific config file from PluginConfigs\DailyDuty\{ContentId}\{FileName}
    /// Creates a `new T` if the file can't be loaded
    /// </summary>
    /// <remarks>Requires the character to be logged in</remarks>
    public static T LoadCharacterConfig<T>(string fileName) where T : new()
        => FileHelpers.LoadFile<T>(FileHelpers.GetFileInfo(FileHelpers.GetCharacterPath(), fileName).FullName);
    
    /// <summary>
    /// Saves a configuration file to PluginConfigs\DailyDuty\{FileName}
    /// </summary>
    public static void SaveConfig<T>(T modificationConfig, string fileName)
        => FileHelpers.SaveFile(modificationConfig, FileHelpers.GetFileInfo( fileName).FullName);
    
    /// <summary>
    /// Saves a character specific config file to PluginConfigs\DailyDuty\{ContentId}\{FileName}
    /// </summary>
    /// <remarks>Requires the character to be logged in</remarks>
    public static void SaveCharacterConfig<T>(T modificationConfig, string fileName)
        => FileHelpers.SaveFile(modificationConfig, FileHelpers.GetFileInfo(FileHelpers.GetCharacterPath(), fileName).FullName);
}
