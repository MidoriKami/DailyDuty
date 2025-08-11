using System;
using System.Numerics;
using DailyDuty.Configs;
using DailyDuty.CustomNodes;
using DailyDuty.Modules.BaseModules;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.CommandManager;
using KamiLib.Extensions;
using KamiToolKit.Nodes;

namespace DailyDuty.Classes;

public class TimersController : IDisposable {
	public TimerNode? WeeklyTimerNode;
	public TimerNode? DailyTimerNode;

	public string WeeklyTimerSavePath => StyleFileHelper.GetPath("WeeklyTimer.style.json"); 
	public string DailyTimerSavePath => StyleFileHelper.GetPath("DailyTimer.style.json"); 

	public TimersController() {
		System.CommandManager.RegisterCommand(new ToggleCommandHandler {
			DisableDelegate = _ => System.TimersConfig.Enabled = false,
			EnableDelegate = _ => System.TimersConfig.Enabled = true,
			ToggleDelegate = _ => System.TimersConfig.Enabled = !System.TimersConfig.Enabled,
			BaseActivationPath = "/timers/",
		});
	}

	public void Dispose() {
		System.NativeController.DisposeNode(ref WeeklyTimerNode);
		System.NativeController.DisposeNode(ref DailyTimerNode);
	}

	public void Load()
		=> System.TimersConfig = TimersConfig.Load();

	public void AttachNodes(SimpleOverlayNode overlayNode) {
		WeeklyTimerNode = new TimerNode {
			NodeId = 3,
			Size = new Vector2(400.0f, 64.0f),
			Scale = new Vector2(0.80f, 0.80f),
			Position = new Vector2(400.0f, 400.0f),
			ModuleName = "Weekly Reset",
			IsVisible = true,
			OnEditComplete = () => WeeklyTimerNode?.Save(WeeklyTimerSavePath),
		};
		WeeklyTimerNode.Load(WeeklyTimerSavePath);
		WeeklyTimerNode.ModuleNameAlignment = AlignmentType.Left; // Fix for old configs using a weird alignment
		System.NativeController.AttachNode(WeeklyTimerNode, overlayNode);

		DailyTimerNode = new TimerNode {
			NodeId = 4,
			Size = new Vector2(400.0f, 64.0f), 
			Scale = new Vector2(0.80f, 0.80f), 
			Position = new Vector2(400.0f, 475.0f),
			ModuleName = "Daily Reset",
			IsVisible = true,
			OnEditComplete = () => DailyTimerNode?.Save(DailyTimerSavePath),
		};
		DailyTimerNode.Load(DailyTimerSavePath);
		DailyTimerNode.ModuleNameAlignment = AlignmentType.Left; // Fix for old configs using a weird alignment
		System.NativeController.AttachNode(DailyTimerNode, overlayNode);
	}

	public void DetachNodes() {
		System.NativeController.DisposeNode(ref WeeklyTimerNode);
		System.NativeController.DisposeNode(ref DailyTimerNode);
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
