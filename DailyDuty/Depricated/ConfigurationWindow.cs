// using System.Collections.Generic;
// using System.Drawing;
// using System.Linq;
// using System.Numerics;
// using DailyDuty.Classes;
// using DailyDuty.CustomNodes;
// using DailyDuty.Modules.BaseModules;
// using Dalamud.Bindings.ImGui;
// using Dalamud.Interface;
// using Dalamud.Interface.Utility;
// using Dalamud.Interface.Utility.Raii;
// using FFXIVClientStructs.FFXIV.Component.GUI;
// using KamiLib.Classes;
// using KamiLib.CommandManager;
// using KamiLib.Configuration;
// using KamiLib.Extensions;
// using KamiLib.Window;
// using KamiToolKit.Classes;
//
// namespace DailyDuty.Windows;
//
// public class ConfigurationWindow : TabbedSelectionWindow<Module> {
//
//     protected override string SelectionListTabName => "Modules";
//     
//     protected override List<ITabItem> Tabs { get; } = [
//         new TodoConfigTab(),
//         new TimersConfigTab(),
//         new ServerInfoBarTab(),
//     ];
//     
//     protected override List<Module> Options => System.ModuleManager.Modules;
//     
//     protected override float SelectionListWidth => 200.0f;
//     
//     protected override float SelectionItemHeight => ImGui.GetTextLineHeight() / ImGuiHelpers.GlobalScale;
//
//     protected override bool ShowListButton => true;
//
//     protected override bool FilterOptions(Module option)
//         => !System.SystemConfig.HideDisabledModules || option.IsEnabled;
//
//     public ConfigurationWindow() : base("DailyDuty - Configuration Window", new Vector2(1000.0f, 500.0f)) {
//         TitleBarButtons.Add(new TitleBarButton {
//             Click = _ => System.WindowManager.AddWindow(new ConfigurationManagerWindow(), WindowFlags.OpenImmediately),
//             Icon = FontAwesomeIcon.Cog,
//             ShowTooltip = () => ImGui.SetTooltip("Open Configuration Manager"),
//             IconOffset = new Vector2(2.0f, 1.0f),
//         });
//         
//         System.CommandManager.RegisterCommand(new CommandHandler {
//             Delegate = _ => UnCollapseOrToggle(),
//             ActivationPath = "/",
//         });
//     }
//
//     protected override void DrawListOption(Module option) {
//         ImGui.Text(option.ModuleName.GetDescription());
//         
//         ImGui.SameLine(ImGui.GetContentRegionAvail().X - 13.0f * ImGuiHelpers.GlobalScale);
//         using var _ = Services.PluginInterface.PushIconFont();
//
//         switch (option.ModuleStatus) {
//             case ModuleStatus.Suppressed when option.IsEnabled:
//                 ImGui.TextColored(KnownColor.MediumPurple.Vector(), FontAwesomeIcon.History.ToIconString());
//                 break;
//             
//             case ModuleStatus.Unknown when option.IsEnabled:
//                 ImGui.TextColored(KnownColor.Orange.Vector(), FontAwesomeIcon.Question.ToIconString());
//                 break;
//             
//             default:
//                 var color = option.IsEnabled ? KnownColor.Green.Vector() : KnownColor.Red.Vector();
//                 ImGui.TextColored(color, FontAwesomeIcon.Circle.ToIconString());
//                 break;
//         }
//     }
//
//     protected override void DrawSelectedOption(Module option) {
//         using var table = ImRaii.Table("module_table", 2, ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.Resizable, ImGui.GetContentRegionAvail());
//         if (!table) return;
//
//         ImGui.TableNextColumn();
//         DrawConfigPane(option);
//
//         ImGui.TableNextColumn();
//         DrawDataPane(option);
//     }
//
//     private static void DrawDataPane(Module option) {
//         using var dataChild = ImRaii.Child($"data_child_{option.ModuleName}", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding);
//         if (!dataChild) return;
//
//         option.DrawData();
//     }
//
//     private static void DrawConfigPane(Module option) {
//         using var configChild = ImRaii.Child($"config_child_{option.ModuleName}", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding);
//         if (!configChild) return;
//
//         option.DrawConfig();
//     }
//
//     protected override void DrawExtraButton() {
//         var label = System.SystemConfig.HideDisabledModules ? "Show Disabled" : "Hide Disabled";
//
//         if (ImGui.Button(label, ImGui.GetContentRegionAvail())) {
//             System.SystemConfig.HideDisabledModules = !System.SystemConfig.HideDisabledModules;
//             System.SystemConfig.Save();
//         }
//     }
//
//     public override void OnClose() => SaveAll();
//
//     public override void OnTabChanged() => SaveAll();
//
//     private void SaveAll() {
//         System.TodoListController.Save();
//         // System.TimersController.WeeklyTimerNode?.Save(System.TimersController.WeeklyTimerSavePath);
//         // System.TimersController.DailyTimerNode?.Save(System.TimersController.DailyTimerSavePath);
//
//         // foreach (var module in System.ModuleManager.Modules) {
//             // module.TodoTaskNode?.Save(StyleFileHelper.GetPath($"{module.ModuleName}.style.json"));
//         // }
//     }
// }
//
// public class TodoConfigTab : ITabItem {
//     public string Name => "Todo List";
//     public bool Disabled => false;
//
//     public void Draw() {
//         
//         ImGui.Text("To release an update in a timely manner, the Todo List feature has been omitted.");
//         ImGui.Spacing();
//         ImGui.Text("These systems are being rebuilt to include several new and super cool features, stay tuned for more information.");
//         
//         // if (System.TodoListController.TodoListNode is not { } listNode) return;
//         //
//         // var configChanged = false;
//         //
//         // using var id = ImRaii.PushId("main_config");
//         //
//         // ImGuiTweaks.Header("Todo List Config");
//         // using (ImRaii.PushIndent()) {
//         //     configChanged |= ImGui.Checkbox("Enable", ref System.TodoConfig.Enabled);
//         //
//         //     ImGuiHelpers.ScaledDummy(5.0f);
//         //     
//         //     var enableMoving = listNode.EnableMoving;
//         //     if (ImGui.Checkbox("Allow Moving", ref enableMoving)) {
//         //         listNode.EnableMoving = enableMoving;
//         //     }
//         //
//         //     var enableResizing = listNode.EnableResizing;
//         //     if (ImGui.Checkbox("Allow Resizing", ref enableResizing)) {
//         //         listNode.EnableResizing = enableResizing;
//         //     }
//         // }
//         //
//         // ImGuiTweaks.Header("Functional Options");
//         // using (ImRaii.PushIndent()) {
//         //     configChanged |= ImGui.Checkbox("Hide in Quest Event", ref System.TodoConfig.HideDuringQuests);
//         //     configChanged |= ImGui.Checkbox("Hide in Duties", ref System.TodoConfig.HideInDuties);
//         // }
//         //
//         // ImGuiTweaks.Header("Todo List Style");
//         // DrawTodoConfig();
//         //
//         // if (configChanged) {
//         //     System.TodoConfig.Save();
//         // }
//         //
//         // System.TodoListController.Refresh();
//     }
//
//     private static void DrawTodoConfig() {
//         using var tabBar = ImRaii.TabBar("mode_select");
//         if (!tabBar) return;
//
//         DrawSimpleModeConfig();
//         DrawAdvancedModeConfig();
//     }
//
//     private static void DrawSimpleModeConfig() {
//         using var simpleMode = ImRaii.TabItem("Simple Mode");
//         if (!simpleMode) return;
//
//         using var tabChild = ImRaii.Child("tab_child", ImGui.GetContentRegionAvail());
//         if (!tabChild) return;
//         
//         DrawBasicSimpleNodeTable();
//         ImGui.Spacing();
//
//         using var categoryTabBar = ImRaii.TabBar("category_tab_bar");
//         if (!categoryTabBar) return;
//
//         DrawSimpleDailyTab();
//         DrawSimpleWeeklyTab();
//         DrawSimpleSpecialTab();
//     }
//
//     private static void DrawSimpleDailyTab() {
//         using var dailyTab = ImRaii.TabItem("Daily Tasks");
//         if (!dailyTab) return;
//         
//         DrawSimpleCategoryConfig(System.TodoListController.DailyTaskNode);        
//     }
//     
//     private static void DrawSimpleWeeklyTab() {
//         using var weeklyTab = ImRaii.TabItem("Weekly Tasks");
//         if (!weeklyTab) return;
//         
//         DrawSimpleCategoryConfig(System.TodoListController.WeeklyTaskNode);        
//     }
//     
//     private static void DrawSimpleSpecialTab() {
//         using var specialTab = ImRaii.TabItem("Special Tasks");
//         if (!specialTab) return;
//         
//         DrawSimpleCategoryConfig(System.TodoListController.SpecialTaskNode);        
//     }
//
//     private static void DrawBasicSimpleNodeTable() {
//         using var table = ImRaii.Table("simple_mode_table", 2);
//         if (!table) return;
//
//         var todoController = System.TodoListController;
//
//         var dailyCategory = todoController.DailyTaskNode;
//         if (dailyCategory is null) return;
//         
//         var weeklyCategory = todoController.WeeklyTaskNode;
//         if (weeklyCategory is null) return;
//         
//         var specialCategory = todoController.SpecialTaskNode;
//         if (specialCategory is null) return;
//         
//         var listNode = todoController.TodoListNode;
//         if (listNode is null) return;
//                         
//         ImGui.TableSetupColumn("##label", ImGuiTableColumnFlags.WidthStretch, 1.0f);
//         ImGui.TableSetupColumn("##config", ImGuiTableColumnFlags.WidthStretch, 2.0f);
//                 
//         ImGui.TableNextRow();
//
//         ImGui.TableNextColumn();
//         ImGui.Text("Position");
//                         
//         ImGui.TableNextColumn();
//         var position = listNode.Position;
//         ImGuiTweaks.SetFullWidth();
//         if (ImGui.DragFloat2("##position", ref position, 0.75f, 0.0f, 5000.0f)) {
//             listNode.Position = position;
//         }
//
//         ImGui.TableNextColumn();
//         ImGui.Text("Size");
//                 
//         ImGui.TableNextColumn();
//         var size = listNode.Size;
//         ImGuiTweaks.SetFullWidth();
//         if (ImGui.DragFloat2("##size", ref size, 0.75f, 0.0f, 5000.0f)) {
//             listNode.Size = size;
//         }
//                 
//         ImGui.TableNextColumn();
//         ImGui.Text("Background Color");
//                 
//         ImGui.TableNextColumn();
//         var backgroundColor = listNode.BackgroundColor;
//         ImGuiTweaks.SetFullWidth();
//         if (ImGui.ColorEdit4("##BackgroundColor", ref backgroundColor, ImGuiColorEditFlags.AlphaPreviewHalf)) {
//             listNode.BackgroundColor = backgroundColor;
//         }
//                 
//         ImGui.TableNextColumn();
//         ImGui.Text("List Orientation");
//                 
//         ImGui.TableNextColumn();
//         var orientation = listNode.LayoutOrientation;
//         ImGuiTweaks.SetFullWidth();
//         if (ComboHelper.EnumCombo("##Orientation", ref orientation)) {
//             listNode.LayoutOrientation = orientation;
//         }
//
//         ImGui.TableNextColumn();
//         ImGui.Text("Anchor Corner");
//                 
//         ImGui.TableNextColumn();
//         var anchor = listNode.LayoutAnchor;
//         ImGuiTweaks.SetFullWidth();
//         if (ComboHelper.EnumCombo("##Anchor", ref anchor)) {
//             listNode.LayoutAnchor = anchor;
//
//             dailyCategory.TaskListNode.LayoutAnchor = anchor;
//             weeklyCategory.TaskListNode.LayoutAnchor = anchor;
//             specialCategory.TaskListNode.LayoutAnchor = anchor;
//
//             var alignmentDirection = anchor switch {
//                 LayoutAnchor.TopLeft => AlignmentType.TopLeft,
//                 LayoutAnchor.TopRight => AlignmentType.TopRight,
//                 LayoutAnchor.BottomLeft => AlignmentType.BottomLeft,
//                 LayoutAnchor.BottomRight => AlignmentType.BottomRight,
//                 _ => AlignmentType.TopLeft,
//             };
//
//             dailyCategory.HeaderTextNode.AlignmentType = alignmentDirection;
//             weeklyCategory.HeaderTextNode.AlignmentType = alignmentDirection;
//             specialCategory.HeaderTextNode.AlignmentType = alignmentDirection;
//         }
//
//         // ImGui.TableNextColumn();
//         // ImGui.Text("Category Vertical Spacing");
//         //
//         // ImGui.TableNextColumn();
//         // var categorySpacing = dailyCategory.Margin.Top;
//         // ImGuiTweaks.SetFullWidth();
//         // if (ImGui.DragFloat("##VerticalSpacing", ref categorySpacing, 0.10f, -10.0f, 5000.0f)) {
//         //     dailyCategory.Margin.Top = categorySpacing;
//         //     dailyCategory.Margin.Bottom = 0.0f;
//         //     dailyCategory.HeaderTextNode.Margin.Top = 0.0f;
//         //     dailyCategory.HeaderTextNode.Margin.Bottom = 0.0f;
//         //     
//         //     weeklyCategory.Margin.Top = categorySpacing;
//         //     weeklyCategory.Margin.Bottom = 0.0f;
//         //     weeklyCategory.HeaderTextNode.Margin.Top = 0.0f;
//         //     weeklyCategory.HeaderTextNode.Margin.Bottom = 0.0f;
//         //     
//         //     specialCategory.Margin.Top = categorySpacing;
//         //     specialCategory.Margin.Bottom = 0.0f;
//         //     specialCategory.HeaderTextNode.Margin.Top = 0.0f;
//         //     specialCategory.HeaderTextNode.Margin.Bottom = 0.0f;
//         // }
//         //
//         // ImGui.TableNextColumn();
//         // ImGui.Text("Category Horizontal Spacing");
//         //         
//         // ImGui.TableNextColumn();
//         // var horizontalSpacing = dailyCategory.Margin.Left;
//         // ImGuiTweaks.SetFullWidth();
//         // if (ImGui.DragFloat("##HorizontalSpacing", ref horizontalSpacing, 0.10f, -10.0f, 5000.0f)) {
//         //     dailyCategory.Margin.Left = horizontalSpacing;
//         //     dailyCategory.Margin.Right = 0.0f;
//         //     dailyCategory.HeaderTextNode.Margin.Left = 0.0f;
//         //     dailyCategory.HeaderTextNode.Margin.Right = 0.0f;
//         //     
//         //     weeklyCategory.Margin.Left = horizontalSpacing;
//         //     weeklyCategory.Margin.Right = 0.0f;
//         //     weeklyCategory.HeaderTextNode.Margin.Left = 0.0f;
//         //     weeklyCategory.HeaderTextNode.Margin.Right = 0.0f;
//         //     
//         //     specialCategory.Margin.Left = horizontalSpacing;
//         //     specialCategory.Margin.Right = 0.0f;
//         //     specialCategory.HeaderTextNode.Margin.Left = 0.0f;
//         //     specialCategory.HeaderTextNode.Margin.Right = 0.0f;
//         // }
//         
//         ImGui.TableNextColumn();
//         ImGui.Text("Show Background");
//
//         ImGui.TableNextColumn();
//         var background = listNode.ShowBackground;
//         ImGuiTweaks.SetFullWidth();
//         if (ImGui.Checkbox("##BackgroundVisible", ref background)) {
//             listNode.ShowBackground = background;
//         }
//                 
//         ImGui.TableNextColumn();
//         ImGui.Text("Show Border");
//
//         ImGui.TableNextColumn();
//         var border = listNode.ShowBorder;
//         ImGuiTweaks.SetFullWidth();
//         if (ImGui.Checkbox("##BorderVisible", ref border)) {
//             listNode.ShowBorder = border;
//         }
//     }
//
//     private static void DrawSimpleCategoryConfig(TodoCategoryNode? node) {
//         if (node is null) return;
//         var listNode = node.TaskListNode;
//         
//         using var table = ImRaii.Table("simple_mode_table", 2);
//         if (!table) return;
//         
//         ImGui.TableSetupColumn("##label", ImGuiTableColumnFlags.WidthStretch, 1.0f);
//         ImGui.TableSetupColumn("##config", ImGuiTableColumnFlags.WidthStretch, 2.0f);
//                 
//         ImGui.TableNextRow();
//
//         ImGui.TableNextColumn();
//         ImGui.Text("Header Color");
//
//         ImGui.TableNextColumn();
//         var headerColor = node.HeaderTextNode.TextColor;
//         ImGuiTweaks.SetFullWidth();
//         if (ImGui.ColorEdit4("##HeaderColor", ref headerColor, ImGuiColorEditFlags.AlphaPreviewHalf)) {
//             node.HeaderTextNode.TextColor = headerColor;
//         }
//         
//         ImGui.TableNextColumn();
//         ImGui.Text("Alignment");
//         
//         ImGui.TableNextColumn();
//         var alignment = listNode.LayoutOrientation;
//         ImGuiTweaks.SetFullWidth();
//         if (ComboHelper.EnumCombo("##Alignment", ref alignment)) {
//             listNode.LayoutOrientation = alignment;
//         }
//
//         ImGui.TableNextColumn();
//         ImGui.Text("Font Size");
//
//         ImGui.TableNextColumn();
//         var firstNode = node.TaskNodes.FirstOrDefault();
//
//         if (firstNode is not null) {
//             var fontSize = (int) firstNode.FontSize;
//             ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
//             if (ImGui.InputInt("##FontSize", ref fontSize)) {
//                 foreach (var taskNode in node.TaskNodes.OfType<TodoTaskNode>()) {
//                     taskNode.FontSize = (uint) fontSize;
//                     taskNode.String = taskNode.String;
//                 }
//             }
//         }
//         
//         ImGui.TableNextColumn();
//         ImGui.Text("Show Header");
//         
//         ImGui.TableNextColumn();
//         var showHeader = node.HeaderTextNode.IsVisible;
//         ImGuiTweaks.SetFullWidth();
//         if (ImGui.Checkbox("##HeaderVisible", ref showHeader)) {
//             node.HeaderTextNode.IsVisible = showHeader;
//         }
//     }
//
//     private static void DrawAdvancedModeConfig() {
//         using var advancedMode = ImRaii.TabItem("Advanced Mode");
//         if (!advancedMode) return;
//         
//         using var tabChild = ImRaii.Child("tab_child", ImGui.GetContentRegionAvail());
//         if (!tabChild) return;
//
//         // System.TodoListController.DrawConfig();
//     }
// }
//
// public class TimersConfigTab : ITabItem {
//     public string Name => "Timers";
//     public bool Disabled => false;
//     public void Draw() {
//         ImGui.Text("To release an update in a timely manner, the Timers feature has been omitted.");
//         ImGui.Spacing();
//         ImGui.Text("These systems are being rebuilt to include several new and super cool features, stay tuned for more information.");
//         
//         // if (System.TimersController.DailyTimerNode is not { } dailyTimerNode) return;
//         // if (System.TimersController.WeeklyTimerNode is not { } weeklyTimerNode) return;
//         //
//         // var configChanged = false;
//         //
//         // ImGuiTweaks.Header("Timers Config");
//         // using (ImRaii.PushIndent()) {
//         //     configChanged |= ImGui.Checkbox("Enable", ref System.TimersConfig.Enabled);
//         //     
//         //     ImGuiHelpers.ScaledDummy(5.0f);
//         //     
//         //     var enableMoving = dailyTimerNode.EnableMoving;
//         //     if (ImGui.Checkbox("Allow Moving", ref enableMoving)) {
//         //         dailyTimerNode.EnableMoving = enableMoving;
//         //         weeklyTimerNode.EnableMoving = enableMoving;
//         //     }
//         //
//         //     var enableResizing = dailyTimerNode.EnableResizing;
//         //     if (ImGui.Checkbox("Allow Resizing", ref enableResizing)) {
//         //         dailyTimerNode.EnableResizing = enableResizing;
//         //         weeklyTimerNode.EnableResizing = enableResizing;
//         //     }
//         //
//         //     ImGuiHelpers.ScaledDummy(5.0f);
//         //     
//         //     configChanged |= ImGui.Checkbox("Daily Timer Enable", ref System.TimersConfig.EnableDailyTimer);
//         //     configChanged |= ImGui.Checkbox("Weekly Timer Enable", ref System.TimersConfig.EnableWeeklyTimer);
//         //     
//         //     ImGuiHelpers.ScaledDummy(5.0f);
//         //
//         //     configChanged |= ImGui.Checkbox("Hide in Duties", ref System.TimersConfig.HideInDuties);
//         //     configChanged |= ImGui.Checkbox("Hide in Quest Event", ref System.TimersConfig.HideInQuestEvents);
//         //     
//         //     ImGuiHelpers.ScaledDummy(5.0f);
//         //     configChanged |= ImGui.Checkbox("Hide Seconds", ref System.TimersConfig.HideTimerSeconds);
//         //
//         // }
//         //
//         // ImGuiHelpers.ScaledDummy(10.0f);
//         // ImGui.Separator();
//         // ImGuiHelpers.ScaledDummy(5.0f);
//         //
//         // DrawTimersConfig();
//         //
//         // if (configChanged) {
//         //     System.TimersConfig.Save();
//         // }
//     }
//
//     private void DrawTimersConfig() {
//         using var table = ImRaii.Table("special_timers_config", 2);
//         if (!table) return;
//         
//         ImGui.TableNextColumn();
//         DrawWeeklyConfig();
//                 
//         ImGui.TableNextColumn();
//         DrawDailyConfig();
//     }
//
//     private void DrawDailyConfig() {
//         using var weeklyChild = ImRaii.Child("daily_child", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding);
//         if (!weeklyChild) return;
//
//         using (ImRaii.PushId("Daily")) {
//             ImGui.TextUnformatted("Daily Timer");
//             ImGuiHelpers.ScaledDummy(5.0f);
//             DrawTimerConfig(System.TimersController.DailyTimerNode);
//         }
//     }
//
//     private void DrawWeeklyConfig() {
//         using var weeklyChild = ImRaii.Child("weekly_child", ImGui.GetContentRegionAvail() - ImGui.GetStyle().FramePadding);
//         if (!weeklyChild) return;
//         
//         using (ImRaii.PushId("Weekly")) {
//             ImGui.TextUnformatted("Weekly Timer");
//             ImGuiHelpers.ScaledDummy(5.0f);
//             DrawTimerConfig(System.TimersController.WeeklyTimerNode);
//         }
//     }
//
//     private void DrawTimerConfig(TimerNode? node) {
//         if (node is null) return;
//
//         using var tabBar = ImRaii.TabBar("node_config_tab_bar");
//         if (!tabBar) return;
//
//         DrawSimpleModeConfig(node);
//         DrawAdvancedModeConfig(node);
//     }
//
//     private static void DrawAdvancedModeConfig(TimerNode node) {
//         using var advancedMode = ImRaii.TabItem("Advanced Mode");
//         if (!advancedMode) return;
//         
//         // node.DrawConfig();
//     }
//
//     private static void DrawSimpleModeConfig(TimerNode node) {
//         using var simpleMode = ImRaii.TabItem("Simple Mode");
//         if (!simpleMode) return;
//
//         using var table = ImRaii.Table("simple_mode_table", 2);
//         if (!table) return;
//
//         ImGui.TableSetupColumn("##label", ImGuiTableColumnFlags.WidthStretch, 1.0f);
//         ImGui.TableSetupColumn("##config", ImGuiTableColumnFlags.WidthStretch, 2.0f);
//         
//         ImGui.TableNextRow();
//
//         ImGui.TableNextColumn();
//         ImGui.Text("Position");
//
//         ImGui.TableNextColumn();
//         var position = node.Position;
//         ImGuiTweaks.SetFullWidth();
//         if (ImGui.DragFloat2("##Position", ref position, 0.75f, 0.0f, 5000.0f)) {
//             node.Position = position;
//         }
//
//         ImGui.TableNextColumn();
//         ImGui.Text("Size");
//
//         ImGui.TableNextColumn();
//         var size = node.Size;
//         ImGuiTweaks.SetFullWidth();
//         if (ImGui.DragFloat2("##Size", ref size, 0.50f, 0.0f, 5000.0f)) {
//             node.Size = size;
//         }
//                 
//         ImGui.TableNextColumn();
//         ImGui.Text("Bar Color");
//                 
//         ImGui.TableNextColumn();
//         var color = node.BarColor;
//         ImGuiTweaks.SetFullWidth();
//         if (ImGui.ColorEdit4("##BarColor", ref color, ImGuiColorEditFlags.AlphaPreviewHalf)) {
//             node.BarColor = color;
//         }
//                 
//         ImGui.TableNextColumn();
//         ImGui.Text("Label Color");
//                 
//         ImGui.TableNextColumn();
//         var labelColor = node.LabelColor;
//         ImGuiTweaks.SetFullWidth();
//         if (ImGui.ColorEdit4("##LabelColor", ref labelColor, ImGuiColorEditFlags.AlphaPreviewHalf)) {
//             node.LabelColor = labelColor;
//         }
//                 
//         ImGui.TableNextColumn();
//         ImGui.Text("Timer Color");
//                 
//         ImGui.TableNextColumn();
//         var timerColor = node.TimerColor;
//         ImGuiTweaks.SetFullWidth();
//         if (ImGui.ColorEdit4("##TimerColor", ref timerColor, ImGuiColorEditFlags.AlphaPreviewHalf)) {
//             node.TimerColor = timerColor;
//         }
//                 
//         ImGui.TableNextColumn();
//         ImGui.Text("Show Label");
//                 
//         ImGui.TableNextColumn();
//         var showText = node.ShowLabel;
//         ImGuiTweaks.SetFullWidth();
//         if (ImGui.Checkbox("##ShowText", ref showText)) {
//             node.ShowLabel = showText;
//         }
//                 
//         ImGui.TableNextColumn();
//         ImGui.Text("Show Timer");
//                 
//         ImGui.TableNextColumn();
//         var showTimer = node.ShowTimer;
//         ImGuiTweaks.SetFullWidth();
//         if (ImGui.Checkbox("##ShowTimer", ref showTimer)) {
//             node.ShowTimer = showTimer;
//         }
//     }
// }
//
// public class ServerInfoBarTab : ITabItem {
//     public string Name => "Server Info Bar";
//     public bool Disabled => false;
//
//     public void Draw() {
//         ImGuiTweaks.Header("Server Info Bar Config");
//         using var indent = ImRaii.PushIndent();
//         
//         if (System.DtrController.Config is { } dtrEntry) {
//             dtrEntry.DrawConfig();
//         }
//         else {
//             ImGui.TextColored(KnownColor.OrangeRed.Vector(), "Failed to load Server Info Bar Controller.");
//         }
//     }
// }
