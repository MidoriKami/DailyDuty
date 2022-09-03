using System.Collections.Generic;
using System.Linq;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons.DataModels;

internal unsafe struct DutyFinderTreeList
{
    private AtkComponentNode* treeNodeBase;
    public List<DutyFinderTreeListItem> Items = new();

    public DutyFinderTreeList(AtkComponentNode* treeListNode)
    {
        treeNodeBase = treeListNode;

        PopulateListItems();
    }

    private void PopulateListItems()
    {
        foreach (var i in Enumerable.Range(61001, 15).Append(6))
        {
            var id = (uint)i;

            if (treeNodeBase == null) continue;

            var listItemNode = Node.GetNodeByID<AtkComponentNode>(treeNodeBase, id);
            Items.Add(new DutyFinderTreeListItem(listItemNode));
        }
    }

    public void HideCloverNodes()
    {
        foreach (var item in Items)
        {
            item.CloverNode.SetVisibility(CloverState.Hidden);
        }
    }

    public void MakeCloverNodes()
    {
        foreach (var item in Items)
        {
            item.MakeCloverNodes();
        }
    }

    public void SetColorAll(ByteColor color)
    {
        foreach (var item in Items)
        {
            item.SetTextColor(color);
        }
    }
}
