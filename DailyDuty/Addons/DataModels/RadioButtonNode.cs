using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons.DataModels;

internal unsafe struct RadioButtonNode
{
    private readonly AtkComponentRadioButton* radioButtonNode;

    public bool Selected => IsNodeSelected();

    public RadioButtonNode(AtkComponentRadioButton* radioButtonNode)
    {
        this.radioButtonNode = radioButtonNode;
    }

    private AtkResNode* GetPrimaryColorResNode()
    {
        if (radioButtonNode == null) return null;

        var componentBase = radioButtonNode->AtkComponentBase;
        var uldManager = componentBase.UldManager;

        return Node.GetNodeByID<AtkResNode>(uldManager, 5);
    }

    private bool IsNodeSelected()
    {
        var resNode = GetPrimaryColorResNode();
        if (resNode == null) return false;

        var resNodeAddColor = resNode->AddRed;

        return resNodeAddColor > 16;
    }
}