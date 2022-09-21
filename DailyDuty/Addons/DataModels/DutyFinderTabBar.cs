using System.Collections.Generic;
using System.Linq;
using DailyDuty.Utilities;

namespace DailyDuty.Addons.DataModels;

internal struct DutyFinderTabBar
{
    private readonly BaseNode addonBase;
    public List<RadioButtonNode> RadioButtons = new();

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

            RadioButtons.Add(new RadioButtonNode(addonBase.GetComponentNode(id)));
        }
    }

    public int GetSelectedTabIndex()
    {
        return RadioButtons.FindIndex(button => button.Selected);
    }
}