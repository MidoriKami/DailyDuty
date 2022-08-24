using DailyDuty.Configuration;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.ModuleSettings;
using Newtonsoft.Json.Linq;

namespace DailyDuty.Utilities;

internal static class ConfigMigration
{
    private static JObject? _parsedJson;

    public static CharacterConfiguration Convert(string fileText)
    {
        _parsedJson = JObject.Parse(fileText);

        return new CharacterConfiguration
        {
            Version = 2,
            CharacterData = GetCharacterData(),
            
            BeastTribe = GetBeastTribe(),
            CustomDelivery = GetCustomDelivery(),
            DomanEnclave = GetDomanEnclave(),
        };
    }

    private static CharacterData GetCharacterData()
    {
        return new CharacterData
        {
            LocalContentID = GetRawValue<ulong>("LocalContentID"),
            Name = GetRawValue<string>("CharacterName"),
            World = GetRawValue<string>("World"),
        };
    }

    private static DomanEnclaveSettings GetDomanEnclave()
    {
        return new DomanEnclaveSettings
        {
            Enabled = GetValue<bool>("DomanEnclave.Enabled"),
            NotifyOnZoneChange = GetValue<bool>("DomanEnclave.ZoneChangeReminder"),
            NotifyOnLogin = GetValue<bool>("DomanEnclave.LoginReminder"),
            EnableClickableLink = GetValue<bool>("DomanEnclave.EnableClickableLink"),
        };
    }

    private static CustomDeliverySettings GetCustomDelivery()
    {
        return new CustomDeliverySettings
        {
            NotificationThreshold = GetValue<int>("CustomDelivery.NotificationThreshold"),
            Enabled = GetValue<bool>("CustomDelivery.Enabled"),
            NotifyOnZoneChange = GetValue<bool>("CustomDelivery.ZoneChangeReminder"),
            NotifyOnLogin = GetValue<bool>("CustomDelivery.LoginReminder")
        };
    }

    private static BeastTribeSettings GetBeastTribe()
    {
        return new BeastTribeSettings
        {
            NotificationThreshold = GetValue<int>("BeastTribe.NotificationThreshold"),
            Enabled = GetValue<bool>("BeastTribe.Enabled"),
            NotifyOnZoneChange = GetValue<bool>("BeastTribe.ZoneChangeReminder"),
            NotifyOnLogin = GetValue<bool>("BeastTribe.LoginReminder")
        };
    }

    private static Setting<T> GetValue<T>(string key) where T : struct
    {
        return new Setting<T>(_parsedJson!.SelectToken(key)!.Value<T>());
    }

    private static T GetRawValue<T>(string key)
    {
        return _parsedJson!.SelectToken(key)!.Value<T>()!;
    }

}