using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System.Numerics;

namespace DailyDuty.Utilities;

internal static unsafe class Node
{
    public static T* GetNodeByID<T>(AtkComponentBase componentBase, uint nodeID, NodeType? type = null) where T : unmanaged
    {
        return GetNodeByID<T>(componentBase.UldManager, nodeID, type);
    }

    public static T* GetNodeByID<T>(AtkUldManager uldManager, uint nodeId, NodeType? type = null) where T : unmanaged 
    {
        for (var i = 0; i < uldManager.NodeListCount; i++) 
        {
            var n = uldManager.NodeList[i];
            if (n->NodeID != nodeId || type != null && n->Type != type.Value) continue;
            return (T*)n;
        }
        return null;
    }

    public static T* GetNodeByID<T>(AtkComponentNode* baseNode, uint nodeId, NodeType? type = null) where T : unmanaged
    {
        if (baseNode == null) return null;

        var component = baseNode->Component;
        if (component == null) return null;

        var uldManager = component->UldManager;

        return GetNodeByID<T>(uldManager, nodeId, type);
    }

    public static AtkComponentNode* GetComponentNode(AtkUnitBase* baseNode, uint id)
    {
        if (baseNode != null)
        {
            return (AtkComponentNode*) baseNode->GetNodeById(id);
        }

        return null;
    }

    public static void MakeCloverNode(AtkComponentNode* rootNode, AtkResNode* beforeNode, uint newNodeID, Vector2 textureCoordinates, Vector2 positionOffset = default)
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

        short xPosition = (short)(290 + positionOffset.X);
        short yPosition = (short)(2 + positionOffset.Y);

        customNode->AtkResNode.SetPositionShort(xPosition, yPosition);

        var prev = beforeNode->PrevSiblingNode;
        customNode->AtkResNode.ParentNode = beforeNode->ParentNode;

        beforeNode->PrevSiblingNode = (AtkResNode*) customNode;
        prev->NextSiblingNode = (AtkResNode*) customNode;

        customNode->AtkResNode.PrevSiblingNode = prev;
        customNode->AtkResNode.NextSiblingNode = beforeNode;

        rootNode->Component->UldManager.UpdateDrawNodeList();
    }
}