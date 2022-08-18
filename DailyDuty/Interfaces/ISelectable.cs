using DailyDuty.Modules.Enums;

namespace DailyDuty.Interfaces;

internal interface ISelectable
{
    ModuleName OwnerModuleName { get; }

    IDrawable Contents { get; }

    void DrawLabel();
}