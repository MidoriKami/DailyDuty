using System;
using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Utilities;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Plugin.Services;
using KamiToolKit;

namespace DailyDuty.Features.ServerInfoBar;

public class ServerInfoBar : FeatureBase {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Server Info Bar",
        FileName = "DTR",
        Type = ModuleType.GeneralFeatures,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation")
        ],
        Tags = [ "DTR" ],
    };

    public ServerInfoBarConfig ModuleConfig = null!;
    public override NodeBase DisplayNode => new ConfigNode(this);
    
    private IDtrBarEntry? daily;
    private IDtrBarEntry? weekly;
    private IDtrBarEntry? combo;

    public override void Load() {
        ModuleConfig = Config.LoadCharacterConfig<ServerInfoBarConfig>($"{ModuleInfo.FileName}.config.json");
        if (ModuleConfig is null) throw new Exception("Failed to load config file");
        
        ModuleConfig.FileName = ModuleInfo.FileName;

        Services.Framework.Update += Update;
    }

    public override void Unload() {
        Services.Framework.Update -= Update;
        
        ModuleConfig = null!;
    }

    public override void Enable() {
        IsEnabled = true;
    }

    public override void Disable() {
        IsEnabled = false;
        
        daily?.Remove();
        daily = null;
		
        weekly?.Remove();
        weekly = null;
		
        combo?.Remove();
        combo = null;
    }

    private void Update(IFramework framework) {
        if (!IsEnabled) return;
        if (ModuleConfig is not { } config) return;

        if (ModuleConfig.SavePending) {
            Services.PluginLog.Debug($"Saving {ModuleInfo.DisplayName} config");
            ModuleConfig.Save();
        }
        
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
}
