using System;
using System.IO;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.ModuleSettings;
using DailyDuty.Configuration.OverlaySettings;
using DailyDuty.Utilities;
using Newtonsoft.Json;

namespace DailyDuty.Configuration;

[Serializable]
internal class CharacterConfiguration
{
    public int Version { get; set; } = 2;

    public CharacterData CharacterData = new();

    public TodoOverlaySettings TodoOverlay = new();
    public TimersOverlaySettings TimersOverlay = new();

    public BeastTribeSettings BeastTribe = new();
    public CustomDeliverySettings CustomDelivery = new();

    public void Save()
    {
        if (CharacterData.LocalContentID != 0)
        {
            Log.Verbose($"{DateTime.Now} - {CharacterData.Name} Saved");

            var configFileInfo = GetConfigFileInfo(CharacterData.LocalContentID);

            var serializedContents = JsonConvert.SerializeObject(this, Formatting.Indented);

            File.WriteAllText(configFileInfo.FullName, serializedContents);
        }
    }

    private static FileInfo GetConfigFileInfo(ulong contentID)
    {
        var pluginConfigDirectory = Service.PluginInterface.ConfigDirectory;

        return new FileInfo(pluginConfigDirectory.FullName + $@"\{contentID}.json");
    }

    public static CharacterConfiguration Load(ulong contentID)
    {
        var configFileInfo = GetConfigFileInfo(contentID);

        if (configFileInfo.Exists)
        {
            var fileText = File.ReadAllText(configFileInfo.FullName);

            var loadedCharacterConfiguration = JsonConvert.DeserializeObject<CharacterConfiguration>(fileText);

            if (loadedCharacterConfiguration == null)
            {
                throw new FileLoadException($"Unable to load configuration file for contentID: {contentID}");
            }

            return loadedCharacterConfiguration;
        }
        else
        {
            var newCharacterConfiguration = new CharacterConfiguration();

            var playerData = Service.ClientState.LocalPlayer;
            var contentId = Service.ClientState.LocalContentId;

            var playerName = playerData?.Name.TextValue ?? "Unknown";
            var playerWorld = playerData?.HomeWorld.GameData?.Name.ToString() ?? "UnknownWorld";

            newCharacterConfiguration.CharacterData = new CharacterData()
            {
                Name = playerName,
                LocalContentID = contentId,
                World = playerWorld,
            };

            newCharacterConfiguration.Save();
            return newCharacterConfiguration;
        }
    }

}