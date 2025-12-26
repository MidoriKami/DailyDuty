// using System;
// using System.Drawing;
// using System.Linq;
// using System.Numerics;
// using DailyDuty.Classes;
// using DailyDuty.Models;
// using DailyDuty.Modules.BaseModules;
// using Dalamud.Bindings.ImGui;
// using Dalamud.Interface;
// using Dalamud.Interface.Utility;
// using FFXIVClientStructs.FFXIV.Client.Game;
// using FFXIVClientStructs.FFXIV.Client.UI;
// using FFXIVClientStructs.FFXIV.Client.UI.Agent;
// using FFXIVClientStructs.FFXIV.Component.GUI;
// using KamiLib.Classes;
// using KamiToolKit.Classes;
// using KamiToolKit.Classes.Controllers;
// using KamiToolKit.Extensions;
// using KamiToolKit.Nodes;
// using Lumina.Excel.Sheets;
// using Lumina.Text.ReadOnly;
// using InstanceContent = FFXIVClientStructs.FFXIV.Client.Game.UI.InstanceContent;
// using SeStringBuilder = Lumina.Text.SeStringBuilder;
//
// namespace DailyDuty.Modules;
//
// public class DutyRouletteData : ModuleTaskData<ContentRoulette> {
//     public int ExpertTomestones;
//     public int ExpertTomestoneCap;
//     public bool AtTomeCap;
//
//     protected override void DrawModuleData() {
//         DrawDataTable(
//             ("Current Weekly Tomestones", ExpertTomestones.ToString()),
//             ("Weekly Tomestone Limit", ExpertTomestoneCap.ToString()),
//             ("At Weekly Limit?", AtTomeCap.ToString())
//         );
//         
//         base.DrawModuleData();
//     }
// }
//
// public class DutyRouletteConfig : ModuleTaskConfig<ContentRoulette> {
//     public bool CompleteWhenCapped;
//     public bool ClickableLink = true;
//     public bool ColorContentFinder = true;
//     public Vector4 CompleteColor = KnownColor.LimeGreen.Vector();
//     public Vector4 IncompleteColor = KnownColor.OrangeRed.Vector();
//     public bool ShowOpenDailyDutyButton = true;
//     public bool ShowResetTimer = true;
//     public Vector4 TimerColor = KnownColor.Black.Vector();
//     
//     protected override void DrawModuleConfig() {
//         SavePending |= ImGui.Checkbox("Clickable Link", ref ClickableLink);
//         SavePending |= ImGui.Checkbox("Mark complete when tomecapped", ref CompleteWhenCapped);
//         SavePending |= ImGui.Checkbox("Show 'Open DailyDuty' button", ref ShowOpenDailyDutyButton);
//         
//         ImGui.Spacing();
//
//         SavePending |= ImGui.Checkbox("Show Daily Reset Timer in Duty Finder", ref ShowResetTimer);
//
//         if (ShowResetTimer) {
//             SavePending |= ImGuiTweaks.ColorEditWithDefault("Timer Color", ref TimerColor, ColorHelper.GetColor(7));
//         }
//         
//         ImGui.Spacing();
//
//         SavePending |= ImGui.Checkbox("Color Duty Finder", ref ColorContentFinder);
//         
//         if (ColorContentFinder) {
//             ImGuiHelpers.ScaledDummy(5.0f);
//
//             SavePending |= ImGuiTweaks.ColorEditWithDefault("Complete Color", ref CompleteColor, KnownColor.LimeGreen.Vector());
//             SavePending |= ImGuiTweaks.ColorEditWithDefault("Incomplete Color", ref IncompleteColor, KnownColor.OrangeRed.Vector());
//         }
//         
//         ImGuiHelpers.ScaledDummy(5.0f);
//         
//         base.DrawModuleConfig();
//     }
// }
//
// public unsafe class DutyRoulette : BaseModules.Modules.DailyTask<DutyRouletteData, DutyRouletteConfig, ContentRoulette> {
//     public override ModuleName ModuleName => ModuleName.DutyRoulette;
//
//     public override bool HasClickableLink => Config.ClickableLink;
//     
//     public override PayloadId ClickableLinkPayloadId => PayloadId.OpenDutyFinderRoulette;
//
//     public override bool HasTooltip => true;
//
//     private TextNode? infoTextNode;
//     private TextButtonNode? openDailyDutyButton;
//     private TextNode? dailyResetTimer;
//
//     private readonly NativeListController rouletteListController;
//
//     public DutyRoulette() {
//         rouletteListController = new NativeListController("ContentsFinder") {
//             ShouldModifyElement = ShouldModifyElement,
//             UpdateElement = UpdateElement,
//             ResetElement = ResetElement,
//             GetPopulatorNode = GetPopulatorNode,
//         };
//         
//         System.ContentsFinderController.OnAttach += AttachNodes;
//         System.ContentsFinderController.OnDetach += DetachNodes;
//         System.ContentsFinderController.OnUpdate += OnContentFinderUpdate;
//     }
//     
//     public override void Dispose() {
//         rouletteListController.Dispose();
//     }
//
//     private AtkComponentListItemRenderer* GetPopulatorNode(AtkUnitBase* addon) {
//         var contentsFinder = (AddonContentsFinder*) addon;
//         return contentsFinder->DutyList->GetItemRendererByNodeId(6);
//     }
//
//     private bool ShouldModifyElement(AtkUnitBase* unitBase, ListItemData listItemData, AtkResNode** nodeList) {
//         var contentData = GetContentData(listItemData.ItemInfo);
//
//         if (!Config.ColorContentFinder) return false;
//         if (contentData.ContentType is not ContentsId.ContentsType.Roulette) return false;
//         
//         var contentRoulette = Services.DataManager.GetExcelSheet<ContentRoulette>().GetRow(contentData.Id);
//         return Config.TaskConfig.FirstOrDefault(task => task.RowId == contentRoulette.RowId) is { Enabled: true };
//     }
//
//     private void UpdateElement(AtkUnitBase* unitBase, ListItemData listItemData, AtkResNode** nodeList) {
//         var dutyNameTextNode = (AtkTextNode*) nodeList[3];
//         var contentData = GetContentData(listItemData.ItemInfo);
//         var contentRoulette = Services.DataManager.GetExcelSheet<ContentRoulette>().GetRow(contentData.Id);
//
//         var isRouletteCompleted = InstanceContent.Instance()->IsRouletteComplete((byte) contentRoulette.RowId);
//         dutyNameTextNode->TextColor = isRouletteCompleted ? Config.CompleteColor.ToByteColor() : Config.IncompleteColor.ToByteColor();
//     }
//
//     private void ResetElement(AtkUnitBase* unitBase, ListItemData listItemData, AtkResNode** nodeList) {
//         var dutyNameTextNode = (AtkTextNode*) nodeList[3];
//         var levelTextNode = (AtkTextNode*) nodeList[4];
//         
//         dutyNameTextNode->TextColor = levelTextNode->TextColor;
//     }
//
//     private static ContentsId GetContentData(AtkComponentListItemPopulator.ListItemInfo* listItemInfo) {
//         var contentId = listItemInfo->ListItem->UIntValues[1];
//         var contentEntry = AgentContentsFinder.Instance()->ContentList[contentId - 1];
//         var contentData = contentEntry.Value->Id;
//         return contentData;
//     }
//
//     private void AttachNodes(AddonContentsFinder* addon) {
//         var targetResNode = addon->GetNodeById(56);
//         if (targetResNode is null) return;
//
//         infoTextNode = new TextNode {
//             NodeId = 1000,
//             X = 16.0f,
//             Y = targetResNode->GetYFloat() + 2.0f, 
//             TextFlags = TextFlags.AutoAdjustNodeSize,
//             AlignmentType = AlignmentType.TopLeft,
//             SeString = GetHintText(),
//             Tooltip = "Feature from DailyDuty Plugin",
//             IsVisible = false,
//         };
//         infoTextNode.AttachNode(targetResNode, NodePosition.AfterTarget);
//         
//         openDailyDutyButton = new TextButtonNode {
//             Position = new Vector2(50.0f, 622.0f),
//             Size = new Vector2(130.0f, 28.0f),
//             IsVisible = true,
//             String = "Open DailyDuty",
//         };
//         openDailyDutyButton.AddEvent(AtkEventType.ButtonClick, () => System.WindowManager.GetWindow<ConfigurationWindow>()?.UnCollapseOrToggle() );
//         openDailyDutyButton.AttachNode(addon->RootNode);
//
//         var targetComponent = GetListHeaderComponentNode(addon);
//         if (targetComponent is not null) {
//             dailyResetTimer = new TextNode {
//                 Position = new Vector2(targetComponent->X, targetComponent->Y),
//                 Size = new Vector2(targetComponent->Width, targetComponent->Height),
//                 AlignmentType = AlignmentType.Center,
//                 Tooltip = "[DailyDuty] Time until next daily reset",
//                 String = "0:00:00:00",
//                 TextColor = Config.TimerColor,
//             };
//
//             dailyResetTimer.AddFlags(NodeFlags.HasCollision);
//
//             dailyResetTimer.AttachNode(targetComponent);
//         }
//         
//         if (Config.TimerColor == Vector4.Zero) {
//             Config.TimerColor = ColorHelper.GetColor(7);
//             SavePending = true;
//         }
//     }
//
//     private void DetachNodes(AddonContentsFinder* addon) {
//         infoTextNode?.Dispose();
//         openDailyDutyButton?.Dispose(); 
//         dailyResetTimer?.Dispose();
//     }
//
//     private void OnContentFinderUpdate(AddonContentsFinder* addon) {
//         openDailyDutyButton?.IsVisible = Config.ShowOpenDailyDutyButton;
//
//         if (dailyResetTimer is not null && Config.ShowResetTimer) {
//             var nextReset = Time.NextDailyReset();
//             var timeRemaining = nextReset - DateTime.UtcNow;
//             
//             dailyResetTimer.String = timeRemaining.FormatTimeSpanShort(System.TimersConfig?.HideTimerSeconds ?? false);
//             dailyResetTimer.TextColor = Config.TimerColor;
//         }
//
//         dailyResetTimer?.IsVisible = Config.ShowResetTimer && addon->SelectedRadioButton == 0;
//
//         infoTextNode?.IsVisible = rouletteListController.ModifiedIndexes.Count is not 0 && addon->SelectedRadioButton is 0;
//     }
//
//     private static AtkComponentNode* GetListHeaderComponentNode(AddonContentsFinder* addon)
//         => addon->DutyList->CategoryItemRendererList->AtkComponentListItemRenderer->ComponentNode;
//
//     private ReadOnlySeString GetHintText()
//         => new SeStringBuilder()
//            .PushColorRgba(Config.IncompleteColor)
//            .Append("Incomplete Task")
//            .PopColor()
//            .Append("        ")
//            .PushColorRgba(Config.CompleteColor)
//            .Append("Complete Task")
//            .PopColor()
//            .ToReadOnlySeString();
//
//     protected override void UpdateTaskLists() {
//         var luminaUpdater = new LuminaTaskUpdater<ContentRoulette>(this, roulette => roulette.DutyType.ToString() != string.Empty);
//         luminaUpdater.UpdateConfig(Config.TaskConfig);
//         luminaUpdater.UpdateData(Data.TaskData);
//     }
//
//     public override void Update() {
//         Data.TaskData.Update(ref DataChanged, rowId => InstanceContent.Instance()->IsRouletteComplete((byte) rowId));
//
//         Data.ExpertTomestones = TryUpdateData(Data.ExpertTomestones, InventoryManager.Instance()->GetWeeklyAcquiredTomestoneCount());
//         Data.ExpertTomestoneCap = TryUpdateData(Data.ExpertTomestoneCap, InventoryManager.GetLimitedTomestoneWeeklyLimit());
//         Data.AtTomeCap = TryUpdateData(Data.AtTomeCap, Data.ExpertTomestones == Data.ExpertTomestoneCap);
//         
//         base.Update();
//     }
//
//     public override void Reset() {
//         Data.TaskData.Reset();
//         
//         base.Reset();
//     }
//
//     protected override ModuleStatus GetModuleStatus() {
//         if (Config.CompleteWhenCapped && Data.AtTomeCap) return ModuleStatus.Complete;
//
//         return IncompleteTaskCount == 0 ? ModuleStatus.Complete : ModuleStatus.Incomplete;
//     }
//     
//     protected override StatusMessage GetStatusMessage() => new LinkedStatusMessage {
//         Message = $"{IncompleteTaskCount} roulettes remaining", 
//         LinkEnabled = Config.ClickableLink, 
//         Payload = PayloadId.OpenDutyFinderRoulette,
//     };
// }
