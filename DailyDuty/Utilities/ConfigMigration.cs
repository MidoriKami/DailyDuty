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
            DutyRoulette = GetDutyRoulette(),
            FashionReport = GetFashionReport(),
            HuntMarksDaily = GetHuntMarksDaily(),
        };
    }

    private static HuntMarksDailySettings GetHuntMarksDaily()
    {
        return new HuntMarksDailySettings
        {
            Enabled = GetSettingValue<bool>("DailyHuntMarks.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("DailyHuntMarks.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("DailyHuntMarks.LoginReminder"),
            TodoUseLongLabel = GetSettingValue<bool>("DailyHuntMarks.ExpandedDisplay"),
        };
    }

    private static FashionReportSettings GetFashionReport()
    {
        return new FashionReportSettings
        {
            Enabled = GetSettingValue<bool>("DutyRoulette.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("DutyRoulette.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("DutyRoulette.LoginReminder"),
            EnableClickableLink = GetSettingValue<bool>("DutyRoulette.EnableClickableLink"),
        };
    }

    private static DutyRouletteSettings GetDutyRoulette()
    {
        return new DutyRouletteSettings
        {
            Enabled = GetSettingValue<bool>("DutyRoulette.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("DutyRoulette.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("DutyRoulette.LoginReminder"),
            EnableClickableLink = GetSettingValue<bool>("DutyRoulette.EnableClickableLink"),
            HideExpertWhenCapped = GetSettingValue<bool>("DutyRoulette.HideWhenCapped"),
            TodoUseLongLabel = GetSettingValue<bool>("DutyRoulette.ExpandedDisplay"),
        };
    }

    private static CharacterData GetCharacterData()
    {
        return new CharacterData
        {
            LocalContentID = GetValue<ulong>("LocalContentID"),
            Name = GetValue<string>("CharacterName"),
            World = GetValue<string>("World"),
        };
    }

    private static DomanEnclaveSettings GetDomanEnclave()
    {
        return new DomanEnclaveSettings
        {
            Enabled = GetSettingValue<bool>("DomanEnclave.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("DomanEnclave.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("DomanEnclave.LoginReminder"),
            EnableClickableLink = GetSettingValue<bool>("DomanEnclave.EnableClickableLink"),
        };
    }

    private static CustomDeliverySettings GetCustomDelivery()
    {
        return new CustomDeliverySettings
        {
            NotificationThreshold = GetSettingValue<int>("CustomDelivery.NotificationThreshold"),
            Enabled = GetSettingValue<bool>("CustomDelivery.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("CustomDelivery.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("CustomDelivery.LoginReminder")
        };
    }

    private static BeastTribeSettings GetBeastTribe()
    {
        return new BeastTribeSettings
        {
            NotificationThreshold = GetSettingValue<int>("BeastTribe.NotificationThreshold"),
            Enabled = GetSettingValue<bool>("BeastTribe.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("BeastTribe.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("BeastTribe.LoginReminder")
        };
    }

    private static Setting<T> GetSettingValue<T>(string key) where T : struct
    {
        return new Setting<T>(_parsedJson!.SelectToken(key)!.Value<T>());
    }

    private static T GetValue<T>(string key)
    {
        return _parsedJson!.SelectToken(key)!.Value<T>()!;
    }

}