using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons.DataModels;

internal unsafe struct RadioButtonNode
{
    private readonly ComponentNode radioButtonNode;

    public bool Selected => IsNodeSelected();

    public RadioButtonNode(ComponentNode radioButtonNode)
    {
        this.radioButtonNode = radioButtonNode;
    }

    private AtkResNode* GetPrimaryColorResNode()
    {
        return radioButtonNode.GetResNode(5);
    }

    private bool IsNodeSelected()
    {
        var resNode = GetPrimaryColorResNode();
        if (resNode == null) return false;

        var resNodeAddColor = resNode->AddRed;

        return resNodeAddColor > 16;
    }
}