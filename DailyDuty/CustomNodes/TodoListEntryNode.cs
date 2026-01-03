using DailyDuty.Classes;
using DailyDuty.Enums;
using DailyDuty.Features.TodoOverlay;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public class TodoListEntryNode : TextNode {
    public required ModuleBase Module { get; init; }
    public required TodoPanelConfig Config { get; init; }

    public TodoListEntryNode() {
        AddEvent(AtkEventType.MouseClick, OnMouseClick);
    }

    private void OnMouseClick()
        => PayloadController.InvokePayload(Module.Tooltip?.ClickAction ?? PayloadId.Unset);

    public void Update() {
        TextColor = Config.TextColor;
        TextOutlineColor = Config.OutlineColor;

        if (Module.Tooltip is not null) {
            
            // The tooltip has been changed
            if (TextTooltip != string.Empty && TextTooltip != Module.Tooltip.TooltipText) {
                TextTooltip = Module.Tooltip.TooltipText;
            }

            ShowClickableCursor = Module.Tooltip.ClickAction is not PayloadId.Unset;
        }
        else {
            ShowClickableCursor = false;
        }
    }
}
