using DailyDuty.Configuration.Components;

namespace DailyDuty.Interfaces;

public interface ISelectable
{
    ModuleName OwnerModuleName { get; }
    IDrawable Contents { get; }
    IModule ParentModule { get; }

    void DrawLabel();
}