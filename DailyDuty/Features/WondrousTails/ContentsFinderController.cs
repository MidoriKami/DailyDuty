using System;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.Controllers;

namespace DailyDuty.Features.WondrousTails;

public unsafe class ContentsFinderController : IDisposable {
    private readonly WondrousTails module;
    private readonly NativeListController listController;

    public ContentsFinderController(WondrousTails module) {
        this.module = module;

        listController = new NativeListController("ContentsFinder") {
            GetPopulatorNode = GetPopulatorMethod,
            ShouldModifyElement = ShouldModifyElementMethod,
            UpdateElement = UpdateElementMethod,
            ResetElement = ResetElementMethod,
        };
        
        listController.Enable();
    }

    public void Dispose() {
        listController.Dispose();
    }

    private static AtkComponentListItemRenderer* GetPopulatorMethod(AtkUnitBase* addon) {
        var contentsFinder = (AddonContentsFinder*) addon;
        return contentsFinder->DutyList->GetItemRendererByNodeId(6);
    }

    private bool ShouldModifyElementMethod(AtkUnitBase* unitBase, ListItemData listItemData, AtkResNode** nodeList) {

        return false;
    }
    
    private void UpdateElementMethod(AtkUnitBase* unitBase, ListItemData listItemData, AtkResNode** nodeList) {

    }
    
    private static void ResetElementMethod(AtkUnitBase* unitBase, ListItemData listItemData, AtkResNode** nodeList) {

    }
}
