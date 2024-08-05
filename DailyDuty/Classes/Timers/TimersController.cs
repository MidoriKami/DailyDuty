using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Models;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.CommandManager;
using KamiLib.Extensions;
using KamiToolKit;
using KamiToolKit.Classes;

namespace DailyDuty.Classes.Timers;

public unsafe class TimersController : NativeUiOverlayController {
	private List<TimerNode>? timerNodes;
	private TimerNode? weeklyTimerNode;
	private TimerNode? dailyTimerNode;

	public TimersController() : base(Service.AddonLifecycle, Service.Framework, Service.GameGui) {
		System.CommandManager.RegisterCommand(new ToggleCommandHandler {
			DisableDelegate = _ => System.TimersConfig.Enabled = false,
			EnableDelegate = _ => System.TimersConfig.Enabled = true,
			ToggleDelegate = _ => System.TimersConfig.Enabled = !System.TodoConfig.Enabled,
			BaseActivationPath = "/timers/",
		});
	}

	protected override void LoadConfig()
		=> System.TimersConfig = TimersConfig.Load();

	protected override void AttachNodes(AddonNamePlate* addonNamePlate) {
		timerNodes = [];
		
		foreach (var module in System.ModuleController.Modules) {
			var newTimerNode = new TimerNode(400000 + (uint) module.ModuleName) {
				Module = module,
			};
			
			if (module.HasTooltip) {
				newTimerNode.Tooltip = module.TooltipText;
			}
			
			if (module.HasClickableLink) {
				newTimerNode.MouseClick = () => PayloadController.GetDelegateForPayload(module.ClickableLinkPayloadId).Invoke(0, null!);
			}

			if (module is { HasTooltip: true } or { HasClickableLink: true }) {
				newTimerNode.EnableEvents(Service.AddonEventManager, (AtkUnitBase*)addonNamePlate);
			}
			
			timerNodes.Add(newTimerNode);
			UpdateTimeNode(newTimerNode);
			
			System.NativeController.AttachToAddon(newTimerNode, (AtkUnitBase*)addonNamePlate, addonNamePlate->RootNode, NodePosition.AsFirstChild);
			System.TimersController.Refresh();
		}

		weeklyTimerNode = new TimerNode(500000);
		System.NativeController.AttachToAddon(weeklyTimerNode, (AtkUnitBase*)addonNamePlate, addonNamePlate->RootNode, NodePosition.AsFirstChild);
		System.TimersController.Refresh();

		dailyTimerNode = new TimerNode(600000);
		System.NativeController.AttachToAddon(dailyTimerNode, (AtkUnitBase*)addonNamePlate, addonNamePlate->RootNode, NodePosition.AsFirstChild);
		System.TimersController.Refresh();
	}

	protected override void DetachNodes(AddonNamePlate* addonNamePlate) {
		if (timerNodes is not null) {
			foreach (var node in timerNodes) {
				System.NativeController.DetachFromAddon(node, (AtkUnitBase*)addonNamePlate);
				node.Dispose();
			}
			
			timerNodes.Clear();
		}

		if (weeklyTimerNode is not null) {
			System.NativeController.DetachFromAddon(weeklyTimerNode, (AtkUnitBase*)addonNamePlate);
			weeklyTimerNode.Dispose();
		}

		if (dailyTimerNode is not null) {
			System.NativeController.DetachFromAddon(dailyTimerNode, (AtkUnitBase*)addonNamePlate);
			dailyTimerNode.Dispose();
		}
	}

	public void Update() {
		if (timerNodes is not null) {
			if (Service.ClientState.IsPvP) {
				foreach (var node in timerNodes) {
					node.IsVisible = false;
				}
				return;
			}

			foreach (var node in timerNodes) {
				node.IsVisible = ShouldShow(node);
				if (!node.IsVisible) continue;
			
				UpdateTimeNode(node);
			}
		}
		
		UpdateDailyTimerNode();
		UpdateWeeklyTimerNode();
	}

	public void Refresh() {
		if (timerNodes is not null) {
			foreach (var node in timerNodes) {
				var module = node.Module;
				if (module is null) continue;
			
				var timerConfig = module.GetTimerConfig();

				node.ModuleName = timerConfig.UseCustomLabel ? timerConfig.CustomLabel : module.ModuleName.GetDescription();
				node.Size = timerConfig.Size;
				node.Position = timerConfig.Position;
				node.BarColor = timerConfig.BarColor;
				node.BarBackgroundColor = timerConfig.BarBackgroundColor;
				node.LabelVisible = !timerConfig.HideName;
				node.TimeVisible = !timerConfig.HideTime;
				node.Tooltip = module.TooltipText;
				node.Scale = new Vector2(timerConfig.Scale);

				UpdateTimeNode(node);
			
				if (module.HasClickableLink && node.MouseClick is null) {
					node.MouseClick = () => PayloadController.GetDelegateForPayload(module.ClickableLinkPayloadId).Invoke(0, null!);
				}
				else if (!module.HasClickableLink && node.MouseClick is not null) {
					node.MouseClick = null;
				}
			}
		}
		
		if (weeklyTimerNode is not null) {
			var timerConfig = System.TimersConfig.WeeklyTimerConfig;
			weeklyTimerNode.ModuleName = timerConfig.UseCustomLabel ? timerConfig.CustomLabel : "Weekly Reset";

		}

		if (dailyTimerNode is not null) {
			var timerConfig = System.TimersConfig.DailyTimerConfig;
			dailyTimerNode.ModuleName = timerConfig.UseCustomLabel ? timerConfig.CustomLabel : "Daily Reset";
		}
	}

	private void UpdateTimeNode(TimerNode node) {
		var module = node.Module;
		if (module is null) return;

		var timerConfig = module.GetTimerConfig();
		var timeRemaining = module.GetTimeRemaining();
		var period = module.GetModulePeriod();

		var percent =  1.0f - (float)(timeRemaining / period);
		if (percent > 1.0f) return;
			
		node.TimeRemainingText = timeRemaining.FormatTimespan(timerConfig.HideSeconds);
		node.Progress = percent;
			
		if (module.GetData().NextReset == DateTime.MaxValue) {
			node.Progress = 1.0f;
			node.TimeRemainingText = string.Empty;
		}
	}

	private bool ShouldShow(TimerNode timerNode) {
		if (!System.TimersConfig.Enabled) return false;
		if (System.TimersConfig.HideInDuties && Service.Condition.IsBoundByDuty()) return false;
		if (System.TimersConfig.HideInQuestEvents && Service.Condition.IsInQuestEvent()) return false;
		
		var module = timerNode.Module;
		if (module is null) return false;
		
		var config = module.GetTimerConfig();
		
		if (!module.IsEnabled) return false;
		if (!config.TimerEnabled) return false;
		if (config.HideWhenComplete && module.ModuleStatus is not ModuleStatus.Incomplete) return false;

		return true;
	}

	private void UpdateWeeklyTimerNode() {
		if (weeklyTimerNode is null) return;

		var timerConfig = System.TimersConfig.WeeklyTimerConfig;

		weeklyTimerNode.IsVisible = !Service.ClientState.IsPvP && timerConfig.TimerEnabled;
		weeklyTimerNode.Size = timerConfig.Size;
		weeklyTimerNode.Position = timerConfig.Position;
		weeklyTimerNode.BarColor = timerConfig.BarColor;
		weeklyTimerNode.BarBackgroundColor = timerConfig.BarBackgroundColor;
		weeklyTimerNode.LabelVisible = !timerConfig.HideName;
		weeklyTimerNode.TimeVisible = !timerConfig.HideTime;
		weeklyTimerNode.Scale = new Vector2(timerConfig.Scale);
		
		var timeRemaining = Time.NextWeeklyReset() - DateTime.UtcNow;
		var period = TimeSpan.FromDays(7);

		var percent =  1.0f - (float)(timeRemaining / period);
		if (percent > 1.0f) return;
			
		weeklyTimerNode.TimeRemainingText = timeRemaining.FormatTimespan(timerConfig.HideSeconds);
		weeklyTimerNode.Progress = percent;
	}

	private void UpdateDailyTimerNode() {
		if (dailyTimerNode is null) return;
		
		var timerConfig = System.TimersConfig.DailyTimerConfig;

		dailyTimerNode.IsVisible = !Service.ClientState.IsPvP && timerConfig.TimerEnabled;
		dailyTimerNode.Size = timerConfig.Size;
		dailyTimerNode.Position = timerConfig.Position;
		dailyTimerNode.BarColor = timerConfig.BarColor;
		dailyTimerNode.BarBackgroundColor = timerConfig.BarBackgroundColor;
		dailyTimerNode.LabelVisible = !timerConfig.HideName;
		dailyTimerNode.TimeVisible = !timerConfig.HideTime;
		dailyTimerNode.Scale = new Vector2(timerConfig.Scale);
		
		var timeRemaining = Time.NextDailyReset() - DateTime.UtcNow;
		var period = TimeSpan.FromDays(1);

		var percent =  1.0f - (float)(timeRemaining / period);
		if (percent > 1.0f) return;
			
		dailyTimerNode.TimeRemainingText = timeRemaining.FormatTimespan(timerConfig.HideSeconds);
		dailyTimerNode.Progress = percent;
	}
}
