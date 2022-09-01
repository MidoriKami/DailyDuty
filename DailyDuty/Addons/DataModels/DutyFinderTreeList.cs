using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons.DataModels;

internal unsafe class DutyFinderTreeList
{
    private readonly AtkComponentNode* treeNodeBase;

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
            item.CloverNodes.SetVisibility(CloverState.Hidden);
        }
    }

    public void MakeCloverNodes()
    {
        foreach (var item in Items)
        {
            item.MakeCloverNodes();
        }
    }
}

internal unsafe class DutyFinderTreeListItem
{
    private readonly AtkComponentNode* treeListItem;
    public readonly CloverNodes CloverNodes;

    public string Label => GetLabel();
    public string FilteredLabel => GetFilteredLabel();

    public DutyFinderTreeListItem(AtkComponentNode* treeListItem)
    {
        this.treeListItem = treeListItem;
        CloverNodes = GetCloverNodes();
    }

    private string GetLabel()
    {
        return GetLabelNode()->NodeText.ToString().ToLower();
    }

    private string GetFilteredLabel()
    {
        return Regex.Replace(Label, "[^\\p{L}\\p{N}]", "");
    }

    private CloverNodes GetCloverNodes()
    {
        var cloverNode = Node.GetNodeByID<AtkImageNode>(treeListItem, 29, NodeType.Image);
        var emptyCloverNode = Node.GetNodeByID<AtkImageNode>(treeListItem, 30, NodeType.Image);

        return new CloverNodes(cloverNode, emptyCloverNode);
    }

    private AtkTextNode* GetLabelNode()
    {
        return Node.GetNodeByID<AtkTextNode>(treeListItem, 5);
    }

    public void MakeCloverNodes()
    {
        if (CloverNodes.EmptyCloverNode == null && CloverNodes.GoldenCloverNode == null)
        {
            // Place new node before the text node
            var textNode = (AtkResNode*)GetLabelNode();

            // Coordinates of clover node
            var clover = new Vector2(97, 65);

            // Coordinates of missing clover node
            var empty = new Vector2(75, 63);

            Node.MakeCloverNode(treeListItem, textNode, 29, clover);
            Node.MakeCloverNode(treeListItem, textNode, 30, empty, new Vector2(1, 0));
        }
    }
}

internal enum CloverState
{
    Hidden,
    Golden,
    Dark
}

internal unsafe class CloverNodes
{
    public AtkImageNode* GoldenCloverNode;
    public AtkImageNode* EmptyCloverNode;

    public CloverNodes(AtkImageNode* golden, AtkImageNode* dark)
    {
        GoldenCloverNode = golden;
        EmptyCloverNode = dark;
    }

    public void SetVisibility(CloverState state)
    {
        if (GoldenCloverNode == null || EmptyCloverNode == null) return;

        switch (state)
        {
            case CloverState.Hidden:
                GoldenCloverNode->AtkResNode.ToggleVisibility(false);
                EmptyCloverNode->AtkResNode.ToggleVisibility(false);
                break;

            case CloverState.Golden:
                GoldenCloverNode->AtkResNode.ToggleVisibility(true);
                EmptyCloverNode->AtkResNode.ToggleVisibility(false);
                break;

            case CloverState.Dark:
                GoldenCloverNode->AtkResNode.ToggleVisibility(false);
                EmptyCloverNode->AtkResNode.ToggleVisibility(true);
                break;
        }
    }
}