using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using DailyDuty.Utilities;
using Dalamud.Plugin.Services;

namespace DailyDuty.Data;

public static class SerializerOptions {
    public static readonly JsonSerializerOptions Options = new() {
        WriteIndented = true,
        IncludeFields = true,
    };
}

public abstract class FileBase<T> where T : FileBase<T>, new() {

    /// <summary>
    /// Gets the file name used for this file.
    /// </summary>
    protected abstract string FileName { get; }

    /// <summary>
    /// Gets an overridable version identifier for this file.
    /// </summary>
    /// <remarks>
    /// Defaults to version 1, if a migration is needed override and increment version.
    /// </remarks>
    public virtual int Version
        => 1;

    private readonly SemaphoreSlim saveLock = new(1, 1);
    private bool pendingSave;

    public static async Task<T> Load() {
        var dummyInstance = new T();
        var fileName = dummyInstance.FileName;

        IPluginLog.Get().Debug($"Loading File {fileName}");
        var fileInfo = FileHelpers.GetFileInfo(FileHelpers.GetCharacterPath(), fileName);

        // New files are created by LoadFile; no migration needed.
        if (!fileInfo.Exists) {
            return await FileHelpers.LoadFile<T>(fileInfo.FullName);
        }

        // Attempt to load and migrate the existing file.
        var migrated = await TryLoadMigratedAsync(dummyInstance, fileName, fileInfo);
        if (migrated is not null) {
            return migrated;
        }

        // Migration failed or file is invalid; fall back to the default loader.
        return await FileHelpers.LoadFile<T>(fileInfo.FullName);
    }

    /// <summary>
    /// Attempts to parse and migrate an existing file.
    /// </summary>
    /// <returns>The migrated instance on success; null on any failure so the caller can fall back.</returns>
    private static async Task<T?> TryLoadMigratedAsync(T instance, string fileName, FileInfo fileInfo) {
        try {
            var fileText = await File.ReadAllTextAsync(fileInfo.FullName);
            var fileData = JsonSerializer.Deserialize<JsonObject>(fileText);

            if (fileData is null) {
                IPluginLog.Get().Error($"Failed to parse file {fileName} as JSON");
                return null;
            }

            var targetVersion = instance.Version;

            // Handle files with no version defined (pre-versioning files).
            if (fileData[nameof(Version)] is null) {
                return await SetVersionAsync(fileData, fileName, fileInfo, targetVersion);
            }

            return await MigrateAsync(instance, fileName, fileInfo, fileData, targetVersion);
        }
        catch (Exception e) {
            IPluginLog.Get().Error(e, $"Failed to migrate file {fileName}");
            return null;
        }
    }

    /// <summary>
    /// Sets the version on a file that has no version, deserializes it, and saves it.
    /// </summary>
    private static async Task<T> SetVersionAsync(JsonObject fileData, string fileName, FileInfo fileInfo, int targetVersion) {
        IPluginLog.Get().Debug($"File {fileName} has no version defined. Setting to current version {targetVersion}");
        fileData[nameof(Version)] = targetVersion;

        var migratedInstance = fileData.Deserialize<T>(SerializerOptions.Options) ?? new T();
        await FileHelpers.SaveFile(migratedInstance, fileInfo.FullName);
        return migratedInstance;
    }

    /// <summary>
    /// Steps through migration versions until the target is reached, then deserializes and saves the result.
    /// </summary>
    private static async Task<T?> MigrateAsync(T instance, string fileName, FileInfo fileInfo, JsonObject fileData, int targetVersion) {
        var initialVersion = fileData[nameof(Version)]!.GetValue<int>();
        var fileVersion = initialVersion;

        // Step through each migration until we reach the target version.
        while (fileVersion < targetVersion) {
            if (!instance.TryMigrateStep(ref fileVersion, fileData)) {
                IPluginLog.Get().Error($"Failed to migrate file {fileName} from version {fileVersion}");
                return null;
            }

            fileData[nameof(Version)] = fileVersion;
        }

        // If migration was needed and succeeded, save the migrated instance.
        if (fileVersion == targetVersion && initialVersion < targetVersion) {
            IPluginLog.Get().Debug($"Successfully migrated {fileName} from version {initialVersion} to version {targetVersion}");

            var migratedInstance = fileData.Deserialize<T>(SerializerOptions.Options) ?? new T();
            await FileHelpers.SaveFile(migratedInstance, fileInfo.FullName);
            return migratedInstance;
        }

        // No migration needed; deserialize the file as-is.
        return fileData.Deserialize<T>(SerializerOptions.Options) ?? new T();
    }

    public Task Save() {
        IPluginLog.Get().Debug($"Queuing Save for {FileName}");

        Interlocked.Exchange(ref pendingSave, true);

        return Task.Run(SaveToFileAsync);
    }

    private async Task SaveToFileAsync() {
        // If we already have a save task running, abort and return.
        if (!await saveLock.WaitAsync(0)) {
            IPluginLog.Get().Verbose($"Save in progress for {FileName}, skipping save");
            return;
        }

        try {
            var fileInfo = FileHelpers.GetFileInfo(FileHelpers.GetCharacterPath(), FileName);

            // The while is in-case another save request came in while we are awaiting SaveConfig to complete.
            // It'll allow this same task to handle multiple saves.
            while (Interlocked.Exchange(ref pendingSave, false)) {
                IPluginLog.Get().Debug($"Saving {FileName}");
                await FileHelpers.SaveFile(this, fileInfo.FullName);
            }
        }
        catch (Exception e) {
            IPluginLog.Get().Error(e, $"Failed to save {FileName}");
        }
        finally {
            saveLock.Release();
        }
    }

    /// <summary>
    /// Function for migrating old values to new values.
    /// </summary>
    /// <param name="fileVersion">Number indicating current file version, be sure to increment this on successful migration</param>
    /// <param name="fileData"></param>
    /// <returns>true if the migration step succeeded and <paramref name="fileVersion"/> was incremented; false if no migration was performed.</returns>
    /// <remarks>
    /// Version will be 0 if migrating from a file before this system was implemented.
    /// </remarks>
    protected virtual bool TryMigrateStep(ref int fileVersion, JsonObject fileData)
        => false;
}
