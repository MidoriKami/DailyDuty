using DailyDuty.Classes;
using DailyDuty.Classes.Nodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;

namespace DailyDuty.Windows;

public class ModuleConfigWindow<T> : NativeAddon where T : ModuleBase {
    public required T Module { get; init; }

    private ConfigNodeBase? configNode;
    
    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        configNode = Module.GetConfigNode();
        configNode.Position = ContentStartPosition;
        configNode.Size = ContentSize;
        configNode.AttachNode(this);
    }
}
