using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyDuty.CustomNodes;
using DailyDuty.Models;
using DailyDuty.Modules.BaseModules;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using KamiLib.CommandManager;
using KamiLib.Extensions;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Classes;

public class TodoListController : IDisposable {
	public ListBoxNode? TodoListNode { get; private set; }

	public TodoCategoryNode? DailyTaskNode { get; private set; }
	public TodoCategoryNode? WeeklyTaskNode { get; private set; }
	public TodoCategoryNode? SpecialTaskNode { get; private set; }
	
	private List<TodoCategoryNode?> TodoListNodes => [ DailyTaskNode, WeeklyTaskNode, SpecialTaskNode ];

	private static string TodoListNodePath => StyleFileHelper.GetPath("TodoList.style.json");
	private static string DailyCategoryPath => StyleFileHelper.GetPath("DailyCategory.style.json");
	private static string WeeklyCategoryPath => StyleFileHelper.GetPath("WeeklyCategory.style.json");
	private static string SpecialCategoryPath => StyleFileHelper.GetPath("SpecialCategory.style.json");

	public TodoListController() {
		System.CommandManager.RegisterCommand(new ToggleCommandHandler {
			DisableDelegate = _ => System.TodoConfig.Enabled = false,
			EnableDelegate = _ => System.TodoConfig.Enabled = true,
			ToggleDelegate = _ => System.TodoConfig.Enabled = !System.TodoConfig.Enabled,
			BaseActivationPath = "/todo/",
		});
	}

	public void Dispose() {
		System.NativeController.DetachNode(TodoListNode, () => {
			TodoListNode?.Dispose();
			TodoListNode = null;
		});
	}

	public void Load()
		=> System.TodoConfig = TodoConfig.Load();

	public void AttachNodes(SimpleOverlayNode overlayNode) {
		TodoListNode = new ListBoxNode {
			NodeId = 2,
			LayoutAnchor = LayoutAnchor.TopLeft,
			Position = new Vector2(750.0f, 375.0f),
			Size = new Vector2(600.0f, 200.0f),
			LayoutOrientation = LayoutOrientation.Horizontal,
			ShowBackground = true,
			BackgroundColor = KnownColor.Aqua.Vector() with { W = 0.40f },
			ClipListContents = true,
			ShowBorder = true,

			OnEditComplete = Save,
		};
		TodoListNode.Load(TodoListNodePath);
		System.NativeController.AttachNode(TodoListNode, overlayNode);

		DailyTaskNode = new TodoCategoryNode(ModuleType.Daily);
		DailyTaskNode.Load(DailyCategoryPath);
		DailyTaskNode.LoadNodes();
		TodoListNode.AddNode(DailyTaskNode);
		
		WeeklyTaskNode = new TodoCategoryNode(ModuleType.Weekly);
		WeeklyTaskNode.Load(WeeklyCategoryPath);
		WeeklyTaskNode.LoadNodes();
		TodoListNode.AddNode(WeeklyTaskNode);
		
		SpecialTaskNode = new TodoCategoryNode(ModuleType.Special);
		SpecialTaskNode.Load(SpecialCategoryPath);
		SpecialTaskNode.LoadNodes();
		TodoListNode.AddNode(SpecialTaskNode);

		System.TodoListController.Refresh();
	}

	public void DetachNodes() {
		System.NativeController.DetachNode(TodoListNode, () => {
			TodoListNode?.Dispose();
			TodoListNode = null;
		});
	}

	public void Update() {
		if (TodoListNode is null) return;
		
		var passedDutyCheck = System.TodoConfig.HideInDuties && !Service.Condition.IsBoundByDuty() || !System.TodoConfig.HideInDuties;
		var passedQuestCheck = System.TodoConfig.HideDuringQuests && !Service.Condition.IsInQuestEvent()  || !System.TodoConfig.HideDuringQuests;

		TodoListNode.IsVisible = passedDutyCheck && passedQuestCheck && System.TodoConfig.Enabled && TodoListNodes.Any(node => node?.AnyTasksActive ?? false);
	}

	public void Refresh() {
		if (TodoListNode is null) return;

		foreach (var category in TodoListNodes) {
			category?.Refresh();
		}
		
		TodoListNode.RecalculateLayout();
	}

	public void DrawConfig() {
		TodoListNode?.DrawConfig();

		using (var dailyCategory = ImRaii.TreeNode("Daily Tasks")) {
			if (dailyCategory) {
				DailyTaskNode?.DrawConfig();
			}
		}

		using (var weeklyCategory = ImRaii.TreeNode("Weekly Tasks")) {
			if (weeklyCategory) {
				WeeklyTaskNode?.DrawConfig();
			}
		}

		using (var specialCategory = ImRaii.TreeNode("Special Tasks")) {
			if (specialCategory) {
				SpecialTaskNode?.DrawConfig();
			}
		}
	}

	public void Save() {
		TodoListNode?.Save(TodoListNodePath);
		DailyTaskNode?.Save(DailyCategoryPath);
		WeeklyTaskNode?.Save(WeeklyCategoryPath);
		SpecialTaskNode?.Save(SpecialCategoryPath);
	}
}