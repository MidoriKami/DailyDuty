using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace DailyDuty.Addons.DataModels;

internal unsafe class DutyFinderTabBar
{
    private readonly AtkUnitBase* addonBase;
    public List<RadioButtonNode> RadioButtons = new();

    public DutyFinderTabBar(AtkUnitBase* addonBase)
    {
        this.addonBase = addonBase;

        PopulateRadioButtons();
    }

    private void PopulateRadioButtons()
    {
        if (addonBase == null) return;

        foreach (var i in Enumerable.Range(41, 10))
        {
            var id = (uint)i;

            var radioButton = (AtkComponentRadioButton*)addonBase->GetNodeById(id)->GetComponent();

            RadioButtons.Add(new RadioButtonNode(radioButton));
        }
    }

    public int GetSelectedTabIndex()
    {
        return RadioButtons.FindIndex(button => button.Selected);
    }
}