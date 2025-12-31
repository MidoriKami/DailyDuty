using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.Controllers;
using KamiToolKit.Extensions;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using SeStringBuilder = Lumina.Text.SeStringBuilder;

namespace DailyDuty.Features.WondrousTails;

public unsafe class ContentsFinderController : IDisposable {
    private readonly WondrousTails module;
    private readonly NativeListController listController;
    private readonly AddonController<AddonContentsFinder> addonController;

    private readonly Dictionary<uint, WondrousTailsNode> imageNodes = [];
    private readonly Dictionary<uint, List<uint>> wondrousTailsDuties = [];
    private TextNode? infoTextNode;
    private WondrousTailsNode? infoTailsNode;
    
    public ContentsFinderController(WondrousTails module) {
        this.module = module;

        listController = new NativeListController("ContentsFinder") {
            GetPopulatorNode = GetPopulatorMethod,
            ShouldModifyElement = ShouldModifyElementMethod,
            UpdateElement = UpdateElementMethod,
            ResetElement = ResetElementMethod,
        };
        
        listController.Enable();
        
        addonController = new AddonController<AddonContentsFinder>("ContentsFinder");
        addonController.OnAttach += ContentsFinderSetup;
        addonController.OnRefresh += ContentsFinderRefresh;
        addonController.OnDetach += _ => {
            foreach (var imageNode in imageNodes.Values) {
                imageNode.Dispose();
            }
            
            imageNodes.Clear();
            
            infoTailsNode?.Dispose();
            infoTailsNode = null;
            
            infoTextNode?.Dispose();
            infoTextNode = null;
        };
        
        addonController.Enable();
    }


    public void Dispose() {
        listController.Dispose();
        addonController.Dispose();
    }
    
    private void ContentsFinderSetup(AddonContentsFinder* addon) {
        wondrousTailsDuties.Clear();

        foreach (var index in Enumerable.Range(0, 16)) {
            var tailsTaskId = PlayerState.Instance()->WeeklyBingoOrderData[index];
            var tailsDuties = Services.DataManager.GetDutiesForOrderData(tailsTaskId).Select(cfc => cfc.RowId).ToList();
            wondrousTailsDuties.Add((uint)index, tailsDuties);
        }

        var targetResNode = addon->GetNodeById(56);
        if (targetResNode is null) return;

        infoTextNode = new TextNode {
            Position = new Vector2(16.0f, targetResNode->GetYFloat() + 2.0f),
            Size = new Vector2(250.0f, 18.0f),
            AlignmentType = AlignmentType.Center,
            SeString = new SeStringBuilder()
                .PushColorRgba(module.ModuleConfig.DutyFinderColor)
                .Append("Wondrous Tails Duty")
                .PopColor()
                .ToReadOnlySeString(),
            TextTooltip = "[DailyDuty] Wondrous Tails Feature",
            IsVisible = false,
        };
        infoTextNode.AttachNode(targetResNode, NodePosition.AfterTarget);

        infoTailsNode = new WondrousTailsNode {
            Size = new Vector2(18.0f, 18.0f), 
            Position = new Vector2(35.0f, -4.0f), 
            IsTaskAvailable = true,
        };
        infoTailsNode.AttachNode(infoTextNode);
    }
    
    private void ContentsFinderRefresh(AddonContentsFinder* addon) {
        var shouldShow = listController.ModifiedIndexes.Count is not 0 && addon->SelectedRadioButton is not 0;

        infoTextNode?.ShowClickableCursor = shouldShow;
        infoTextNode?.IsVisible = shouldShow;
        infoTailsNode?.IsVisible = shouldShow && module.ModuleConfig.CloverIndicator;

        addon->UpdateCollisionNodeList(false);
    }

    private AtkComponentListItemRenderer* GetPopulatorMethod(AtkUnitBase* addon) {
        var contentsFinder = (AddonContentsFinder*) addon;
        return contentsFinder->DutyList->GetItemRendererByNodeId(6);
    }

    private bool ShouldModifyElementMethod(AtkUnitBase* unitBase, ListItemData listItemData, AtkResNode** nodeList) {
        var contentId = listItemData.ItemInfo->ListItem->UIntValues[1];
        var contentEntry = AgentContentsFinder.Instance()->ContentList[contentId - 1];
        var contentData = contentEntry.Value->Id;

        if (module.ModuleConfig is { CloverIndicator: false, ColorDutyFinderText: false }) return false;
        if (contentData.ContentType is not ContentsId.ContentsType.Regular) return false;
		
        var cfc = Services.DataManager.GetExcelSheet<ContentFinderCondition>().GetRow(contentData.Id);
        return IsTailsTask(cfc);
    }
    
    private void UpdateElementMethod(AtkUnitBase* unitBase, ListItemData listItemData, AtkResNode** nodeList) {
        var dutyNameTextNode = (AtkTextNode*) nodeList[3];
        var levelTextNode = (AtkTextNode*) nodeList[4];

        var index = listItemData.ItemInfo->ListItem->Renderer->OwnerNode->NodeId;
		
        var contentId = listItemData.ItemInfo->ListItem->UIntValues[1];
        var contentEntry = AgentContentsFinder.Instance()->ContentList[contentId - 1];
        var contentData = contentEntry.Value->Id;
        var cfc = Services.DataManager.GetExcelSheet<ContentFinderCondition>().GetRow(contentData.Id);

        // If clover is enabled
        if (module.ModuleConfig.CloverIndicator) {
			
            // And it is attached already, update it
            if (imageNodes.TryGetValue(index, out var node)) {
                node.IsVisible = true;
                node.IsTaskAvailable = IsTaskAvailable(cfc);
            }
			
            // else make it and attach it
            else {
                dutyNameTextNode->Width = (ushort) (dutyNameTextNode->Width - 24.0f);

                var newNode = new WondrousTailsNode {
                    Size = new Vector2(24.0f, 24.0f),
                    Position = new Vector2(dutyNameTextNode->X + dutyNameTextNode->Width, 0.0f),
                    IsTaskAvailable = IsTaskAvailable(cfc),
                };
                newNode.AttachNode((AtkResNode*)dutyNameTextNode, NodePosition.AfterTarget);

                imageNodes.Add(index, newNode);
            }
        }

        if (module.ModuleConfig.ColorDutyFinderText) {
            if (IsTaskAvailable(cfc)) {
                dutyNameTextNode->TextColor = module.ModuleConfig.DutyFinderColor.ToByteColor();
            }
            else {
                dutyNameTextNode->TextColor = levelTextNode->TextColor;
            }
        }
        else {
            dutyNameTextNode->TextColor = levelTextNode->TextColor;
        }
    }
    
    private void ResetElementMethod(AtkUnitBase* unitBase, ListItemData listItemData, AtkResNode** nodeList) {
        var dutyNameTextNode = (AtkTextNode*) nodeList[3];
        var levelTextNode = (AtkTextNode*) nodeList[4];
        var index = listItemData.ItemInfo->ListItem->Renderer->OwnerNode->NodeId;

        // Remove node
        if (imageNodes.TryGetValue(index, out var node)) {
            dutyNameTextNode->Width = (ushort) (dutyNameTextNode->Width + 24.0f);

            imageNodes.Remove(index);
            node.Dispose();
        }
		
        // Reset Color
        dutyNameTextNode->TextColor = levelTextNode->TextColor;
    }

    private bool IsTailsTask(ContentFinderCondition cfc)
        => wondrousTailsDuties.Values.Any(dutyList => dutyList.Contains(cfc.RowId));
    
    private bool IsTaskAvailable(ContentFinderCondition cfc) {
        var taskState = GetStatusForDuty(cfc.RowId);
        return taskState is PlayerState.WeeklyBingoTaskStatus.Claimable or PlayerState.WeeklyBingoTaskStatus.Open;
    }
    
    private PlayerState.WeeklyBingoTaskStatus? GetStatusForDuty(uint cfc) { 
        foreach(var (taskId, dutyList) in wondrousTailsDuties) {
            if (dutyList.Contains(cfc)) {
                return PlayerState.Instance()->GetWeeklyBingoTaskStatus((int)taskId);
            }
        }

        return null;
    }
}
