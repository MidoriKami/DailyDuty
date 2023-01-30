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

public class HuntMarksDailySettings : GenericSettings
{
    public TrackedHunt[] TrackedHunts = 
    {
        new(HuntMarkType.RealmRebornLevelOne, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.HeavenswardLevelOne, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.HeavenswardLevelTwo, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.HeavenswardLevelThree, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.StormbloodLevelOne, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.StormbloodLevelTwo, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.StormbloodLevelThree, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.ShadowbringersLevelOne, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.ShadowbringersLevelTwo, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.ShadowbringersLevelThree, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.EndwalkerLevelOne, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.EndwalkerLevelTwo, TrackedHuntState.Unobtained, new Setting<bool>(false)),
        new(HuntMarkType.EndwalkerLevelThree, TrackedHuntState.Unobtained, new Setting<bool>(false)),
    };
}

public class HuntMarksDaily : AbstractModule
{
    public override ModuleName Name => ModuleName.HuntMarksDaily;

    private static HuntMarksDailySettings Settings => Service.ConfigurationManager.CharacterConfiguration.HuntMarksDaily;
    public override GenericSettings GenericSettings => Settings;

    public HuntMarksDaily()
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

    public override void DoReset()
    {
        foreach (var hunt in Settings.TrackedHunts)
        {
            hunt.State = TrackedHuntState.Unobtained;
        }
    }
    
    public override string GetStatusMessage() => $"{GetIncompleteCount()} {Strings.HuntMarks_Remaining}";
    public override ModuleStatus GetModuleStatus() => GetIncompleteCount() == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
    private static int GetIncompleteCount() => Settings.TrackedHunts.Count(hunt => hunt.Tracked && hunt.State != TrackedHuntState.Killed);
    public override CompletionType CompletionType => CompletionType.Daily;
    public override bool HasLongLabel => true;
    
    public override string GetLongTaskLabel()
    {
        var strings = Settings.TrackedHunts
            .Where(hunt => hunt.Tracked && hunt.State != TrackedHuntState.Killed)
            .Select(hunt => hunt.HuntType.GetLabel())
            .ToList();

        return strings.Any() ? string.Join("\n", strings) : Strings.HuntMarks_DailyLabel;
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
                .BeginTable(0.60f)
                .AddDataRows(Settings.TrackedHunts.Where(row => row.Tracked))
                .EndTable()
                .Draw();
        }
        else
        {
            InfoBox.Instance
                .AddTitle(Strings.HuntMarks_Status, out var innerWidth)
                .AddStringCentered(Strings.HuntMarks_NothingTracked, innerWidth, Colors.Orange)
                .Draw();
        }
            
        InfoBox.Instance.DrawSuppressionOption(this);
    }
}