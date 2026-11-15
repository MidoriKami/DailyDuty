using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DailyDuty.Utilities;
using Dalamud.Plugin.Services;
using Newtonsoft.Json.Linq;

namespace DailyDuty.Classes;

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

        // If it doesn't exist yet, LoadFile will create one for us.
        // We also don't have to migrate when a new instance is created.
        if (!fileInfo.Exists) {
            return await FileHelpers.LoadFile<T>(fileInfo.FullName);
        }

        try {
            var fileText = await File.ReadAllTextAsync(fileInfo.FullName);
            var jObject = JObject.Parse(fileText);

            var versionToken = jObject[nameof(Version)];
            var targetVersion = dummyInstance.Version;

            // Version wasn't defined until now, just set it and call it good.
            if (versionToken is null) {
                IPluginLog.Get().Debug($"File {fileName} has no version defined. Setting to current version {targetVersion}");

                // Set the version to target, save, and return
                jObject[nameof(Version)] = targetVersion;
                var migratedInstance = jObject.ToObject<T>() ?? new T();
                await FileHelpers.SaveFile(migratedInstance, fileInfo.FullName);
                return migratedInstance;
            }

            var initialVersion = versionToken.ToObject<int>();
            var fileVersion = initialVersion;

            // Try to do the migration
            while (fileVersion < targetVersion) {
                if (!dummyInstance.TryMigrateStep(ref fileVersion, jObject)) {
                    IPluginLog.Get().Error($"Failed to migrate file {fileName} from version {fileVersion}");
                    break;
                }

                // Update jObject with new version.
                jObject[nameof(Version)] = fileVersion;
            }

            // If we reached our target version, and migration was needed in the first place
            if (fileVersion == targetVersion && initialVersion < targetVersion) {
                IPluginLog.Get().Debug($"Successfully migrated {fileName} from version {initialVersion} to version {targetVersion}");

                var migratedInstance = jObject.ToObject<T>() ?? new T();
                await FileHelpers.SaveFile(migratedInstance, fileInfo.FullName);
                return migratedInstance;
            }
        }
        catch (Exception e) {
            IPluginLog.Get().Error(e, $"Failed to migrate file {fileName}");
        }

        // File existed, we tried to migrate it, but it failed, so load the original file, or create a new one if its invalid.
        return await FileHelpers.LoadFile<T>(fileInfo.FullName);
    }

    public Task Save() {
        IPluginLog.Get().Debug($"Queuing Save for {FileName}");

        Interlocked.Exchange(ref pendingSave, true);

        return Task.Run(SaveAsync);
    }

    private async Task SaveAsync() {
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
    /// <param name="jObject">The JSON properties of the loaded config file</param>
    /// <returns>true if the migration step succeeded and <paramref name="fileVersion"/> was incremented; false if no migration was performed.</returns>
    /// <remarks>
    /// Version will be 0 if migrating from a file before this system was implemented.
    /// </remarks>
    protected virtual bool TryMigrateStep(ref int fileVersion, JObject jObject)
        => false;
}
