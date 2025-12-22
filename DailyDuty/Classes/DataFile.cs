using System.Collections.Generic;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Lumina.Excel;

namespace DailyDuty.Classes;

/// <summary> For managing lumina data </summary>
public abstract class DataFile<T, TU> : IDataFile where T : DataFile<T, TU>, new() where TU : struct, IExcelRow<TU> {
    protected abstract string FileName { get; }

    public static T Load() {
        var configFileName = new T().FileName;

        Services.PluginLog.Debug($"Loading Data {configFileName}.datafile.json");
        return Data.LoadCharacterData<T>($"{configFileName}.datafile.json");
    }

    public void Save() {
        Services.PluginLog.Debug($"Saving Data {FileName}.datafile.json");
        Data.SaveCharacterData(this, $"{FileName}.datafile.json");
    }

    public Dictionary<uint, LuminaDataEntry<TU>> LuminaData = [];

    public virtual void Update() { }
}
