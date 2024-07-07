
using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Localization;
using DailyDuty.Models;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Extensions;
using KamiToolKit;
using KamiToolKit.Classes;

namespace DailyDuty.Classes.Timers;

public unsafe class TimersController() : NativeUiOverlayController(Service.AddonLifecycle, Service.Framework, Service.GameGui) {
	private List<TimerNode>? timerNodes;
	
	protected override void LoadConfig()
		=> System.TimersConfig = TimersConfig.Load();

	protected override void AttachNodes(AddonNamePlate* addonNamePlate) {
		timerNodes = [];
		
		foreach (var module in System.ModuleController.Modules) {
			Service.Log.Debug($"{module.ModuleName}-{(uint)module.ModuleName}");
			
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
	}

	protected override void DetachNodes(AddonNamePlate* addonNamePlate) {
		if (timerNodes is null) return;

		foreach (var node in timerNodes) {
			System.NativeController.DetachFromAddon(node, (AtkUnitBase*)addonNamePlate);
			node.Dispose();
		}
		
		timerNodes.Clear();
	}

	public void Update() {
		if (timerNodes is null) return;
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

	public void Refresh() {
		if (timerNodes is null) return;
		
		foreach (var node in timerNodes) {
			var module = node.Module;
			var timerConfig = module.GetTimerConfig();

			node.ModuleName = module.ModuleName.GetDescription(Strings.ResourceManager);
			node.Size = timerConfig.Size;
			node.Position = timerConfig.Position;
			node.Scale = new Vector2(0.80f);
			node.BarColor = timerConfig.BarColor;
			node.BarBackgroundColor = timerConfig.BarBackgroundColor;
			node.LabelVisible = !timerConfig.HideName;
			node.TimeVisible = !timerConfig.HideTime;

			UpdateTimeNode(node);
			
			if (module.HasClickableLink && node.MouseClick is null) {
				node.MouseClick = () => PayloadController.GetDelegateForPayload(module.ClickableLinkPayloadId).Invoke(0, null!);
			}
			else if (!module.HasClickableLink && node.MouseClick is not null) {
				node.MouseClick = null;
			}
		}
	}

	private void UpdateTimeNode(TimerNode node) {
		var module = node.Module;
		var timerConfig = module.GetTimerConfig();
		var timeRemaining = node.Module.GetTimeRemaining();
		var period = node.Module.GetModulePeriod();

		var percent =  1.0f - (float)(timeRemaining / period);
		if (percent > 1.0f) return;
			
		node.TimeRemainingText = timeRemaining.FormatTimespan(timerConfig.HideSeconds);
		node.Progress = percent;
			
		if (module.GetNextReset() == DateTime.MaxValue) {
			node.Progress = 1.0f;
			node.TimeRemainingText = string.Empty;
		}
	}

	private bool ShouldShow(TimerNode timerNode) {
		var module = timerNode.Module;
		var config = module.GetTimerConfig();
		
		if (!module.IsEnabled) return false;
		if (!config.TimerEnabled) return false;
		if (config.HideWhenComplete && module.ModuleStatus is ModuleStatus.Complete) return false;
		if (config.HideInDuties && Service.Condition.IsBoundByDuty()) return false;
		if (config.HideInQuestEvents && Service.Condition.IsInQuestEvent()) return false;

		return true;
	}
}
