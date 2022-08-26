using System;
using System.Collections.Generic;
using DailyDuty.Configuration;
using DailyDuty.Configuration.Components;
using DailyDuty.Configuration.Enums;
using DailyDuty.Configuration.ModuleSettings;
using DailyDuty.DataStructures;
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
            HuntMarksWeekly = GetHuntMarksWeekly(),
            JumboCactpot = GetJumboCactpot(),
            Levequest = GetLevequest(),
            MiniCactpot = GetMiniCactpot(),
            TreasureMap = GetTreasureMap(),
            WondrousTails = GetWondrousTails(),
        };
    }

    private static WondrousTailsSettings GetWondrousTails()
    {
        return new WondrousTailsSettings
        {
            InstanceNotifications = GetSettingValue<bool>("WondrousTails.InstanceNotifications"),
            Enabled = GetSettingValue<bool>("WondrousTails.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("WondrousTails.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("WondrousTails.LoginReminder"),
            EnableClickableLink = GetSettingValue<bool>("WondrousTails.EnableOpenBookLink"),
        };
    }

    private static TreasureMapSettings GetTreasureMap()
    {
        return new TreasureMapSettings
        {
            LastMapGathered = GetValue<DateTime>("TreasureMap.LastMapGathered"),
            NextReset = GetValue<DateTime>("TreasureMap.NextReset"),
            Enabled = GetSettingValue<bool>("TreasureMap.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("TreasureMap.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("TreasureMap.LoginReminder"),
        };
    }

    private static MiniCactpotSettings GetMiniCactpot()
    {
        return new MiniCactpotSettings
        {
            NextReset = GetValue<DateTime>("MiniCactpot.NextReset"),
            Enabled = GetSettingValue<bool>("MiniCactpot.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("MiniCactpot.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("MiniCactpot.LoginReminder"),
            EnableClickableLink = GetSettingValue<bool>("MiniCactpot.EnableClickableLink"),
            TicketsRemaining = GetValue<int>("MiniCactpot.TicketsRemaining"),
        };
    }

    private static LevequestSettings GetLevequest()
    {
        return new LevequestSettings
        {
            NextReset = GetValue<DateTime>("Levequest.NextReset"),
            NotificationThreshold = GetSettingValue<int>("Levequest.NotificationThreshold"),
            Enabled = GetSettingValue<bool>("Levequest.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("Levequest.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("Levequest.LoginReminder"),
            ComparisonMode = GetSettingEnum<ComparisonMode>("Levequest.ComparisonMode"),
        };
    }

    private static JumboCactpotSettings GetJumboCactpot()
    {
        return new JumboCactpotSettings
        {
            NextReset = GetValue<DateTime>("JumboCactpot.NextReset"),
            Enabled = GetSettingValue<bool>("JumboCactpot.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("JumboCactpot.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("JumboCactpot.LoginReminder"),
            EnableClickableLink = GetSettingValue<bool>("JumboCactpot.EnableClickableLink"),
            Tickets = GetTickets("JumboCactpot.Tickets"),
        };
    }

    private static HuntMarksWeeklySettings GetHuntMarksWeekly()
    {
        return new HuntMarksWeeklySettings
        {
            NextReset = GetValue<DateTime>("WeeklyHuntMarks.NextReset"),
            Enabled = GetSettingValue<bool>("WeeklyHuntMarks.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("WeeklyHuntMarks.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("WeeklyHuntMarks.LoginReminder"),
            TodoUseLongLabel = GetSettingValue<bool>("WeeklyHuntMarks.ExpandedDisplay"),
            TrackedHunts = GetTrackedHunts("WeeklyHuntMarks.TrackedHunts"),
        };
    }

    private static HuntMarksDailySettings GetHuntMarksDaily()
    {
        return new HuntMarksDailySettings
        {
            NextReset = GetValue<DateTime>("DailyHuntMarks.NextReset"),
            Enabled = GetSettingValue<bool>("DailyHuntMarks.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("DailyHuntMarks.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("DailyHuntMarks.LoginReminder"),
            TodoUseLongLabel = GetSettingValue<bool>("DailyHuntMarks.ExpandedDisplay"),
            TrackedHunts = GetTrackedHunts("DailyHuntMarks.TrackedHunts"),
        };
    }

    private static FashionReportSettings GetFashionReport()
    {
        return new FashionReportSettings
        {
            AllowancesRemaining = GetValue<int>("FashionReport.AllowancesRemaining"),
            HighestWeeklyScore = GetValue<int>("FashionReport.HighestWeeklyScore"),
            NextReset = GetValue<DateTime>("FashionReport.NextReset"),
            Enabled = GetSettingValue<bool>("FashionReport.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("FashionReport.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("FashionReport.LoginReminder"),
            EnableClickableLink = GetSettingValue<bool>("FashionReport.EnableClickableLink"),
            Mode = GetSettingEnum<FashionReportMode>("FashionReport.Mode"),
        };
    }

    private static DutyRouletteSettings GetDutyRoulette()
    {
        return new DutyRouletteSettings
        {
            NextReset = GetValue<DateTime>("DutyRoulette.NextReset"),
            Enabled = GetSettingValue<bool>("DutyRoulette.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("DutyRoulette.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("DutyRoulette.LoginReminder"),
            EnableClickableLink = GetSettingValue<bool>("DutyRoulette.EnableClickableLink"),
            HideExpertWhenCapped = GetSettingValue<bool>("DutyRoulette.HideWhenCapped"),
            TodoUseLongLabel = GetSettingValue<bool>("DutyRoulette.ExpandedDisplay"),
            TrackedRoulettes = GetTrackedRoulettes("DutyRoulette.TrackedRoulettes"),
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
            NextReset = GetValue<DateTime>("DomanEnclave.NextReset"),
            EnableClickableLink = GetSettingValue<bool>("DomanEnclave.EnableClickableLink"),
            Enabled = GetSettingValue<bool>("DomanEnclave.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("DomanEnclave.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("DomanEnclave.LoginReminder"),
        };
    }

    private static CustomDeliverySettings GetCustomDelivery()
    {
        return new CustomDeliverySettings
        {
            NextReset = GetValue<DateTime>("CustomDelivery.NextReset"),
            NotificationThreshold = GetSettingValue<int>("CustomDelivery.NotificationThreshold"),
            Enabled = GetSettingValue<bool>("CustomDelivery.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("CustomDelivery.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("CustomDelivery.LoginReminder"),
            ComparisonMode = GetSettingEnum<ComparisonMode>("CustomDelivery.ComparisonMode"),
        };
    }

    private static BeastTribeSettings GetBeastTribe()
    {
        return new BeastTribeSettings
        {
            NextReset = GetValue<DateTime>("BeastTribe.NextReset"),
            NotificationThreshold = GetSettingValue<int>("BeastTribe.NotificationThreshold"),
            Enabled = GetSettingValue<bool>("BeastTribe.Enabled"),
            NotifyOnZoneChange = GetSettingValue<bool>("BeastTribe.ZoneChangeReminder"),
            NotifyOnLogin = GetSettingValue<bool>("BeastTribe.LoginReminder"),
            ComparisonMode = GetSettingEnum<ComparisonMode>("BeastTribe.ComparisonMode"),
        };
    }

    private static Setting<T> GetSettingValue<T>(string key) where T : struct
    {
        return new Setting<T>(_parsedJson!.SelectToken(key)!.Value<T>());
    }

    private static Setting<T> GetSettingEnum<T>(string key) where T : struct
    {
        var readValue = _parsedJson!.SelectToken(key)!.Value<int>();

        return new Setting<T>((T) Enum.ToObject(typeof(T), readValue));
    }

    private static T GetValue<T>(string key)
    {
        return _parsedJson!.SelectToken(key)!.Value<T>()!;
    }

    private static JArray GetArray(string key)
    {
        return (JArray) _parsedJson!.SelectToken(key)!;
    }

    private static TrackedRoulette[] GetTrackedRoulettes(string key)
    {
        var array = GetArray(key);

        var resultArray = new TrackedRoulette[array.Count];

        for (var i = 0; i < array.Count; ++i)
        {
            var element = array[i];

            var tracked = element["Tracked"]!.Value<bool>();
            var completed = element["Completed"]!.Value<bool>();
            var type = element["Type"]!.Value<int>();

            resultArray[i] = new TrackedRoulette((RouletteType)type, new Setting<bool>(tracked), completed);
        }

        return resultArray;
    }

    private static TrackedHunt[] GetTrackedHunts(string key)
    {
        var array = GetArray(key);

        var resultArray = new TrackedHunt[array.Count];

        for (var i = 0; i < array.Count; ++i)
        {
            var element = array[i];

            var tracked = element["Tracked"]!.Value<bool>();
            var state = element["State"]!.Value<int>();
            var type = element["Type"]!.Value<int>();

            resultArray[i] = new TrackedHunt((HuntMarkType)type, (TrackedHuntState)state, new Setting<bool>(tracked));
        }

        return resultArray;
    }

    private static List<int> GetTickets(string key)
    {
        var array = GetArray(key);

        return array.ToObject<List<int>>()!;
    }
}