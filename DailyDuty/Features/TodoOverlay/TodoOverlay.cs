using DailyDuty.Classes;
using DailyDuty.Enums;
using KamiToolKit;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Features.TodoOverlay;

public class TodoOverlay : Module<TodoOverlayConfig, DataBase> {
    public override ModuleInfo ModuleInfo => new() {
        DisplayName = "Todo List Overlay",
        FileName = "TodoList",
        Type = ModuleType.GeneralFeatures,
        ChangeLog = [
            new ChangeLogInfo(1, "Initial Reimplementation"),
        ],
        Tags = [ "Overlay" , "Todo" ],
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
