using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Newtonsoft.Json;

namespace DailyDuty.System;

public static unsafe class FileController
{
    public static object LoadFile(string filePath, object targetObject)
    {
        DebugPrint($"[FileController] Loading {filePath}");
        
        if (LoadFile(filePath, targetObject.GetType(), out var loadedData))
        {
            return loadedData;
        }
        
        DebugPrint($"[FileController] File Doesn't Exist, creating: {filePath}");
        
        SaveFile(filePath, targetObject.GetType(), targetObject);
        return targetObject;
    }
    
    private static bool LoadFile(string fileName, Type fileType, [NotNullWhen(true)] out object? loadedData)
    {
        try
        {
            var fileInfo = GetFileInfo(fileName);

            if (fileInfo is { Exists: false })
            {
                loadedData = null;
                return false;
            }
        
            var jsonString = File.ReadAllText(fileInfo.FullName);
            loadedData = JsonConvert.DeserializeObject(jsonString, fileType)!;
            return true;
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"[FileController] Failed to load file: {fileName}");
            
            loadedData = null;
            return false;
        }
    }

    public static void SaveFile(string fileName, Type fileType, object objectData)
    {
        DebugPrint($"[FileController] Saving {fileName}");
        
        try
        {
            var fileInfo = GetFileInfo(fileName);
        
            var jsonString = JsonConvert.SerializeObject(objectData, fileType, new JsonSerializerSettings { Formatting = Formatting.Indented });
            File.WriteAllText(fileInfo.FullName, jsonString);
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"[FileController] Failed to save file: {fileName}");
        }
    }

    private static FileInfo GetFileInfo(string fileName)
    {
        var contentId = PlayerState.Instance()->ContentId;
        var configDirectory = GetCharacterDirectory(contentId);
        
        return new FileInfo(Path.Combine(configDirectory.FullName, fileName));
    }
    
    private static DirectoryInfo GetCharacterDirectory(ulong contentId)
    {
        var directoryInfo = new DirectoryInfo(Path.Combine(Service.PluginInterface.ConfigDirectory.FullName, contentId.ToString()));

        if (directoryInfo is { Exists: false })
        {
            directoryInfo.Create();
        }

        return directoryInfo;
    }

    private static void DebugPrint(string message)
    {
        #if DEBUG
        PluginLog.Debug(message);
        #endif
    }
}