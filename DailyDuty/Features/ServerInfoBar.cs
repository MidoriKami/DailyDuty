using System;
using System.Threading.Tasks;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Plugin.Services;
using KamiToolKit.Components.Builders;

namespace DailyDuty.Features;

public class ServerInfoBar : Feature<ServerInfoBarConfig>, IFrameworkUpdatable {

    public override string DisplayName => Strings.ServerInfoBar_DisplayName;

    public override ServerInfoBarConfig Config { get; protected set; } = null!;

    private IDtrBarEntry? daily;
    private IDtrBarEntry? weekly;
    private IDtrBarEntry? combo;

    public override async Task EnableAsync() {
        Config = await ServerInfoBarConfig.Load();
    }

    public override Task DisableAsync() {
        daily?.Remove();
        daily = null;

        weekly?.Remove();
        weekly = null;

        combo?.Remove();
        combo = null;

        return Task.CompletedTask;
    }

    public override void BuildConfigUi(NodeBuilder builder) => builder
        .Spacer(5.0f)
        .Row(Strings.ServerInfoBar_DailyTimer, rowBuilder => rowBuilder
            .Checkbox("", Config.SoloDaily, val => {
                Config.SoloDaily = val;
                Config.Save();
            }))
        .Row(Strings.ServerInfoBar_WeeklyTimer, rowBuilder => rowBuilder
            .Checkbox("", Config.SoloWeekly, val => {
                Config.SoloWeekly = val;
                Config.Save();
            }))
        .Row(Strings.ServerInfoBar_ToggleableTimer, rowBuilder => rowBuilder
            .Checkbox("", Config.Combo, val => {
                Config.Combo = val;
                Config.Save();
            }))
        .Spacer(5.0f)
        .Row(Strings.TimersOverlay_HideSeconds, rowBuilder => rowBuilder
            .Checkbox("", Config.HideSeconds, val => {
                Config.HideSeconds = val;
                Config.Save();
            }));

    void IFrameworkUpdatable.Update() {
        if (!Enabled) return;

        var nextDailyReset = Time.NextDailyReset();
        var nextWeeklyReset = Time.NextWeeklyReset();

        var timeUntilDailyReset = nextDailyReset - DateTime.UtcNow;
        var timeUntilWeeklyReset = nextWeeklyReset - DateTime.UtcNow;

        // Solo Daily entry
        if (daily is null && Config.SoloDaily) {
            daily = IDtrBar.Get().Get("DailyDuty - Daily Timer");
            daily.OnClick = _ => System.ConfigWindow.Toggle();
            daily.Tooltip = Strings.ServerInfoBar_ClickOpenConfig;
        }

        if (daily is not null && !Config.SoloDaily) {
            daily.Remove();
            daily = null;
        }

        // Solo Weekly entry
        if (weekly is null && Config.SoloWeekly) {
            weekly = IDtrBar.Get().Get("DailyDuty - Weekly Timer");
            weekly.OnClick = _ => System.ConfigWindow.Toggle();
            weekly.Tooltip = Strings.ServerInfoBar_ClickOpenConfig;
        }

        if (weekly is not null && !Config.SoloWeekly) {
            weekly.Remove();
            weekly = null;
        }

        // Combo entry
        if (combo is null && Config.Combo) {
            combo = IDtrBar.Get().Get("DailyDuty - Combo Timer");
            combo.OnClick = OnComboClick;
            combo.Tooltip = Strings.ServerInfoBar_LeftClickChange + "\n" +
                            Strings.ServerInfoBar_RightClickOpenConfig;
        }

        if (combo is not null && !Config.Combo) {
            combo.Remove();
            combo = null;
        }

        // Update text on entries
        daily?.Text = $"{Strings.Additional_DailyReset}: {timeUntilDailyReset.FormatTime(Config.HideSeconds)}";
        weekly?.Text = $"{Strings.Additional_WeeklyReset}: {timeUntilWeeklyReset.FormatTime(Config.HideSeconds)}";

        if (combo is not null) {
            var timeText = Config.CurrentMode is DtrMode.Daily
                ? timeUntilDailyReset.FormatTime(Config.HideSeconds)
                : timeUntilWeeklyReset.FormatTime(Config.HideSeconds);
            var label = Config.CurrentMode is DtrMode.Daily
                ? Strings.Additional_DailyReset
                : Strings.Additional_WeeklyReset;
            combo.Text = $"{label}: {timeText}";
        }
    }

    private void OnComboClick(DtrInteractionEvent obj) {
        switch (obj.ClickType) {
            case MouseClickType.Left:
                Config.CurrentMode = Config.CurrentMode is DtrMode.Daily ? DtrMode.Weekly : DtrMode.Daily;
                Config.Save();
                break;

            case MouseClickType.Right:
                System.ConfigWindow.Toggle();
                break;
        }
    }
}

public class ServerInfoBarConfig : FeatureConfigFile<ServerInfoBarConfig> {
    protected override string FileName => "ServerInfoBar.config.json";

    public bool SoloDaily { get; set; }
    public bool SoloWeekly { get; set; }
    public bool Combo { get; set; } = true;
    public bool HideSeconds { get; set; }

    public DtrMode CurrentMode { get; set; } = DtrMode.Daily;
}
