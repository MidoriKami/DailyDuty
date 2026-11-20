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
        .Checkbox(Strings.ServerInfoBar_DailyTimer, Config.SoloDaily, val => {
            Config.SoloDaily = val;
            Config.Save();
        })
        .Checkbox(Strings.ServerInfoBar_WeeklyTimer, Config.SoloWeekly, val => {
            Config.SoloWeekly = val;
            Config.Save();
        })
        .Checkbox(Strings.ServerInfoBar_ToggleableTimer, Config.Combo, val => {
            Config.Combo = val;
            Config.Save();
        })
        .Spacer(5.0f)
        .Checkbox(Strings.TimersOverlay_HideSeconds, Config.HideSeconds, val => {
            Config.HideSeconds = val;
            Config.Save();
        });

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
        daily?.Text = $"{Strings.Additional_DailyReset}: {FormatTime(timeUntilDailyReset)}";
        weekly?.Text = $"{Strings.Additional_WeeklyReset}: {FormatTime(timeUntilWeeklyReset)}";

        if (combo is not null) {
            var timeText = Config.CurrentMode is DtrMode.Daily
                ? FormatTime(timeUntilDailyReset)
                : FormatTime(timeUntilWeeklyReset);
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

    private string FormatTime(TimeSpan timeRemaining) {
        if (Config.HideSeconds) {
            return $"{timeRemaining:d\\.hh\\:mm}";
        }

        return $"{timeRemaining:d\\.hh\\:mm\\:ss}";
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
