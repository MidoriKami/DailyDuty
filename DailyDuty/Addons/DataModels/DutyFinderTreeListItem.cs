using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Numerics;
using System.Text.RegularExpressions;
using DailyDuty.Utilities;

namespace DailyDuty.Addons.DataModels;

internal unsafe class DutyFinderTreeListItem
{
    private readonly AtkComponentNode* treeListItem;
    public readonly CloverNode CloverNode;

    public string Label => GetLabel();
    public string FilteredLabel => GetFilteredLabel();

    public DutyFinderTreeListItem(AtkComponentNode* treeListItem)
    {
        this.treeListItem = treeListItem;
        CloverNode = GetCloverNodes();
    }

    private string GetLabel()
    {
        return GetLabelNode()->NodeText.ToString().ToLower();
    }

    private string GetFilteredLabel()
    {
        return Regex.Replace(Label, "[^\\p{L}\\p{N}]", "");
    }

    private CloverNode GetCloverNodes()
    {
        var cloverNode = Node.GetNodeByID<AtkImageNode>(treeListItem, 29, NodeType.Image);
        var emptyCloverNode = Node.GetNodeByID<AtkImageNode>(treeListItem, 30, NodeType.Image);

        return new CloverNode(cloverNode, emptyCloverNode);
    }

    private AtkTextNode* GetLabelNode()
    {
        return Node.GetNodeByID<AtkTextNode>(treeListItem, 5);
    }

    public void MakeCloverNodes()
    {
        if (CloverNode.EmptyCloverNode == null && CloverNode.GoldenCloverNode == null)
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

    public void SetTextColor(Vector4 color)
    {
        var textNode = GetLabelNode();
        if (textNode == null) return;

        textNode->TextColor.R = (byte)(color.X * 255);
        textNode->TextColor.G = (byte)(color.Y * 255);
        textNode->TextColor.B = (byte)(color.Z * 255);
        textNode->TextColor.A = (byte)(color.W * 255);
    }

    public ByteColor GetTextColor()
    {
        var textNode = GetLabelNode();
        if (textNode == null) return new ByteColor();

        return textNode->TextColor;
    }

    public void SetTextColor(ByteColor color)
    {
        var textNode = GetLabelNode();
        if (textNode == null) return;

        textNode->TextColor = color;
    }
}