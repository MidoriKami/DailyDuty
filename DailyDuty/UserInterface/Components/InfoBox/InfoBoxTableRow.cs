using System;

namespace DailyDuty.UserInterface.Components.InfoBox;

public class InfoBoxTableRow : DrawList<InfoBoxTableRow>
{
    private readonly InfoBoxTable owner;

    public Action? FirstColumn => DrawActions.Count > 0 ? DrawActions[0] : null;
    public Action? SecondColumn => DrawActions.Count > 1 ? DrawActions[1] : null;

    public InfoBoxTableRow(InfoBoxTable owner)
    {
        this.owner = owner;
        DrawListOwner = this;
    }

    public InfoBoxTable EndRow()
    {
        owner.AddRow(this);

        return owner;
    }
}