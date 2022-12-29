using System;
using System.Collections.Generic;
using System.IO;
using DailyDuty.Configuration.Components;
using DailyDuty.DataStructures;
using DailyDuty.Modules;
using KamiLib.ConfigMigration;
using KamiLib.Configuration;
using Newtonsoft.Json.Linq;

namespace DailyDuty.Configuration;

internal static class ConfigMigration
{
    public static CharacterConfiguration Convert(FileInfo filePath)
    {
        Migrate.LoadFile(filePath);

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
            InstanceNotifications = Migrate.GetSettingValue<bool>("WondrousTails.InstanceNotifications"),
            Enabled = Migrate.GetSettingValue<bool>("WondrousTails.Enabled"),
            NotifyOnZoneChange = Migrate.GetSettingValue<bool>("WondrousTails.ZoneChangeReminder"),
            NotifyOnLogin = Migrate.GetSettingValue<bool>("WondrousTails.LoginReminder"),
            EnableClickableLink = Migrate.GetSettingValue<bool>("WondrousTails.EnableOpenBookLink"),
        };
    }

    private static TreasureMapSettings GetTreasureMap()
    {
        return new TreasureMapSettings
        {
            LastMapGathered = Migrate.GetValue<DateTime>("TreasureMap.LastMapGathered"),
            NextReset = Migrate.GetValue<DateTime>("TreasureMap.NextReset"),
            Enabled = Migrate.GetSettingValue<bool>("TreasureMap.Enabled"),
            NotifyOnZoneChange = Migrate.GetSettingValue<bool>("TreasureMap.ZoneChangeReminder"),
            NotifyOnLogin = Migrate.GetSettingValue<bool>("TreasureMap.LoginReminder"),
        };
    }

    private static MiniCactpotSettings GetMiniCactpot()
    {
        return new MiniCactpotSettings
        {
            NextReset = Migrate.GetValue<DateTime>("MiniCactpot.NextReset"),
            Enabled = Migrate.GetSettingValue<bool>("MiniCactpot.Enabled"),
            NotifyOnZoneChange = Migrate.GetSettingValue<bool>("MiniCactpot.ZoneChangeReminder"),
            NotifyOnLogin = Migrate.GetSettingValue<bool>("MiniCactpot.LoginReminder"),
            EnableClickableLink = Migrate.GetSettingValue<bool>("MiniCactpot.EnableClickableLink"),
            TicketsRemaining = Migrate.GetValue<int>("MiniCactpot.TicketsRemaining"),
        };
    }

    private static LevequestSettings GetLevequest()
    {
        return new LevequestSettings
        {
            NextReset = Migrate.GetValue<DateTime>("Levequest.NextReset"),
            NotificationThreshold = Migrate.GetSettingValue<int>("Levequest.NotificationThreshold"),
            Enabled = Migrate.GetSettingValue<bool>("Levequest.Enabled"),
            NotifyOnZoneChange = Migrate.GetSettingValue<bool>("Levequest.ZoneChangeReminder"),
            NotifyOnLogin = Migrate.GetSettingValue<bool>("Levequest.LoginReminder"),
            ComparisonMode = Migrate.GetSettingEnum<ComparisonMode>("Levequest.ComparisonMode"),
        };
    }

    private static JumboCactpotSettings GetJumboCactpot()
    {
        return new JumboCactpotSettings
        {
            NextReset = Migrate.GetValue<DateTime>("JumboCactpot.NextReset"),
            Enabled = Migrate.GetSettingValue<bool>("JumboCactpot.Enabled"),
            NotifyOnZoneChange = Migrate.GetSettingValue<bool>("JumboCactpot.ZoneChangeReminder"),
            NotifyOnLogin = Migrate.GetSettingValue<bool>("JumboCactpot.LoginReminder"),
            EnableClickableLink = Migrate.GetSettingValue<bool>("JumboCactpot.EnableClickableLink"),
            Tickets = GetTickets("JumboCactpot.Tickets"),
        };
    }

    private static HuntMarksWeeklySettings GetHuntMarksWeekly()
    {
        return new HuntMarksWeeklySettings
        {
            NextReset = Migrate.GetValue<DateTime>("WeeklyHuntMarks.NextReset"),
            Enabled = Migrate.GetSettingValue<bool>("WeeklyHuntMarks.Enabled"),
            NotifyOnZoneChange = Migrate.GetSettingValue<bool>("WeeklyHuntMarks.ZoneChangeReminder"),
            NotifyOnLogin = Migrate.GetSettingValue<bool>("WeeklyHuntMarks.LoginReminder"),
            TodoUseLongLabel = Migrate.GetSettingValue<bool>("WeeklyHuntMarks.ExpandedDisplay"),
            TrackedHunts = GetTrackedHunts("WeeklyHuntMarks.TrackedHunts"),
        };
    }

    private static HuntMarksDailySettings GetHuntMarksDaily()
    {
        return new HuntMarksDailySettings
        {
            NextReset = Migrate.GetValue<DateTime>("DailyHuntMarks.NextReset"),
            Enabled = Migrate.GetSettingValue<bool>("DailyHuntMarks.Enabled"),
            NotifyOnZoneChange = Migrate.GetSettingValue<bool>("DailyHuntMarks.ZoneChangeReminder"),
            NotifyOnLogin = Migrate.GetSettingValue<bool>("DailyHuntMarks.LoginReminder"),
            TodoUseLongLabel = Migrate.GetSettingValue<bool>("DailyHuntMarks.ExpandedDisplay"),
            TrackedHunts = GetTrackedHunts("DailyHuntMarks.TrackedHunts"),
        };
    }

    private static FashionReportSettings GetFashionReport()
    {
        return new FashionReportSettings
        {
            AllowancesRemaining = Migrate.GetValue<int>("FashionReport.AllowancesRemaining"),
            HighestWeeklyScore = Migrate.GetValue<int>("FashionReport.HighestWeeklyScore"),
            NextReset = Migrate.GetValue<DateTime>("FashionReport.NextReset"),
            Enabled = Migrate.GetSettingValue<bool>("FashionReport.Enabled"),
            NotifyOnZoneChange = Migrate.GetSettingValue<bool>("FashionReport.ZoneChangeReminder"),
            NotifyOnLogin = Migrate.GetSettingValue<bool>("FashionReport.LoginReminder"),
            EnableClickableLink = Migrate.GetSettingValue<bool>("FashionReport.EnableClickableLink"),
            Mode = Migrate.GetSettingEnum<FashionReportMode>("FashionReport.Mode"),
        };
    }

    private static DutyRouletteSettings GetDutyRoulette()
    {
        return new DutyRouletteSettings
        {
            NextReset = Migrate.GetValue<DateTime>("DutyRoulette.NextReset"),
            Enabled = Migrate.GetSettingValue<bool>("DutyRoulette.Enabled"),
            NotifyOnZoneChange = Migrate.GetSettingValue<bool>("DutyRoulette.ZoneChangeReminder"),
            NotifyOnLogin = Migrate.GetSettingValue<bool>("DutyRoulette.LoginReminder"),
            EnableClickableLink = Migrate.GetSettingValue<bool>("DutyRoulette.EnableClickableLink"),
            HideExpertWhenCapped = Migrate.GetSettingValue<bool>("DutyRoulette.HideWhenCapped"),
            TodoUseLongLabel = Migrate.GetSettingValue<bool>("DutyRoulette.ExpandedDisplay"),
            TrackedRoulettes = GetTrackedRoulettes("DutyRoulette.TrackedRoulettes"),
        };
    }

    private static CharacterData GetCharacterData()
    {
        return new CharacterData
        {
            LocalContentID = Migrate.GetValue<ulong>("LocalContentID"),
            Name = Migrate.GetValue<string>("CharacterName"),
            World = Migrate.GetValue<string>("World"),
        };
    }

    private static DomanEnclaveSettings GetDomanEnclave()
    {
        return new DomanEnclaveSettings
        {
            NextReset = Migrate.GetValue<DateTime>("DomanEnclave.NextReset"),
            EnableClickableLink = Migrate.GetSettingValue<bool>("DomanEnclave.EnableClickableLink"),
            Enabled = Migrate.GetSettingValue<bool>("DomanEnclave.Enabled"),
            NotifyOnZoneChange = Migrate.GetSettingValue<bool>("DomanEnclave.ZoneChangeReminder"),
            NotifyOnLogin = Migrate.GetSettingValue<bool>("DomanEnclave.LoginReminder"),
        };
    }

    private static CustomDeliverySettings GetCustomDelivery()
    {
        return new CustomDeliverySettings
        {
            NextReset = Migrate.GetValue<DateTime>("CustomDelivery.NextReset"),
            NotificationThreshold = Migrate.GetSettingValue<int>("CustomDelivery.NotificationThreshold"),
            Enabled = Migrate.GetSettingValue<bool>("CustomDelivery.Enabled"),
            NotifyOnZoneChange = Migrate.GetSettingValue<bool>("CustomDelivery.ZoneChangeReminder"),
            NotifyOnLogin = Migrate.GetSettingValue<bool>("CustomDelivery.LoginReminder"),
            ComparisonMode = Migrate.GetSettingEnum<ComparisonMode>("CustomDelivery.ComparisonMode"),
        };
    }

    private static BeastTribeSettings GetBeastTribe()
    {
        return new BeastTribeSettings
        {
            NextReset = Migrate.GetValue<DateTime>("BeastTribe.NextReset"),
            NotificationThreshold = Migrate.GetSettingValue<int>("BeastTribe.NotificationThreshold"),
            Enabled = Migrate.GetSettingValue<bool>("BeastTribe.Enabled"),
            NotifyOnZoneChange = Migrate.GetSettingValue<bool>("BeastTribe.ZoneChangeReminder"),
            NotifyOnLogin = Migrate.GetSettingValue<bool>("BeastTribe.LoginReminder"),
            ComparisonMode = Migrate.GetSettingEnum<ComparisonMode>("BeastTribe.ComparisonMode"),
        };
    }

    private static TrackedRoulette[] GetTrackedRoulettes(string key)
    {
        var array = Migrate.GetArray(key);

        var resultArray = new TrackedRoulette[array.Count];

        for (var i = 0; i < array.Count; ++i)
        {
            var element = array[i];

            var tracked = element["Tracked"]!.Value<bool>();
            var completed = element["Completed"]!.Value<bool>();
            var type = element["Type"]!.Value<int>();

            resultArray[i] = new TrackedRoulette((RouletteType)type, new Setting<bool>(tracked), completed ? RouletteState.Complete : RouletteState.Incomplete);
        }

        return resultArray;
    }

    private static TrackedHunt[] GetTrackedHunts(string key)
    {
        var array = Migrate.GetArray(key);

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
        var array = Migrate.GetArray(key);

        return array.ToObject<List<int>>()!;
    }
}