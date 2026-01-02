using DailyDuty.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.CustomNodes;

public class TodoListEntryNode : TextNode {
    public required ModuleBase Module { get; init; }
}
