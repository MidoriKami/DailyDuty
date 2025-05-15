using System;
using DailyDuty.Models;
using DailyDuty.Modules;
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

	public TodoListController() : base(Service.PluginInterface) {
		System.CommandManager.RegisterCommand(new ToggleCommandHandler {
			DisableDelegate = _ => System.TodoConfig.Enabled = false,
			EnableDelegate = _ => System.TodoConfig.Enabled = true,
			ToggleDelegate = _ => System.TodoConfig.Enabled = !System.TodoConfig.Enabled,
			BaseActivationPath = "/todo/",
		});
	}

	protected override void PreAttach() {
		System.TodoConfig = TodoConfig.Load();
	}
	
	protected override void AttachNodes(AddonNamePlate* addonNamePlate) {
		todoListNode = new ListNode<TodoCategoryNode> {
			NodeID = 300_000,
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
		todoListNode?.Dispose();
		todoListNode = null;
	}

	public void Update() {
		if (todoListNode is null) return;
		
		var passedDutyCheck = System.TodoConfig.HideInDuties && !Service.Condition.IsBoundByDuty() || !System.TodoConfig.HideInDuties;
		var passedQuestCheck = System.TodoConfig.HideDuringQuests && !Service.Condition.IsInQuestEvent()  || !System.TodoConfig.HideDuringQuests;

		todoListNode.IsVisible = passedDutyCheck && passedQuestCheck && System.TodoConfig.Enabled;
	}

	public void Refresh() {
		if (todoListNode is null) return;
		
		todoListNode.SetStyle(System.TodoConfig.ListStyle);
		todoListNode.IsVisible = System.TodoConfig.Enabled;

		foreach (var category in todoListNode) {
			category.Refresh();
		}
		
		todoListNode.RecalculateLayout();
	}
}