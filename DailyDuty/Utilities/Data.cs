using System.Threading.Tasks;

namespace DailyDuty.Utilities;

public static class Data {
    public static string CharacterDataPath => FileHelpers.GetFileInfo(FileHelpers.GetCharacterPath()).FullName;

    /// <summary>
    /// Loads a data file from PluginConfigs\DailyDuty\{FileName}
    /// </summary>
    public static async Task<T> LoadData<T>(string fileName) where T : new()
        => await FileHelpers.LoadFile<T>(FileHelpers.GetFileInfo(fileName).FullName);

    /// <summary>
    /// Loads a data file from PluginConfigs\DailyDuty\{FolderName}\{FileName}
    /// </summary>
    public static async Task<T> LoadData<T>(string folderName, string fileName) where T : new()
        => await FileHelpers.LoadFile<T>(FileHelpers.GetFileInfo(folderName, fileName).FullName);

    /// <summary>
    /// Loads a character specific data file from PluginConfigs\DailyDuty\{ContentId}\{FileName}
    /// Creates a `new T` if the file can't be loaded
    /// </summary>
    /// <remarks>Requires the character to be logged in</remarks>
    public static async Task<T> LoadCharacterData<T>(string fileName) where T : new()
        => await FileHelpers.LoadFile<T>(FileHelpers.GetFileInfo(FileHelpers.GetCharacterPath(), fileName).FullName);

    /// <summary>
    /// Saves a data file to PluginConfigs\DailyDuty\{FileName}
    /// </summary>
    public static async Task SaveData<T>(T data, string fileName)
        => await FileHelpers.SaveFile(data, FileHelpers.GetFileInfo(fileName).FullName);

    /// <summary>
    /// Saves a data file to PluginConfigs\DailyDuty\{FolderName}\{FileName}
    /// </summary>
    public static async Task SaveData<T>(T data, string folderName, string fileName)
        => await FileHelpers.SaveFile(data, FileHelpers.GetFileInfo(folderName, fileName).FullName);

    /// <summary>
    /// Saves a character specific data file to PluginConfigs\DailyDuty\{ContentId}\{FileName}
    /// </summary>
    /// <remarks>Requires the character to be logged in</remarks>
    public static async Task SaveCharacterData<T>(T data, string fileName)
        => await FileHelpers.SaveFile(data, FileHelpers.GetFileInfo(FileHelpers.GetCharacterPath(), fileName).FullName);

    /// <summary>
    /// Loads a binary file from PluginConfigs\DailyDuty\{FolderName}\{FileName}
    /// </summary>
    public static async Task<byte[]> LoadBinaryData(int length, string folderName, string fileName)
        => await FileHelpers.LoadBinaryFile(length, FileHelpers.GetFileInfo(folderName, fileName).FullName);

    /// <summary>
    /// Saves a binary file to PluginConfigs\DailyDuty\{FolderName}\{FileName}
    /// </summary>
    public static async Task SaveBinaryData(byte[] data, string folderName, string fileName)
        => await FileHelpers.SaveBinaryFile(data, FileHelpers.GetFileInfo(folderName, fileName).FullName);
}
