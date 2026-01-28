using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Controllers;
using KamiToolKit.Extensions;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using InstanceContent = FFXIVClientStructs.FFXIV.Client.Game.UI.InstanceContent;
using SeStringBuilder = Lumina.Text.SeStringBuilder;

namespace DailyDuty.Features.DutyRoulette;

public unsafe class DutyRouletteDutyFinderController : IDisposable {
    private readonly DutyRoulette module;
    private readonly NativeListController listController;
    private readonly AddonController<AddonContentsFinder> addonController;
    private TextNode? infoTextNode;

    public DutyRouletteDutyFinderController(DutyRoulette module) {
        this.module = module;

        listController = new NativeListController("ContentsFinder") {
            GetPopulatorNode = GetPopulatorMethod,
            ShouldModifyElement = ShouldModifyElementMethod,
            UpdateElement = UpdateElementMethod,
            ResetElement = ResetElementMethod,
        };
        
        listController.Enable();
        
        addonController = new AddonController<AddonContentsFinder>("ContentsFinder");
        addonController.OnAttach += addon => {
            var targetResNode = addon->GetNodeById(56);
            if (targetResNode is null) return;
            
            infoTextNode = new TextNode {
                Position = new Vector2(16.0f, targetResNode->GetYFloat() + 2.0f),
                Size = new Vector2(250.0f, 18.0f),
                AlignmentType = AlignmentType.Center,
                String = new SeStringBuilder()
                    .PushColorRgba(module.ModuleConfig.IncompleteColor)
                    .Append("Incomplete Task")
                    .PopColor()
                    .Append("        ")
                    .PushColorRgba(module.ModuleConfig.CompleteColor)
                    .Append("Complete Task")
                    .PopColor()
                    .ToReadOnlySeString(),
                TextTooltip = "[DailyDuty] Duty Roulette Feature",
                IsVisible = false,
            };
            infoTextNode.AttachNode(targetResNode, NodePosition.AfterTarget);
        };

        addonController.OnDetach += _ => {
            infoTextNode?.Dispose();
            infoTextNode = null;
        };

        addonController.OnRefresh += addon => {
            var shouldShow = listController.ModifiedIndexes.Count is not 0 && addon->SelectedRadioButton is 0;
            
            infoTextNode?.ShowClickableCursor = shouldShow;
            infoTextNode?.IsVisible = shouldShow;
            
            if (!shouldShow) {
                infoTextNode?.RemoveNodeFlags(NodeFlags.RespondToMouse, NodeFlags.EmitsEvents);
            }
            else {
                infoTextNode?.AddNodeFlags(NodeFlags.RespondToMouse, NodeFlags.EmitsEvents);
            }
            
            addon->UpdateCollisionNodeList(false);
        };
        
        addonController.Enable();
    }

    public void Dispose() {
        listController.Dispose();
        addonController.Dispose();
    }

    private static AtkComponentListItemRenderer* GetPopulatorMethod(AtkUnitBase* addon) {
        var contentsFinder = (AddonContentsFinder*) addon;
        return contentsFinder->DutyList->GetComponentItemRendererById(6);
    }

    private bool ShouldModifyElementMethod(AtkUnitBase* unitBase, ListItemData listItemData, AtkResNode** nodeList) {
        var contentData = GetContentData(listItemData.ItemInfo);
    
        if (!module.ModuleConfig.ColorContentFinder) return false;
        if (contentData.ContentType is not ContentsId.ContentsType.Roulette) return false;
        
        return module.ModuleConfig.TrackedRoulettes.Contains(contentData.Id);
    }
    
    private void UpdateElementMethod(AtkUnitBase* unitBase, ListItemData listItemData, AtkResNode** nodeList) {
        var dutyNameTextNode = (AtkTextNode*) nodeList[3];
        var contentData = GetContentData(listItemData.ItemInfo);
        var contentRoulette = Services.DataManager.GetExcelSheet<ContentRoulette>().GetRow(contentData.Id);
    
        var isRouletteCompleted = InstanceContent.Instance()->IsRouletteComplete((byte) contentRoulette.RowId);
        dutyNameTextNode->TextColor = isRouletteCompleted ? module.ModuleConfig.CompleteColor.ToByteColor() : module.ModuleConfig.IncompleteColor.ToByteColor();
    }
    
    private static void ResetElementMethod(AtkUnitBase* unitBase, ListItemData listItemData, AtkResNode** nodeList) {
        var dutyNameTextNode = (AtkTextNode*) nodeList[3];
        var levelTextNode = (AtkTextNode*) nodeList[4];
        
        dutyNameTextNode->TextColor = levelTextNode->TextColor;
    }
    
    private static ContentsId GetContentData(AtkComponentListItemPopulator.ListItemInfo* listItemInfo) {
        var contentId = listItemInfo->ListItem->UIntValues[1];
        var contentEntry = AgentContentsFinder.Instance()->ContentList[contentId - 1];
        var contentData = contentEntry.Value->Id;
        return contentData;
    }
}
