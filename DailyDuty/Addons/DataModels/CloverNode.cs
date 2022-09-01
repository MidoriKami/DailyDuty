using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons.DataModels;

internal enum CloverState
{
    Hidden,
    Golden,
    Dark
}

internal unsafe class CloverNode
{
    public AtkImageNode* GoldenCloverNode;
    public AtkImageNode* EmptyCloverNode;

    public CloverNode(AtkImageNode* golden, AtkImageNode* dark)
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