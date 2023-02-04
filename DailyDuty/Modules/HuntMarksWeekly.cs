using System.Linq;
using DailyDuty.DataModels;
using DailyDuty.DataStructures.HuntMarks;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Game;
using KamiLib.Configuration;
using KamiLib.Drawing;

namespace DailyDuty.Modules;

public class HuntMarksWeeklySettings : GenericSettings
{
    public TrackedHunt[] TrackedHunts = 
    {
        new(HuntMarkType.RealmRebornElite, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.HeavenswardElite, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.StormbloodElite, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.ShadowbringersElite, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.EndwalkerElite, TrackedHuntState.Unobtained, new Setting<bool>(false)),
    };
}

public class HuntMarksWeekly : Module
{
    public override ModuleName Name => ModuleName.HuntMarksWeekly;

    private static HuntMarksWeeklySettings Settings => Service.ConfigurationManager.CharacterConfiguration.HuntMarksWeekly;
    public override GenericSettings GenericSettings => Settings;

    public HuntMarksWeekly()
    {
        Service.Framework.Update += OnFrameworkUpdate;
    }

    public override void Dispose()
    {
        Service.Framework.Update -= OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        if (!Service.ConfigurationManager.CharacterDataLoaded) return;

        foreach (var hunt in Settings.TrackedHunts)
        {
            UpdateState(hunt);
        }
    }

    public override void DoReset()
    {
        foreach (var hunt in Settings.TrackedHunts)
        {
            hunt.State = TrackedHuntState.Unobtained;
        }
    }

    private static void UpdateState(TrackedHunt hunt)
    {
        var data = HuntMarkData.Instance.GetHuntData(hunt.HuntType);

        switch (hunt.State)
        {
            case TrackedHuntState.Unobtained when data.Obtained:
                hunt.State = TrackedHuntState.Obtained;
                Service.ConfigurationManager.Save();
                break;

            case TrackedHuntState.Obtained when data is { Obtained: false, IsCompleted: false }:
                hunt.State = TrackedHuntState.Unobtained;
                Service.ConfigurationManager.Save();
                break;

            case TrackedHuntState.Obtained when data.IsCompleted:
                hunt.State = TrackedHuntState.Killed;
                Service.ConfigurationManager.Save();
                break;
        }
    }

    public override string GetStatusMessage() => $"{GetIncompleteCount()} {Strings.HuntMarks_Remaining}";
    public override ModuleStatus GetModuleStatus() => GetIncompleteCount() == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    private static int GetIncompleteCount() => Settings.TrackedHunts.Count(hunt => hunt.Tracked && hunt.State != TrackedHuntState.Killed);
    public override CompletionType CompletionType => CompletionType.Weekly;
    public override bool HasLongLabel => true;

    public override string GetLongTaskLabel()
    {
        var strings = Settings.TrackedHunts
            .Where(hunt => hunt.Tracked && hunt.State != TrackedHuntState.Killed)
            .Select(hunt => hunt.HuntType.GetLabel())
            .ToList();

        return strings.Any() ? string.Join("\n", strings) : Strings.HuntMarks_WeeklyLabel;
    }
    
    protected override void DrawConfiguration()
    {
        InfoBox.Instance.DrawGenericSettings(this);

        InfoBox.Instance
            .AddTitle(Strings.HuntMarks_Tracked)
            .AddList(Settings.TrackedHunts)
            .Draw();

        InfoBox.Instance.DrawNotificationOptions(this);
    }

    protected override void DrawStatus()
    {
        InfoBox.Instance.DrawGenericStatus(this);

        if (Settings.TrackedHunts.Any(hunt => hunt.Tracked))
        {
            InfoBox.Instance
                .AddTitle(Strings.HuntMarks_Status)
                .BeginTable()
                .AddDataRows(Settings.TrackedHunts.Where(row => row.Tracked))
                .EndTable()
                .Draw();
        }
        else
        {
            InfoBox.Instance
                .AddTitle(Strings.HuntMarks_Status, out var innerWidth2)
                .AddStringCentered(Strings.HuntMarks_NothingTracked, innerWidth2, Colors.Orange)
                .Draw();
        }
            
        InfoBox.Instance.DrawSuppressionOption(this);
    }
}
