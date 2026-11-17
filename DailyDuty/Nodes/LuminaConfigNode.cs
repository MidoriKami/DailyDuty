using System.Collections.Generic;
using DailyDuty.Utilities;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes;
using Lumina.Excel;

namespace DailyDuty.Nodes;

public class LuminaConfigNode<T> : ListItemNode<T>, IListItemNode where T : struct, IExcelRow<T> {

    private readonly CheckboxNode checkboxNode;

    public static float ItemHeight => 24.0f;

    public bool IsChecked => checkboxNode.IsChecked;

    public LuminaConfigNode() {
        DisableInteractions();

        checkboxNode = new CheckboxNode {
            TruncateText = true,
        };
        checkboxNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        checkboxNode.Size = Size;
    }

    protected override void SetNodeData(T itemData) {

        checkboxNode.OnChecked = null;

        switch (ReferenceData) {
            case HashSet<uint> enabledEntries:
                checkboxNode.IsChecked = enabledEntries.Contains(itemData.RowId);
                break;
        }

        checkboxNode.OnChecked = _ => OnStateChanged?.Invoke(this);
        checkboxNode.ItemTooltip = itemData.ItemId;
        checkboxNode.String = itemData.DisplayString;
    }
}
