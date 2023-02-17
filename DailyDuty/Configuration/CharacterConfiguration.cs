using System;
using System.IO;
using DailyDuty.DataModels;
using DailyDuty.Modules;
using DailyDuty.UserInterface.Windows;
using Dalamud.Configuration;
using Dalamud.Logging;
using Newtonsoft.Json;

namespace DailyDuty.Configuration;

[Serializable]
internal class CharacterConfiguration : IPluginConfiguration
{
    public int Version { get; set; } = 2;

    public CharacterData CharacterData = new();
    public bool HideDisabledModulesInSelectWindow = false;

    public TodoOverlaySettings TodoOverlay = new();
    public TimersOverlaySettings TimersOverlay = new();

    public BeastTribeSettings BeastTribe = new();
    public CustomDeliverySettings CustomDelivery = new();
    public DomanEnclaveSettings DomanEnclave = new();
    public DutyRouletteSettings DutyRoulette = new();
    public FashionReportSettings FashionReport = new();
    public HuntMarksDailySettings HuntMarksDaily = new();
    public HuntMarksWeeklySettings HuntMarksWeekly = new();
    public JumboCactpotSettings JumboCactpot = new();
    public LevequestSettings Levequest = new();
    public MiniCactpotSettings MiniCactpot = new();
    public TreasureMapSettings TreasureMap = new();
    public WondrousTailsSettings WondrousTails = new();
    public ChallengeLogSettings ChallengeLog = new();
    public RaidsNormalSettings RaidsNormal = new();
    public RaidsAllianceSettings RaidsAlliance = new();
    public FauxHollowsSettings FauxHollows = new();
    public GrandCompanySupplySettings GrandCompanySupply = new();
    public GrandCompanyProvisionSettings GrandCompanyProvision = new();
    public MaskedCarnivaleSettings MaskedCarnivale = new();
    public GrandCompanySquadronSettings GrandCompanySquadron = new();

    public void Save(bool saveBackup = false)
    {
        if (CharacterData.LocalContentID != 0)
        {
            PluginLog.Verbose($"{DateTime.Now} - {CharacterData.Name} Saved");

            var configFileInfo = saveBackup ? GetBackupConfigFileInfo(CharacterData.LocalContentID) : GetConfigFileInfo(CharacterData.LocalContentID);

            var serializedContents = JsonConvert.SerializeObject(this, Formatting.Indented);

            using var writer = new StreamWriter(configFileInfo.FullName);
            writer.Write(serializedContents);
            writer.Close();
        }
        else
        {
            PluginLog.Warning("Tried to save a config with invalid LocalContentID, aborting save.");
        }
    }

    public void SaveBackup() => Save(true);
    private static FileInfo GetConfigFileInfo(ulong contentID) => new(Service.PluginInterface.ConfigDirectory.FullName + $@"\{contentID}.json");
    private static FileInfo GetBackupConfigFileInfo(ulong contentID) => new(Service.PluginInterface.ConfigDirectory.FullName + $@"\{contentID}.bak.json");

    public static CharacterConfiguration Load(ulong contentID)
    {
        // If configuration file exists for this character
        if (GetConfigFileInfo(contentID) is { Exists: true } configFileInfo)
        {
            // File exists, read all contents
            using var reader = new StreamReader(configFileInfo.FullName);
            var fileText = reader.ReadToEnd();
            reader.Close();

            // Deserialize, and get version
            return JsonConvert.DeserializeObject<IPluginConfiguration>(fileText)?.Version switch
            {
                // Config is correct version, load config
                2 => LoadExistingCharacterConfiguration(contentID, fileText),
                
                // If version is null due to corrupted json
                null => TryLoadBackupConfiguration(contentID),
                
                // Config wrong version, make new config, it's been long enough since we moved to version 2
                _ => CreateNewCharacterConfiguration()
            };
        }
        
        // If it doesn't exist, make it.
        else
        {
            return CreateNewCharacterConfiguration();
        }
    }
    private static CharacterConfiguration TryLoadBackupConfiguration(ulong contentID)
    {
        // If backup config file exists for this character
        if (GetBackupConfigFileInfo(contentID) is { Exists: true } backupConfigFileInfo)
        {
            // File exists, read all contents
            using var reader = new StreamReader(backupConfigFileInfo.FullName);
            var fileText = reader.ReadToEnd();
            reader.Close();
            
            // Double check that backup config file version is what we expect
            // Deserialize, and get version
            return JsonConvert.DeserializeObject<IPluginConfiguration>(fileText)?.Version switch
            {
                // Backup Config is correct version, load config the same way we would a normal config but give it the backup filetext
                2 => LoadExistingCharacterConfiguration(contentID, fileText),

                // Config is null or wrong version, make new config
                _ => CreateNewCharacterConfiguration()
            };
        }
        
        // Backup config doesn't exist, we already tried to load an exist config, we gotta nuke and retry
        else
        {
            return CreateNewCharacterConfiguration();
        }
    }

    private static CharacterConfiguration LoadExistingCharacterConfiguration(ulong contentID, string fileText)
    {
        var loadedCharacterConfiguration = JsonConvert.DeserializeObject<CharacterConfiguration>(fileText);

        if (loadedCharacterConfiguration == null)
        {
            throw new FileLoadException($"Unable to load configuration file for contentID: {contentID}");
        }
        
        loadedCharacterConfiguration.CharacterData.Update();
        loadedCharacterConfiguration.Save();
        return loadedCharacterConfiguration;
    }
    
    private static CharacterConfiguration CreateNewCharacterConfiguration()
    {
        var newCharacterConfiguration = new CharacterConfiguration();

        newCharacterConfiguration.Save();
        return newCharacterConfiguration;
    }
}