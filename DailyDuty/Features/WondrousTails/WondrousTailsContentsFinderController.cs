using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Controllers;
using KamiToolKit.Extensions;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using SeStringBuilder = Lumina.Text.SeStringBuilder;

namespace DailyDuty.Features.WondrousTails;

public unsafe class WondrousTailsContentsFinderController : IDisposable {
    private readonly WondrousTails module;
    private readonly NativeListController<AddonContentsFinder, ContentsFinderListItem> listController;
    private readonly AddonController<AddonContentsFinder> addonController;

    private readonly Dictionary<uint, WondrousTailsNode> imageNodes = [];
    private readonly Dictionary<uint, List<uint>> wondrousTailsDuties = [];
    private TextNode? infoTextNode;
    private WondrousTailsNode? infoTailsNode;
    
    public WondrousTailsContentsFinderController(WondrousTails module) {
        this.module = module;

        listController = new NativeListController<AddonContentsFinder, ContentsFinderListItem> {
            AddonName = "ContentsFinder",
            GetPopulatorNode = GetPopulatorMethod,
            ShouldModifyElement = ShouldModifyElementMethod,
            UpdateElement = UpdateElementMethod,
            ResetElement = ResetElementMethod,
        };
        listController.Enable();
        
        addonController = new AddonController<AddonContentsFinder> {
            AddonName = "ContentsFinder",
            OnSetup = SetupContentsFinder,
            OnRefresh = RefreshContentsFinder,
            OnFinalize = FinalizeContentsFinder,
        };
        addonController.Enable();
    }

    public void Dispose() {
        listController.Dispose();
        addonController.Dispose();
    }

    private void SetupContentsFinder(AddonContentsFinder* addon) {
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
            String = new SeStringBuilder()
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

    private void RefreshContentsFinder(AddonContentsFinder* addon) {
        var shouldShow = listController.ModifiedIndexes.Count is not 0 && addon->SelectedRadioButton is not 0;

        infoTextNode?.ShowClickableCursor = shouldShow;
        infoTextNode?.IsVisible = shouldShow;
        infoTailsNode?.IsVisible = shouldShow && module.ModuleConfig.CloverIndicator;

        addon->UpdateCollisionNodeList(false);
    }

    private void FinalizeContentsFinder(AddonContentsFinder* _) {
        foreach (var imageNode in imageNodes.Values) {
            imageNode.Dispose();
        }
        imageNodes.Clear();

        infoTailsNode?.Dispose();
        infoTailsNode = null;

        infoTextNode?.Dispose();
        infoTextNode = null;
    }

    private AtkComponentListItemRenderer* GetPopulatorMethod(AddonContentsFinder* addonContentsFinder)
        => addonContentsFinder->DutyList->GetComponentItemRendererById(6);

    private bool ShouldModifyElementMethod(AddonContentsFinder* addonContentsFinder, ContentsFinderListItem listItem) {
        if (module.ModuleConfig is { CloverIndicator: false, ColorDutyFinderText: false }) return false;
        if (listItem.ContentType is not ContentsId.ContentsType.Regular) return false;

        return IsTailsTask(listItem.ContentsFinderCondition);
    }
    
    private void UpdateElementMethod(AddonContentsFinder* addonContentsFinder, ContentsFinderListItem listItem) {
        var contentFinderCondition = listItem.ContentsFinderCondition;

        // If clover is enabled
        if (module.ModuleConfig.CloverIndicator) {
			
            // And it is attached already, update it
            if (imageNodes.TryGetValue(listItem.NodeId, out var node)) {
                node.IsVisible = true;
                node.IsTaskAvailable = IsTaskAvailable(contentFinderCondition);
            }
			
            // else make it and attach it
            else {
                listItem.DutyNameTextNode->Width = (ushort) (listItem.DutyNameTextNode->Width - 24.0f);

                var newNode = new WondrousTailsNode {
                    Size = new Vector2(24.0f, 24.0f),
                    Position = new Vector2(listItem.DutyNameTextNode->X + listItem.DutyNameTextNode->Width, 0.0f),
                    IsTaskAvailable = IsTaskAvailable(contentFinderCondition),
                };
                newNode.AttachNode(listItem.DutyNameTextNode, NodePosition.AfterTarget);

                imageNodes.Add(listItem.NodeId, newNode);
            }
        }

        if (module.ModuleConfig.ColorDutyFinderText) {
            if (IsTaskAvailable(contentFinderCondition)) {
                listItem.DutyNameTextNode->TextColor = module.ModuleConfig.DutyFinderColor.ToByteColor();
            }
            else {
                listItem.DutyNameTextNode->TextColor = listItem.LevelTextNode->TextColor;
            }
        }
        else {
            listItem.DutyNameTextNode->TextColor = listItem.LevelTextNode->TextColor;
        }
    }
    
    private void ResetElementMethod(AddonContentsFinder* addonContentsFinder, ContentsFinderListItem listItem) {
        // Remove node
        if (imageNodes.TryGetValue(listItem.NodeId, out var node)) {
            listItem.DutyNameTextNode->Width = (ushort) (listItem.DutyNameTextNode->Width + 24.0f);

            imageNodes.Remove(listItem.NodeId);
            node.Dispose();
        }

        // Reset Color
        listItem.DutyNameTextNode->TextColor = listItem.LevelTextNode->TextColor;
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
