using System.Collections.Generic;
using System.Linq;
using KamiLib.Utilities;

namespace DailyDuty.DataModels;

public readonly struct DutyFinderTabBar
{
    private readonly BaseNode addonBase;
    private readonly List<RadioButtonNode> radioButtons = new();

    public DutyFinderTabBar(BaseNode addonBase)
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

            radioButtons.Add(new RadioButtonNode(addonBase.GetComponentNode(id)));
        }
    }

    public int GetSelectedTabIndex()
    {
        return radioButtons.FindIndex(button => button.Selected);
    }
}