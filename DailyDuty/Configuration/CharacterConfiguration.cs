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

    public void Save()
    {
        if (CharacterData.LocalContentID != 0)
        {
            PluginLog.Verbose($"{DateTime.Now} - {CharacterData.Name} Saved");

            var configFileInfo = GetConfigFileInfo(CharacterData.LocalContentID);

            var serializedContents = JsonConvert.SerializeObject(this, Formatting.Indented);

            var writer = new StreamWriter(new FileStream(configFileInfo.FullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite));
            writer.Write(serializedContents);
            writer.Dispose();
        }
        else
        {
            PluginLog.Warning("Tried to save a config with invalid LocalContentID, aborting save.");
        }
    }

    private static FileInfo GetConfigFileInfo(ulong contentID)
    {
        var pluginConfigDirectory = Service.PluginInterface.ConfigDirectory;

        return new FileInfo(pluginConfigDirectory.FullName + $@"\{contentID}.json");
    }

    public static CharacterConfiguration Load(ulong contentID)
    {
        if (GetConfigFileInfo(contentID) is { Exists: true } configFileInfo)
        {
            return Migrate.GetFileVersion(configFileInfo) switch
            {
                2 => LoadExistingCharacterConfiguration(contentID, configFileInfo),
                1 => GenerateMigratedCharacterConfiguration(),
                _ => CreateNewCharacterConfiguration()
            };
        }
        else
        {
            return CreateNewCharacterConfiguration();
        }
    }

    private static CharacterConfiguration LoadExistingCharacterConfiguration(ulong contentID, FileSystemInfo configFileInfo)
    {
        var reader = new StreamReader(new FileStream(configFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        var fileText = reader.ReadToEnd();
        reader.Dispose();
        
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