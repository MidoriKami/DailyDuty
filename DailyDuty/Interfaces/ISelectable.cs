using DailyDuty.Configuration.Components;

namespace DailyDuty.Interfaces;

internal interface ISelectable
{
    ModuleName OwnerModuleName { get; }
    IDrawable Contents { get; }
    IModule ParentModule { get; }

    void DrawLabel();
}