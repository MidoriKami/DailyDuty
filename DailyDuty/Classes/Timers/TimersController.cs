using System;
using DailyDuty.Models;
using DailyDuty.Modules;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.CommandManager;
using KamiLib.Extensions;
using KamiToolKit;
using KamiToolKit.Classes;

namespace DailyDuty.Classes.Timers;

public unsafe class TimersController : NativeUiOverlayController {
	private TimerNode? weeklyTimerNode;
	private TimerNode? dailyTimerNode;

	public TimersController() : base(Service.PluginInterface) {
		System.CommandManager.RegisterCommand(new ToggleCommandHandler {
			DisableDelegate = _ => System.TimersConfig.Enabled = false,
			EnableDelegate = _ => System.TimersConfig.Enabled = true,
			ToggleDelegate = _ => System.TimersConfig.Enabled = !System.TodoConfig.Enabled,
			BaseActivationPath = "/timers/",
		});
	}

	protected override void PreAttach() {
		System.TimersConfig = TimersConfig.Load();

		// Potentially fix corrupted config files
		if (System.TimersConfig.Version is not 2) {
			System.TimersConfig = new TimersConfig();
			System.TimersConfig.Save();
		}
	}

	protected override void AttachNodes(AddonNamePlate* addonNamePlate) {
		weeklyTimerNode = new TimerNode(500000);
		System.NativeController.AttachToAddon(weeklyTimerNode, (AtkUnitBase*)addonNamePlate, addonNamePlate->RootNode, NodePosition.AsFirstChild);

		dailyTimerNode = new TimerNode(600000);
		System.NativeController.AttachToAddon(dailyTimerNode, (AtkUnitBase*)addonNamePlate, addonNamePlate->RootNode, NodePosition.AsFirstChild);
		
		System.TimersController.Refresh();
	}

	protected override void DetachNodes(AddonNamePlate* addonNamePlate) {
		if (weeklyTimerNode is not null) {
			System.NativeController.DetachFromAddon(weeklyTimerNode, (AtkUnitBase*)addonNamePlate, () => {
				weeklyTimerNode.Dispose();
				weeklyTimerNode = null;
			});
		}

		if (dailyTimerNode is not null) {
			System.NativeController.DetachFromAddon(dailyTimerNode, (AtkUnitBase*)addonNamePlate, () => {
				dailyTimerNode.Dispose();
				dailyTimerNode = null;
			});
		}
	}

	public void Update() {
		if (dailyTimerNode is not null) {
			UpdateNode(dailyTimerNode, System.TimersConfig.DailyTimerConfig, ModuleType.Daily);
		}

		if (weeklyTimerNode is not null) {
			UpdateNode(weeklyTimerNode, System.TimersConfig.WeeklyTimerConfig, ModuleType.Weekly);
		}
	}

	public void Refresh() {
		if (weeklyTimerNode is not null) {
			var timerConfig = System.TimersConfig.WeeklyTimerConfig;
			weeklyTimerNode.SetStyle(timerConfig.Style);
			weeklyTimerNode.ModuleName = timerConfig.UseCustomLabel ? timerConfig.CustomLabel : "Weekly Reset";
		}

		if (dailyTimerNode is not null) {
			var timerConfig = System.TimersConfig.DailyTimerConfig;
			dailyTimerNode.SetStyle(timerConfig.Style);
			dailyTimerNode.ModuleName = timerConfig.UseCustomLabel ? timerConfig.CustomLabel : "Daily Reset";
		}
	}

	private void UpdateNode(TimerNode? node, TimerConfig config, ModuleType type) {
		if (node is null) return;

		node.IsVisible = config.TimerEnabled && ShouldShow();

		var nextReset = type switch {
			ModuleType.Weekly => Time.NextWeeklyReset(),
			ModuleType.Daily => Time.NextDailyReset(),
			_ => throw new Exception("Invalid Type"),
		};

		var resetPeriod = type switch {
			ModuleType.Weekly => TimeSpan.FromDays(7),
			ModuleType.Daily => TimeSpan.FromDays(1),
			_ => throw new Exception("Invalid Type"),
		};
		
		var timeRemaining = nextReset - DateTime.UtcNow;

		var percent =  1.0f - (float)(timeRemaining / resetPeriod);
		if (percent > 1.0f) return;
			
		node.TimeRemainingText = timeRemaining.FormatTimespan(config.HideSeconds);
		node.Progress = percent;
	}
	
	private bool ShouldShow() {
		if (!System.TimersConfig.Enabled) return false;
		if (System.TimersConfig.HideInDuties && Service.Condition.IsBoundByDuty()) return false;
		if (System.TimersConfig.HideInQuestEvents && Service.Condition.IsInQuestEvent()) return false;

		return true;
	}
}
