using System;
using DailyDuty.Configs;
using Dalamud.Game.Gui.Dtr;

namespace DailyDuty.Classes;

public class DtrController : IDisposable {
	public DtrConfig? Config;

	private IDtrBarEntry? daily;
	private IDtrBarEntry? weekly;
	private IDtrBarEntry? combo;
	
	public void Load()
		=> Config = DtrConfig.Load();

	public void Update() {
		if (Config is null) return;

		var nextDailyReset = Time.NextDailyReset();
		var nextWeeklyReset = Time.NextWeeklyReset();

		var timeUntilDailyReset = nextDailyReset - DateTime.UtcNow;
		var timeUntilWeeklyReset = nextWeeklyReset - DateTime.UtcNow;
		
		if (daily is null && Config.SoloDaily) {
			daily = Service.DtrBar.Get("DailyDuty - Daily Timer");
			daily.OnClick = _ => System.ConfigurationWindow.Toggle();
			daily.Tooltip = "Click to Open Configuration";
		}

		if (daily is not null && !Config.SoloDaily) {
			daily.Remove();
			daily = null;
		}

		if (weekly is null && Config.SoloWeekly) {
			weekly = Service.DtrBar.Get("DailyDuty - Weekly Timer");
			weekly.OnClick = _ => System.ConfigurationWindow.Toggle();
			weekly.Tooltip = "Click to Open Configuration";
		}
		
		if (weekly is not null && !Config.SoloWeekly) {
			weekly.Remove();
			weekly = null;
		}
		
		if (combo is null && Config.Combo) {
			combo = Service.DtrBar.Get("DailyDuty - Combo Timer");
			combo.OnClick = OnComboClick;
			combo.Tooltip = "Left Click to Change Mode\n" +
			                "Right Click to Open Configuration";
		}
		
		if (combo is not null && !Config.Combo) {
			combo.Remove();
			combo = null;
		}
		
		if (daily is not null) {
			daily.Text = $"Daily Reset: {timeUntilDailyReset.FormatTimespan(Config.HideSeconds)}";
		}

		if (weekly is not null) {
			weekly.Text = $"Weekly Reset: {timeUntilWeeklyReset.FormatTimespan(Config.HideSeconds)}";
		}

		if (combo is not null) {
			if (Config.CurrentMode is DtrMode.Daily) {
				combo.Text = $"Daily Reset: {timeUntilDailyReset.FormatTimespan(Config.HideSeconds)}";
			}
			else {
				combo.Text = $"Weekly Reset: {timeUntilWeeklyReset.FormatTimespan(Config.HideSeconds)}";
			}
		}
	}

	private void OnComboClick(DtrInteractionEvent obj) {
		if (Config is null) return;

		switch (obj.ClickType) {
			case MouseClickType.Left:
				Config.CurrentMode = Config.CurrentMode is DtrMode.Daily ? DtrMode.Weekly : DtrMode.Daily;
				Config.Save();
				break;

			case MouseClickType.Right:
				System.ConfigurationWindow.Toggle();
				break;
		}
	}

	public void Unload() {
		daily?.Remove();
		weekly?.Remove();
		combo?.Remove();

		Config?.Save();
		Config = null;
	}

	public void Dispose() => Unload();
}