using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;

namespace DailyDuty.Utilities;

public static class FileHelpers {
    private static readonly JsonSerializerOptions SerializerOptions = new() {
        WriteIndented = true,
        IncludeFields = true,
    };

    public static async Task<T> LoadFile<T>(string filePath) where T : new() {
        var fileInfo = new FileInfo(filePath);
        if (fileInfo is { Exists: true }) {
            try {
                var fileText = await IReliableFileStorage.Get().ReadAllTextAsync(fileInfo.FullName);
                var dataObject = JsonSerializer.Deserialize<T>(fileText, SerializerOptions);

                // If deserialize result is null, create a new instance instead and save it.
                if (dataObject is null) {
                    dataObject = new T();
                    await SaveFile(dataObject, filePath);
                }

                return dataObject;
            }
            catch (Exception e) {
                // If there is any kind of error loading the file, generate a new one instead and save it.
                IPluginLog.Get().Error(e, $"Error trying to load file {filePath}, creating a new one instead.");

                await SaveFile(new T(), filePath);
            }
        }

        var newFile = new T();
        await SaveFile(newFile, filePath);

        return newFile;
    }

    public static async Task SaveFile<T>(T? file, string filePath) {
        try {
            if (file is null) {
                IPluginLog.Get().Error("Null file provided.");
                return;
            }

            var fileText = JsonSerializer.Serialize(file, file.GetType(), SerializerOptions);
            await IReliableFileStorage.Get().WriteAllTextAsync(filePath, fileText);
        }
        catch (Exception e) {
            IPluginLog.Get().Error(e, $"Error trying to save file {filePath}");
        }
    }

    public static async Task<byte[]> LoadBinaryFile(int length, string filePath) {
        var fileInfo = new FileInfo(filePath);
        if (fileInfo is { Exists: true }) {
            try {
                var dataObject = await IReliableFileStorage.Get().ReadAllBytesAsync(fileInfo.FullName);

                // If the loaded data doesn't match the expected length, create a new array instead.
                if (dataObject.Length != length) {
                    dataObject = new byte[length];
                    await SaveFile(dataObject, filePath);
                }

                return dataObject;
            }
            catch (Exception e) {
                // If there is any kind of error loading the file, generate a new byte array instead and save it.
                IPluginLog.Get().Error(e, $"Error trying to load file {filePath}, creating a new one instead.");

                await SaveFile(new byte[length], filePath);
            }
        }

        var newFile = new byte[length];
        await SaveFile(newFile, filePath);

        return newFile;
    }

    public static async Task SaveBinaryFile(byte[] data, string filePath) {
        try {
            await IReliableFileStorage.Get().WriteAllBytesAsync(filePath, data);
        }
        catch (Exception e) {
            IPluginLog.Get().Error(e, $"Error trying to save binary data {filePath}");
        }
    }

    public static FileInfo GetFileInfo(params string[] path) {
        var directory = DailyDuty.PluginInterface.ConfigDirectory;

        for (var index = 0; index < path.Length - 1; index++) {
            directory = new DirectoryInfo(Path.Combine(directory.FullName, path[index]));
            if (!directory.Exists) {
                directory.Create();
            }
        }

        return new FileInfo(Path.Combine(directory.FullName, path[^1]));
    }

    public static string GetCharacterPath() {
        if (!IClientState.Get().IsLoggedIn) {
            throw new InvalidOperationException("Character is not logged in.");
        }

        return IPlayerState.Get().ContentId.ToString();
    }
}
