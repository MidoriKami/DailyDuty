using DailyDuty.Classes;
using DailyDuty.Enums;
using KamiToolKit;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Features.TimersOverlay;

public class TimersOverlay : Module<TimersOverlayConfig, DataBase> {

    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Timers Overlay",
        FileName = "Timers",
        Type = ModuleType.GeneralFeatures,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Re-Implementation"),
        ],
        Tags = [ "Countdown", "Reset" ],
    };
    
    protected override void OnEnable() {
    }

    protected override void OnDisable() {
    }

    public override NodeBase GetStatusDisplayNode() {
        return new ResNode();
    }
    
    public override CompletionStatus? GetModuleStatus() => null;
    public override ReadOnlySeString? GetStatusMessage() => null;
}
