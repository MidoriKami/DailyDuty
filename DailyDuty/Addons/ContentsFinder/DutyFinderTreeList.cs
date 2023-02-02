using System.Collections.Generic;
using System.Linq;
using DailyDuty.DataModels;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using KamiLib.Atk;

namespace DailyDuty.Addons.ContentsFinder;

public struct DutyFinderTreeList
{
    private readonly ComponentNode treeNodeBase;
    public List<DutyFinderTreeListItem> Items = new();

    public DutyFinderTreeList(ComponentNode treeListNode)
    {
        treeNodeBase = treeListNode;

        PopulateListItems();
    }

    private void PopulateListItems()
    {
        foreach (var i in Enumerable.Range(61001, 15).Append(6))
        {
            var id = (uint)i;

            var listItemNode = treeNodeBase.GetComponentNode(id);
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
            item.ShiftImageNodes();
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
