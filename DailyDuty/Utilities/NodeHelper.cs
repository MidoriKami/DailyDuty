using System;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Utilities;

internal unsafe class BaseNode
{
    private readonly AtkUnitBase* node;

    public BaseNode(string addon)
    {
        node = (AtkUnitBase*) Service.GameGui.GetAddonByName(addon, 1);
    }

    public BaseNode Print()
    {
        Chat.Print("AtkUnitBase", new IntPtr(node));

        return this;
    }

    public ResNode GetRootNode()
    {
        if (node == null) return new ResNode(null);

        return new ResNode(node->RootNode);
    }

    public ResNode GetResNode(uint id)
    {
        if (node == null) return new ResNode(null);

        var targetNode = node->GetNodeById(id);

        return new ResNode(targetNode);
    }

    public ComponentNode GetComponentNode(uint id)
    {
        if (node == null) return new ComponentNode(null);

        var targetNode = (AtkComponentNode*) node->GetNodeById(id);

        return new ComponentNode(targetNode);
    }

    public ComponentNode GetNestedNode(params uint[] idList)
    {
        uint index = 0;

        ComponentNode startingNode;

        do
        {
            startingNode = GetComponentNode(idList[index]);

        } while (index++ < idList.Length);
        
        return startingNode;
    }
}

internal unsafe class ResNode
{
    private readonly AtkResNode* node;

    public ResNode(AtkResNode* node)
    {
        this.node = node;
    }

    public ResNode Print()
    {
        Chat.Print("AtkResNode", new IntPtr(node));
    
        return this;
    }
}

internal unsafe class ComponentNode
{
    private readonly AtkComponentNode* node;
    private readonly AtkComponentBase* componentBase;

    public ComponentNode(AtkComponentNode* node)
    {
        this.node = node;

        componentBase = node == null ? null : node->Component;
    }

    public ComponentNode Print()
    {
        Chat.Print("AtkComponentNode", new IntPtr(node));
    
        return this;
    }

    public ComponentNode GetComponentNode(uint id)
    {
        if (componentBase == null) return new ComponentNode(null);

        var targetNode = Node.GetNodeByID<AtkComponentNode>(componentBase->UldManager, id);

        return new ComponentNode(targetNode);
    }

    public T* GetNode<T>(uint id) where T : unmanaged
    {
        if(componentBase == null) return null;

        return Node.GetNodeByID<T>(componentBase->UldManager, id);
    }

    public AtkComponentNode* GetPointer()
    {
        return node;
    }
}

internal static unsafe class Node
{
    public static T* GetNodeByID<T>(AtkUldManager uldManager, uint nodeId) where T : unmanaged 
    {
        for (var i = 0; i < uldManager.NodeListCount; i++) 
        {
            var currentNode = uldManager.NodeList[i];

            if (currentNode->NodeID != nodeId) continue;

            return (T*) currentNode;
        }

        return null;
    }
}