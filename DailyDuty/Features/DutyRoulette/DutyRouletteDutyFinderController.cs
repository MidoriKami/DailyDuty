using System;
using System.Numerics;
using DailyDuty.Classes;
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
    private readonly NativeListController<AddonContentsFinder, ContentsFinderListItem> listController;
    private readonly AddonController<AddonContentsFinder> addonController;
    private TextNode? infoTextNode;

    public DutyRouletteDutyFinderController(DutyRoulette module) {
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
        var targetResNode = addon->GetNodeById(56);
        if (targetResNode is null) return;

        infoTextNode = new TextNode {
            Position = new Vector2(16.0f, targetResNode->GetYFloat() + 2.0f),
            Size = new Vector2(250.0f, 18.0f),
            AlignmentType = AlignmentType.Center,
            String = new SeStringBuilder().PushColorRgba(module.ModuleConfig.IncompleteColor)
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
    }

    private void RefreshContentsFinder(AddonContentsFinder* addon) {
        var shouldShow = listController.ModifiedIndexes.Count is not 0 && addon->SelectedRadioButton is 0;

        infoTextNode?.ShowClickableCursor = shouldShow;
        infoTextNode?.IsVisible = shouldShow;
        infoTextNode?.String = new SeStringBuilder().PushColorRgba(module.ModuleConfig.IncompleteColor)
            .Append("Incomplete Task")
            .PopColor()
            .Append("        ")
            .PushColorRgba(module.ModuleConfig.CompleteColor)
            .Append("Complete Task")
            .PopColor()
            .ToReadOnlySeString();

        if (!shouldShow) {
            infoTextNode?.RemoveNodeFlags(NodeFlags.RespondToMouse, NodeFlags.EmitsEvents);
        }
        else {
            infoTextNode?.AddNodeFlags(NodeFlags.RespondToMouse, NodeFlags.EmitsEvents);
        }

        addon->UpdateCollisionNodeList(false);
    }
    
    private void FinalizeContentsFinder(AddonContentsFinder* _) {
        infoTextNode?.Dispose();
        infoTextNode = null;
    }

    private static AtkComponentListItemRenderer* GetPopulatorMethod(AddonContentsFinder* addonContentsFinder)
        => addonContentsFinder->DutyList->GetComponentItemRendererById(6);

    private bool ShouldModifyElementMethod(AddonContentsFinder* addonContentsFinder, ContentsFinderListItem listItem) {
        if (!module.ModuleConfig.ColorContentFinder) return false;
        if (listItem.ContentType is not ContentsId.ContentsType.Roulette) return false;

        return module.ModuleConfig.TrackedRoulettes.Contains(listItem.GetContentId().Id);
    }
    
    private void UpdateElementMethod(AddonContentsFinder* addonContentsFinder, ContentsFinderListItem listItem) {
        if (listItem.ContentType is not  ContentsId.ContentsType.Roulette) return;

        var rouletteInfo = Services.DataManager
            .GetExcelSheet<ContentRoulette>()
            .GetRow(listItem.GetContentId().Id);
        
        if (InstanceContent.Instance()->IsRouletteComplete((byte)rouletteInfo.RowId)) {
            listItem.DutyNameTextNode->TextColor = module.ModuleConfig.CompleteColor.ToByteColor();
        }
        else {
            listItem.DutyNameTextNode->TextColor = module.ModuleConfig.IncompleteColor.ToByteColor();
        }
    }

    private static void ResetElementMethod(AddonContentsFinder* addonContentsFinder, ContentsFinderListItem listItemInfo)
        => listItemInfo.DutyNameTextNode->TextColor = listItemInfo.LevelTextNode->TextColor;
}
