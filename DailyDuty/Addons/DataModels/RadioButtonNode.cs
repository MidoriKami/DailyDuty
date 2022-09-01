using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons.DataModels;

internal unsafe class RadioButtonNode
{
    private readonly AtkComponentRadioButton* radioButtonNode;

    public bool Selected => IsNodeSelected();

    public RadioButtonNode(AtkComponentRadioButton* radioButtonNode)
    {
        this.radioButtonNode = radioButtonNode;
    }

    private AtkResNode* GetPrimaryColorResNode()
    {
        var baseComponent = radioButtonNode->AtkComponentBase.UldManager;

        return Node.GetNodeByID<AtkResNode>(baseComponent, 5);
    }

    private bool IsNodeSelected()
    {
        var resNode = GetPrimaryColorResNode();

        var resNodeAddColor = resNode->AddRed;

        return resNodeAddColor > 16;
    }
}