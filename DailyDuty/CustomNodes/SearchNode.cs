using System;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.CustomNodes;

public class SearchNode : SimpleComponentNode {
    private readonly HorizontalFlexNode searchContainerNode;

    public SearchNode() {
        searchContainerNode = new HorizontalFlexNode {
            Height = 28.0f,
            AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
        };
        searchContainerNode.AttachNode(this);

        searchContainerNode.AddNode(new TextInputNode {
            OnInputReceived = OnSearchBoxInputReceived,
            PlaceholderString = "Search . . .",
            AutoSelectAll = true,
        });
    }

    private void OnSearchBoxInputReceived(ReadOnlySeString obj)
        => OnSearchUpdated?.Invoke(obj);

    public required Action<ReadOnlySeString>? OnSearchUpdated { get; set; }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        searchContainerNode.Size = Size;
        searchContainerNode.RecalculateLayout();
    }
}
