using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Features.TodoOverlay;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public class TodoListEntryNode : TextNode {
    public required LoadedModule LoadedModule { get; init; }
    public required ModuleBase Module { get; init; }
    public required TodoPanelConfig Config { get; init; }

    public TodoListEntryNode()
        => AddEvent(AtkEventType.MouseClick, OnMouseClick);

    private void OnMouseClick()
        => PayloadController.InvokePayload(Module.Tooltip?.ClickAction ?? PayloadId.Unset);
}
