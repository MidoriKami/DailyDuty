using System;
using System.Numerics;
using DailyDuty.CustomNodes;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using FFXIVClientStructs.FFXIV.Client.UI;
using KamiLib.CommandManager;
using KamiLib.Configuration;
using KamiLib.Extensions;
using KamiToolKit.Classes;

namespace DailyDuty.Classes;

public unsafe class TimersController : IDisposable {
	public TimerNode? WeeklyTimerNode { get; private set; }
	public TimerNode? DailyTimerNode { get; private set; }

	public string WeeklyTimerSavePath => StyleFileHelper.GetPath("WeeklyTimer.style.json"); 
	public string DailyTimerSavePath => StyleFileHelper.GetPath("DailyTimer.style.json"); 

	public TimersController() {
		System.CommandManager.RegisterCommand(new ToggleCommandHandler {
			DisableDelegate = _ => System.TimersConfig.Enabled = false,
			EnableDelegate = _ => System.TimersConfig.Enabled = true,
			ToggleDelegate = _ => System.TimersConfig.Enabled = !System.TimersConfig.Enabled,
			BaseActivationPath = "/timers/",
		});

		System.NameplateAddonController.PreEnable += PreAttach;
		System.NameplateAddonController.OnAttach += AttachNodes;
		System.NameplateAddonController.OnDetach += DetachNodes;
	}

	public void Dispose() {
		System.NameplateAddonController.PreEnable -= PreAttach;
		System.NameplateAddonController.OnAttach -= AttachNodes;
		System.NameplateAddonController.OnDetach -= DetachNodes;
		
		System.NativeController.DetachNode(WeeklyTimerNode, () => {
			WeeklyTimerNode?.Dispose();
			WeeklyTimerNode = null;
		});
		
		System.NativeController.DetachNode(DailyTimerNode, () => {
			DailyTimerNode?.Dispose();
			DailyTimerNode = null;
		});
	}

	private void PreAttach(AddonNamePlate* addonNamePlate)
		=> System.TimersConfig = TimersConfig.Load();

	private void AttachNodes(AddonNamePlate* addonNamePlate) {
		WeeklyTimerNode = new TimerNode(500000) {
			Size = new Vector2(400.0f, 32.0f),
			Scale = new Vector2(0.80f, 0.80f),
			Position = new Vector2(400.0f, 400.0f),
			ModuleName = "Weekly Reset",
			IsVisible = true,
		};
		
		WeeklyTimerNode.Load(Service.PluginInterface.GetCharacterFileInfo(Service.ClientState.LocalContentId, "WeeklyTimer.style.json").FullName);
		System.NativeController.AttachNode(WeeklyTimerNode, addonNamePlate->RootNode, NodePosition.AsFirstChild);

		DailyTimerNode = new TimerNode(600000) {
			Size = new Vector2(400.0f, 32.0f), 
			Scale = new Vector2(0.80f, 0.80f), 
			Position = new Vector2(400.0f, 475.0f),
			ModuleName = "Daily Reset",
			IsVisible = true,
		};
		
		DailyTimerNode.Load(Service.PluginInterface.GetCharacterFileInfo(Service.ClientState.LocalContentId, "DailyTimer.style.json").FullName);
		System.NativeController.AttachNode(DailyTimerNode, addonNamePlate->RootNode, NodePosition.AsFirstChild);
	}

	private void DetachNodes(AddonNamePlate* addonNamePlate) {
		System.NativeController.DetachNode(WeeklyTimerNode, () => {
			WeeklyTimerNode?.Dispose();
			WeeklyTimerNode = null;
		});
		
		System.NativeController.DetachNode(DailyTimerNode, () => {
			DailyTimerNode?.Dispose();
			DailyTimerNode = null;
		});
	}

	public void Update() {
		if (DailyTimerNode is null || WeeklyTimerNode is null) return;
		
		UpdateNode(DailyTimerNode, ModuleType.Daily, System.TimersConfig.EnableDailyTimer);
		UpdateNode(WeeklyTimerNode, ModuleType.Weekly, System.TimersConfig.EnableWeeklyTimer);
	}

	private void UpdateNode(TimerNode? node, ModuleType type, bool showTimer) {
		if (node is null) return;

		node.IsVisible = ShouldShow() && showTimer;

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

		node.TimeRemainingText = timeRemaining.FormatTimespan(System.TimersConfig.HideTimerSeconds);
		node.Progress = percent;
	}
	
	private bool ShouldShow() {
		if (!System.TimersConfig.Enabled) return false;
		if (System.TimersConfig.HideInDuties && Service.Condition.IsBoundByDuty()) return false;
		if (System.TimersConfig.HideInQuestEvents && Service.Condition.IsInQuestEvent()) return false;

		return true;
	}
}
