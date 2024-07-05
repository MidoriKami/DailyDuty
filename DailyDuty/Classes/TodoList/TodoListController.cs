using System;
using System.Drawing;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Extensions;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Classes.TodoList;

public unsafe class TodoListController : IDisposable {
	private ListNode<TodoCategoryNode>? todoListNode;

	private AddonNamePlate* AddonNamePlate => (AddonNamePlate*) Service.GameGui.GetAddonByName("NamePlate");

	public void Dispose() {
		Unload();
	}
	
	public void Load() {
		System.TodoConfig = TodoConfig.Load();

		Service.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "NamePlate", OnNamePlateSetup);
		Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, "NamePlate", OnNamePlateFinalize);

		if (AddonNamePlate is not null) {
			AttachToNative(AddonNamePlate);
		}
	}

	public void Unload() {
		Service.AddonLifecycle.UnregisterListener(OnNamePlateSetup);
		Service.AddonLifecycle.UnregisterListener(OnNamePlateFinalize);

		if (AddonNamePlate is not null) {
			DetachFromNative(AddonNamePlate);
		}
	}

	public void Update() {
		if (todoListNode is null) return;
		
		var passedDutyCheck = System.TodoConfig.HideInDuties && !Service.Condition.IsBoundByDuty() || !System.TodoConfig.HideInDuties;
		var passedQuestCheck = System.TodoConfig.HideDuringQuests && !Service.Condition.IsInQuestEvent()  || !System.TodoConfig.HideDuringQuests;

		todoListNode.IsVisible = passedDutyCheck && passedQuestCheck && System.TodoConfig.Enabled;
	}

	private void OnNamePlateSetup(AddonEvent type, AddonArgs args) {
		AttachToNative((AddonNamePlate*)args.Addon);
	}
	
	private void AttachToNative(AddonNamePlate* addonNamePlate) {
		Service.Framework.RunOnFrameworkThread(() => {
			todoListNode = new ListNode<TodoCategoryNode> {
				Size = System.TodoConfig.Size,
				Position = System.TodoConfig.Position,
				LayoutAnchor = System.TodoConfig.Anchor,
				NodeFlags = NodeFlags.Clip,
				IsVisible = System.TodoConfig.Enabled,
				LayoutOrientation = System.TodoConfig.SingleLine ? LayoutOrientation.Horizontal : LayoutOrientation.Vertical,
				NodeID = 300_000,
				Color = KnownColor.White.Vector(),
				BackgroundVisible = System.TodoConfig.ShowListBackground,
				BackgroundColor = System.TodoConfig.ListBackgroundColor,
			};

			foreach (var moduleType in Enum.GetValues<ModuleType>()) {
				var categoryNode = new TodoCategoryNode(moduleType);
				categoryNode.LoadNodes(addonNamePlate);
				
				todoListNode.Add(categoryNode);
			}
			
			System.NativeController.AttachToAddon(todoListNode, (AtkUnitBase*)addonNamePlate, addonNamePlate->RootNode, NodePosition.AsFirstChild);
			System.TodoListController.Refresh();
		});
	}

	private void OnNamePlateFinalize(AddonEvent type, AddonArgs args) {
		DetachFromNative((AddonNamePlate*)args.Addon);
	}
	
	private void DetachFromNative(AddonNamePlate* addonNamePlate) {
		Service.Framework.RunOnFrameworkThread(() => {
			if (todoListNode is not null) {
				System.NativeController.DetachFromAddon(todoListNode, (AtkUnitBase*)addonNamePlate);
				todoListNode.Dispose();
				todoListNode = null;
			}
			
			todoListNode?.Dispose();
		});
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
		todoListNode.LayoutOrientation = System.TodoConfig.SingleLine ? LayoutOrientation.Horizontal : LayoutOrientation.Vertical;
		
		foreach (var category in todoListNode) {
			category.Refresh();
		}
		
		todoListNode.RecalculateLayout();
	}
}