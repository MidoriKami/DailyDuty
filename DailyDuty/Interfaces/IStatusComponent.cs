using KamiLib.Interfaces;

namespace DailyDuty.Interfaces;

public interface IStatusComponent
{
    IModule ParentModule { get; }
    ISelectable Selectable { get; }

    void DrawStatus();
}