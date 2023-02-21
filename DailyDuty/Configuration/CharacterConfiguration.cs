using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using DailyDuty.DataModels;
using DailyDuty.Modules;
using DailyDuty.UserInterface.Components;
using Dalamud.Configuration;
using Dalamud.Logging;
using Newtonsoft.Json;

namespace DailyDuty.Configuration;

[Serializable]
public class CharacterConfiguration : IPluginConfiguration
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
            
            SaveConfigFile(GetConfigFileInfo(CharacterData.LocalContentID, saveBackup));
        }
    }

    public void SaveBackup() => Save(true);
    
    public static CharacterConfiguration Load(ulong contentID)
    {
        try
        {
            var mainConfigFileInfo = GetConfigFileInfo(contentID, false);
            var backupConfigFileInfo = GetConfigFileInfo(contentID, true);

            return TryLoadConfiguration(mainConfigFileInfo, backupConfigFileInfo);
        }
        catch (Exception e)
        {
            PluginLog.Warning(e, $"Exception Occured during loading Character {contentID}. Loading new default config instead.");
            return new CharacterConfiguration();
        }
    }
    
    private static CharacterConfiguration TryLoadConfiguration(FileSystemInfo? mainConfigInfo = null, FileSystemInfo? backupConfigInfo = null)
    {
        try
        {
            if (TryLoadSpecificConfiguration(mainConfigInfo, out var mainCharacterConfiguration)) return mainCharacterConfiguration;
        }
        catch (Exception e)
        {
            PluginLog.Warning(e, "Exception Occured loading Main Configuration. Attempting to load Backup Config.");
        }
        
        if (TryLoadSpecificConfiguration(backupConfigInfo, out var backupCharacterConfiguration)) return backupCharacterConfiguration;

        return new CharacterConfiguration();
    }
    
    private static bool TryLoadSpecificConfiguration(FileSystemInfo? fileInfo, [NotNullWhen(true)] out CharacterConfiguration? info)
    {
        if (fileInfo is null || !fileInfo.Exists)
        {
            info = null;
            return false;
        }
        
        info = JsonConvert.DeserializeObject<CharacterConfiguration>(LoadFile(fileInfo));
        return info is not null;
    }
    
    private static FileInfo GetConfigFileInfo(ulong contentId, bool backupFile) => new(Service.PluginInterface.ConfigDirectory.FullName + $@"\{contentId}{(backupFile ? ".bak" : string.Empty)}.json");

    private static string LoadFile(FileSystemInfo fileInfo)
    {
        using var reader = new StreamReader(fileInfo.FullName);
        return reader.ReadToEnd();
    }

    private static void SaveFile(FileSystemInfo file, string fileText)
    {
        using var writer = new StreamWriter(file.FullName);
        writer.Write(fileText);
    }

    private void SaveConfigFile(FileSystemInfo file)
    {
        var text = JsonConvert.SerializeObject(this, Formatting.Indented);
        SaveFile(file, text);
    }
}