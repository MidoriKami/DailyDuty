using System.Collections.Generic;
using System.Linq;
using DailyDuty.Data;
using DailyDuty.Utilities;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes;
using KamiToolKit.UI.Nodes.ComponentNode;
using Lumina.Excel;

namespace DailyDuty.Nodes.Config;

public class LuminaConfigNode<T> : ListItemNode<T>, IListItemNode where T : struct, IExcelRow<T> {

    private readonly CheckboxNode checkboxNode;

    public static float ItemHeight => 24.0f;

    public bool IsChecked => checkboxNode.IsChecked;

    public LuminaConfigNode() {
        DisableInteractions();

        checkboxNode = new CheckboxNode {
            TruncateText = true,
            OnChecked = _ => OnStateChanged?.Invoke(this),
        };
        checkboxNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        checkboxNode.Size = Size;
    }

    protected override void SetNodeData(T itemData) {

        switch (ReferenceData) {
            case HashSet<uint> enabledEntries:
                checkboxNode.SetValueSilent(enabledEntries.Contains(itemData.RowId));
                break;
            case List<RaidEntry> trackedRaids:
                checkboxNode.SetValueSilent(trackedRaids.Any(e => e.Id == itemData.RowId));
                break;
        }

        checkboxNode.ItemTooltip = itemData.ItemId;
        checkboxNode.String = itemData.DisplayString;
    }
}
