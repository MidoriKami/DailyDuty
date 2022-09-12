using System.Collections.Generic;
using DailyDuty.Interfaces;

namespace DailyDuty.UserInterface.Components.InfoBox;

public class InfoBoxList : DrawList<InfoBoxList>
{
    private readonly InfoBox owner;

    public InfoBoxList(InfoBox owner)
    {
        this.owner = owner;
        DrawListOwner = this;
    }

    public InfoBox EndList()
    {
        foreach (var row in DrawActions)
        {
            owner.AddAction(row);
        }

        return owner;
    }

    public InfoBoxList AddRows(IEnumerable<IInfoBoxListConfigurationRow> rows)
    {
        foreach (var row in rows)
        {
            row.GetConfigurationRow(this);
        }

        return this;
    }
}