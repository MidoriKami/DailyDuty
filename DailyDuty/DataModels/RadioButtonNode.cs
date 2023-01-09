using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Atk;

namespace DailyDuty.DataModels;

public readonly unsafe struct RadioButtonNode
{
    private readonly ComponentNode radioButtonNode;

    public bool Selected => IsNodeSelected();

    public RadioButtonNode(ComponentNode radioButtonNode)
    {
        this.radioButtonNode = radioButtonNode;
    }

    private AtkResNode* GetPrimaryColorResNode()
    {
        return radioButtonNode.GetNode<AtkResNode>(5);
    }

    private bool IsNodeSelected()
    {
        var resNode = GetPrimaryColorResNode();
        if (resNode == null) return false;

        var resNodeAddColor = resNode->AddRed;

        return resNodeAddColor > 16;
    }
}