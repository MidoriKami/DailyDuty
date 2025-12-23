using System;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Extensions;
using DailyDuty.Utilities;
using Dalamud.Game.Gui.Dtr;
using KamiToolKit;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Features.ServerInfoBar;

public class ServerInfoBar : Module<ServerInfoBarConfig, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Server Info Bar",
        FileName = "DTR",
        Type = ModuleType.GeneralFeatures,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation")
        ],
        Tags = [ "DTR" ],
    };

    private IDtrBarEntry? daily;
    private IDtrBarEntry? weekly;
    private IDtrBarEntry? combo;
    
    protected override void OnEnable() { }

    protected override void OnDisable() {
        daily?.Remove();
        daily = null;
		
        weekly?.Remove();
        weekly = null;
		
        combo?.Remove();
        combo = null;
    }

    public override NodeBase GetStatusDisplayNode() {
        return new ResNode();
    }

    protected override void Update() {
        if (ModuleConfig is not { } config) return;
        
        var nextDailyReset = Time.NextDailyReset();
        var nextWeeklyReset = Time.NextWeeklyReset();

        var timeUntilDailyReset = nextDailyReset - DateTime.UtcNow;
        var timeUntilWeeklyReset = nextWeeklyReset - DateTime.UtcNow;
		
        if (daily is null && config.SoloDaily) {
            daily = Services.DtrBar.Get("DailyDuty - Daily Timer");
            daily.OnClick = _ => System.ConfigurationWindow.Toggle();
            daily.Tooltip = "Click to Open Configuration";
        }

        if (daily is not null && !config.SoloDaily) {
            daily.Remove();
            daily = null;
        }

        if (weekly is null && config.SoloWeekly) {
            weekly = Services.DtrBar.Get("DailyDuty - Weekly Timer");
            weekly.OnClick = _ => System.ConfigurationWindow.Toggle();
            weekly.Tooltip = "Click to Open Configuration";
        }
		
        if (weekly is not null && !config.SoloWeekly) {
            weekly.Remove();
            weekly = null;
        }
		
        if (combo is null && config.Combo) {
            combo = Services.DtrBar.Get("DailyDuty - Combo Timer");
            combo.OnClick = OnComboClick;
            combo.Tooltip = "Left Click to Change Mode\n" +
                            "Right Click to Open Configuration";
        }
		
        if (combo is not null && !config.Combo) {
            combo.Remove();
            combo = null;
        }

        daily?.Text = $"Daily Reset: {timeUntilDailyReset.FormatTimespan(config.HideSeconds)}";

        weekly?.Text = $"Weekly Reset: {timeUntilWeeklyReset.FormatTimespan(config.HideSeconds)}";

        if (combo is not null) {
            if (config.CurrentMode is DtrMode.Daily) {
                combo.Text = $"Daily Reset: {timeUntilDailyReset.FormatTimespan(config.HideSeconds)}";
            }
            else {
                combo.Text = $"Weekly Reset: {timeUntilWeeklyReset.FormatTimespan(config.HideSeconds)}";
            }
        }
    }

    private void OnComboClick(DtrInteractionEvent obj) {
        if (ModuleConfig is not { } config) return;
        
        switch (obj.ClickType) {
            case MouseClickType.Left:
                config.CurrentMode = config.CurrentMode is DtrMode.Daily ? DtrMode.Weekly : DtrMode.Daily;
                config.Save();
                break;

            case MouseClickType.Right:
                System.ConfigurationWindow.Toggle();
                break;
        }
    }

    public override CompletionStatus? GetModuleStatus() => null;
    public override ReadOnlySeString? GetStatusMessage() => null;
}
