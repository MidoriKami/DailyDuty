using System;
using System.Drawing;
using System.Numerics;
using DailyDuty.Models;
using DailyDuty.Modules;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.CommandManager;
using KamiLib.Extensions;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Classes.TodoList;

public unsafe class TodoListController : NativeUiOverlayController {
	private ListNode<TodoCategoryNode>? todoListNode;

	public TodoListController() : base(Service.AddonLifecycle, Service.Framework, Service.GameGui) {
		System.CommandManager.RegisterCommand(new ToggleCommandHandler {
			DisableDelegate = _ => System.TodoConfig.Enabled = false,
			EnableDelegate = _ => System.TodoConfig.Enabled = true,
			ToggleDelegate = _ => System.TodoConfig.Enabled = !System.TodoConfig.Enabled,
			BaseActivationPath = "/todo/",
		});
	}

	protected override void LoadConfig() {
		System.TodoConfig = TodoConfig.Load();
	}
	
	protected override void AttachNodes(AddonNamePlate* addonNamePlate) {
		todoListNode = new ListNode<TodoCategoryNode> {
			Size = System.TodoConfig.Size,
			Position = System.TodoConfig.Position,
			LayoutAnchor = System.TodoConfig.Anchor,
			ClipListContents = true,
			IsVisible = System.TodoConfig.Enabled,
			LayoutOrientation = System.TodoConfig.SingleLine ? LayoutOrientation.Horizontal : LayoutOrientation.Vertical,
			NodeID = 300_000,
			Color = KnownColor.White.Vector(),
			BackgroundVisible = System.TodoConfig.ShowListBackground,
			BackgroundColor = System.TodoConfig.ListBackgroundColor,
			BorderVisible = System.TodoConfig.ShowListBorder,
			BackgroundFitsContents = System.TodoConfig.FitBackground,
			Scale = new Vector2(System.TodoConfig.Scale),
		};

		foreach (var moduleType in Enum.GetValues<ModuleType>()) {
			var categoryNode = new TodoCategoryNode(moduleType);
			categoryNode.LoadNodes(addonNamePlate);
			categoryNode.EnableEvents(Service.AddonEventManager, (AtkUnitBase*)addonNamePlate);
			
			todoListNode.Add(categoryNode);
		}
			
		System.NativeController.AttachToAddon(todoListNode, (AtkUnitBase*)addonNamePlate, addonNamePlate->RootNode, NodePosition.AsFirstChild);
		System.TodoListController.Refresh();
	}
	
	protected override void DetachNodes(AddonNamePlate* addonNamePlate) {
		if (todoListNode is not null) {
			System.NativeController.DetachFromAddon(todoListNode, (AtkUnitBase*)addonNamePlate);
			todoListNode.Dispose();
			todoListNode = null;
		}
			
		todoListNode?.Dispose();
	}

	public void Update() {
		if (todoListNode is null) return;
		
		var passedDutyCheck = System.TodoConfig.HideInDuties && !Service.Condition.IsBoundByDuty() || !System.TodoConfig.HideInDuties;
		var passedQuestCheck = System.TodoConfig.HideDuringQuests && !Service.Condition.IsInQuestEvent()  || !System.TodoConfig.HideDuringQuests;

		todoListNode.IsVisible = passedDutyCheck && passedQuestCheck && System.TodoConfig.Enabled;
	}

	public void Refresh() {
		if (todoListNode is null) return;
		if (Service.ClientState.IsPvP) {
			todoListNode.IsVisible = false;
			return;
		}
		
		todoListNode.IsVisible = System.TodoConfig.Enabled;
		todoListNode.LayoutAnchor = System.TodoConfig.Anchor;
		todoListNode.Position = System.TodoConfig.Position;
		todoListNode.Size = System.TodoConfig.Size;
		todoListNode.BackgroundVisible = System.TodoConfig.ShowListBackground;
		todoListNode.BackgroundColor = System.TodoConfig.ListBackgroundColor;
		todoListNode.BorderVisible = System.TodoConfig.ShowListBorder;
		todoListNode.BackgroundFitsContents = System.TodoConfig.FitBackground;
		todoListNode.LayoutOrientation = System.TodoConfig.SingleLine ? LayoutOrientation.Horizontal : LayoutOrientation.Vertical;
		todoListNode.Scale = new Vector2(System.TodoConfig.Scale);

		foreach (var category in todoListNode) {
			category.Refresh();
		}
		
		todoListNode.RecalculateLayout();
	}
}