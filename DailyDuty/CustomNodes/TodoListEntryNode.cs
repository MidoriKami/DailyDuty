using DailyDuty.Classes;
using DailyDuty.Features.TodoOverlay;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public class TodoListEntryNode : TextNode {
    public required ModuleBase Module { get; init; }
    public required TodoPanelConfig Config { get; init; }

    public void Update() {
        TextColor = Config.TextColor;
        TextOutlineColor = Config.OutlineColor;
    }
}
