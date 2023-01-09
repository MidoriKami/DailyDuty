using System.Text.RegularExpressions;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Atk;
using Vector2 = System.Numerics.Vector2;
using Vector4 = System.Numerics.Vector4;

namespace DailyDuty.DataModels;

public unsafe partial struct DutyFinderTreeListItem
{
    private readonly ComponentNode treeListItem;
    public CloverNode CloverNode;

    public string Label => GetLabel();
    public string FilteredLabel => GetFilteredLabel();

    public DutyFinderTreeListItem(ComponentNode treeListItem)
    {
        this.treeListItem = treeListItem;

        var cloverNode = treeListItem.GetNode<AtkImageNode>(29);
        var emptyCloverNode = treeListItem.GetNode<AtkImageNode>(30);

        CloverNode =  new CloverNode(cloverNode, emptyCloverNode);
    }

    private string GetLabel()
    {
        var labelNode = GetLabelNode();

        return labelNode == null ? string.Empty : labelNode->NodeText.ToString().ToLower();
    }

    private string GetFilteredLabel()
    {
        return AlphanumericRegex().Replace(Label, "");
    }

    private AtkTextNode* GetLabelNode()
    {
        return treeListItem.GetNode<AtkTextNode>(5);
    }

    public void ShiftImageNodes()
    {
        var moogleNode = treeListItem.GetNode<AtkResNode>(6);
        moogleNode->X = 285;

        var levelSync = treeListItem.GetNode<AtkResNode>(10);
        levelSync->X = 305;
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

            MakeCloverNode(treeListItem.GetPointer(), textNode, 29, clover);
            MakeCloverNode(treeListItem.GetPointer(), textNode, 30, empty, new Vector2(1, 0));
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

    private static void MakeCloverNode(AtkComponentNode* rootNode, AtkResNode* beforeNode, uint newNodeID, Vector2 textureCoordinates, Vector2 positionOffset = default)
    {
        var customNode = IMemorySpace.GetUISpace()->Create<AtkImageNode>();
        customNode->AtkResNode.Type = NodeType.Image;
        customNode->AtkResNode.NodeID = newNodeID;
        customNode->AtkResNode.Flags = 8243;
        customNode->AtkResNode.DrawFlags = 0;
        customNode->WrapMode = 1;
        customNode->Flags = 0;

        var partsList = (AtkUldPartsList*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldPartsList), 8);
        if (partsList == null)
        {
            customNode->AtkResNode.Destroy(true);
            return;
        }

        partsList->Id = 0;
        partsList->PartCount = 1;

        var part = (AtkUldPart*) IMemorySpace.GetUISpace()->Malloc((ulong) sizeof(AtkUldPart), 8);
        if (part == null) {
            IMemorySpace.Free(partsList, (ulong)sizeof(AtkUldPartsList));
            customNode->AtkResNode.Destroy(true);
            return;
        }

        part->U = (ushort)textureCoordinates.X;
        part->V = (ushort)textureCoordinates.Y;
        part->Width = 20;
        part->Height = 20;
        
        partsList->Parts = part;

        var asset = (AtkUldAsset*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldAsset), 8);
        if (asset == null) {
            IMemorySpace.Free(part, (ulong)sizeof(AtkUldPart));
            IMemorySpace.Free(partsList, (ulong)sizeof(AtkUldPartsList));
            customNode->AtkResNode.Destroy(true);
            return;
        }

        asset->Id = 0;
        asset->AtkTexture.Ctor();
        part->UldAsset = asset;
        customNode->PartsList = partsList;

        customNode->LoadTexture("ui/uld/WeeklyBingo.tex");

        customNode->AtkResNode.ToggleVisibility(true);

        customNode->AtkResNode.SetWidth(20);
        customNode->AtkResNode.SetHeight(20);

        var xPosition = (short)(325 + positionOffset.X);
        var yPosition = (short)(2 + positionOffset.Y);

        customNode->AtkResNode.SetPositionShort(xPosition, yPosition);

        var prev = beforeNode->PrevSiblingNode;
        customNode->AtkResNode.ParentNode = beforeNode->ParentNode;

        beforeNode->PrevSiblingNode = (AtkResNode*) customNode;
        prev->NextSiblingNode = (AtkResNode*) customNode;

        customNode->AtkResNode.PrevSiblingNode = prev;
        customNode->AtkResNode.NextSiblingNode = beforeNode;

        rootNode->Component->UldManager.UpdateDrawNodeList();
    }

    [GeneratedRegex("[^\\p{L}\\p{N}]")]
    private static partial Regex AlphanumericRegex();
}