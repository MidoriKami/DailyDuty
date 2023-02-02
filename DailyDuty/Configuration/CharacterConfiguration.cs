using System;
using System.IO;
using DailyDuty.DataModels;
using DailyDuty.Modules;
using DailyDuty.UserInterface.Windows;
using Dalamud.Logging;
using KamiLib.Configuration;
using Newtonsoft.Json;

namespace DailyDuty.Configuration;

[Serializable]
internal class CharacterConfiguration
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

            var configFileInfo = saveBackup ? GetConfigFileInfo(CharacterData.LocalContentID) : GetBackupConfigFileInfo(CharacterData.LocalContentID);

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
    
    private static FileInfo GetConfigFileInfo(ulong contentID)
    {
        var pluginConfigDirectory = Service.PluginInterface.ConfigDirectory;

        return new FileInfo(pluginConfigDirectory.FullName + $@"\{contentID}.json");
    }
    
    private static FileInfo GetBackupConfigFileInfo(ulong contentID)
    {
        var pluginConfigDirectory = Service.PluginInterface.ConfigDirectory;

        return new FileInfo(pluginConfigDirectory.FullName + $@"\{contentID}.bak.json");
    }

    public static CharacterConfiguration Load(ulong contentID)
    {
        if (GetConfigFileInfo(contentID) is { Exists: true } configFileInfo)
        {
            using var reader = new StreamReader(configFileInfo.FullName);
            var fileText = reader.ReadToEnd();
            reader.Close();
                
            Migrate.ParseJObject(fileText);
            
            return Migrate.GetFileVersion() switch
            {
                2 => LoadExistingCharacterConfiguration(contentID, fileText),
                1 => GenerateMigratedCharacterConfiguration(),
                _ => CreateNewCharacterConfiguration()
            };
        }
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
    
    private static CharacterConfiguration GenerateMigratedCharacterConfiguration()
    {
        CharacterConfiguration migratedConfiguration;

        try
        {
            migratedConfiguration = ConfigMigration.Convert();
            migratedConfiguration.CharacterData.Update();
            migratedConfiguration.Save();
        }
        catch (Exception e)
        {
            PluginLog.Warning(e, "Unable to Migrate Configuration, generating new configuration instead.");
            migratedConfiguration = CreateNewCharacterConfiguration();
        }

        return migratedConfiguration;
    }

    private static CharacterConfiguration CreateNewCharacterConfiguration()
    {
        var newCharacterConfiguration = new CharacterConfiguration();

        newCharacterConfiguration.Save();
        return newCharacterConfiguration;
    }
}