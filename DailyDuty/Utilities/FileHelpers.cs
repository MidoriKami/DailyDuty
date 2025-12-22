using System;
using System.IO;
using System.Text.Json;
using Dalamud.Utility;

namespace DailyDuty.Utilities;

public static class FileHelpers {
    private static readonly JsonSerializerOptions SerializerOptions = new() {
        WriteIndented = true,
        IncludeFields = true,
    };

    public static T LoadFile<T>(string filePath) where T : new() {
        var fileInfo = new FileInfo(filePath);
        if (fileInfo is { Exists: true }) {
            try {
                var fileText = File.ReadAllText(fileInfo.FullName);
                var dataObject = JsonSerializer.Deserialize<T>(fileText, SerializerOptions);

                // If deserialize result is null, create a new instance instead and save it.
                if (dataObject is null) {
                    dataObject = new T();
                    SaveFile(dataObject, filePath);
                }
            
                return dataObject;
            }
            catch (Exception e) {
                // If there is any kind of error loading the file, generate a new one instead and save it.
                Services.PluginLog.Error(e, $"Error trying to load file {filePath}, creating a new one instead.");
            
                SaveFile(new T(), filePath);
            }
        }

        var newFile = new T();
        SaveFile(newFile, filePath);
    
        return newFile;
    }

    public static void SaveFile<T>(T? file, string filePath) {
        try {
            if (file is null) {
                Services.PluginLog.Error("Null file provided.");
                return;
            }
            
            var fileText = JsonSerializer.Serialize(file, file.GetType(), SerializerOptions);
            FilesystemUtil.WriteAllTextSafe(filePath, fileText);
        }
        catch (Exception e) {
            Services.PluginLog.Error(e, $"Error trying to save file {filePath}");
        }
    }

    public static byte[] LoadBinaryFile(int length, string filePath) {
        var fileInfo = new FileInfo(filePath);
        if (fileInfo is { Exists: true }) {
            try {
                var dataObject = File.ReadAllBytes(fileInfo.FullName);

                // If deserialize result is null, create a new instance instead and save it.
                if (dataObject.Length != length) {
                    dataObject = new byte[length];
                    SaveFile(dataObject, filePath);
                }
            
                return dataObject;
            }
            catch (Exception e) {
                // If there is any kind of error loading the file, generate a new one instead and save it.
                Services.PluginLog.Error(e, $"Error trying to load file {filePath}, creating a new one instead.");
            
                SaveFile(new byte[length], filePath);
            }
        }

        var newFile = new byte[length];
        SaveFile(newFile, filePath);
    
        return newFile;
    }

    public static void SaveBinaryFile(byte[] data, string filePath) {
        try {
            FilesystemUtil.WriteAllBytesSafe(filePath, data);
        }
        catch (Exception e) {
            Services.PluginLog.Error(e, $"Error trying to save binary data {filePath}");
        }
    }

    public static FileInfo GetFileInfo(params string[] path) {
        var directory = Services.PluginInterface.ConfigDirectory;

        for (var index = 0; index < path.Length - 1; index++) {
            directory = new DirectoryInfo(Path.Combine(directory.FullName, path[index]));
            if (!directory.Exists) {
                directory.Create();
            }
        }

        return new FileInfo(Path.Combine(directory.FullName, path[^1]));
    }

    public static string GetCharacterPath() {
        if (!Services.ClientState.IsLoggedIn) {
            throw new Exception("Character is not logged in.");
        }
        
        return Services.PlayerState.ContentId.ToString();
    }
}
