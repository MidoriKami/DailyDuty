
using KamiLib.Interfaces;

namespace DailyDuty.Interfaces;

public interface IConfigurationComponent
{
    IModule ParentModule { get; }
    ISelectable Selectable { get; }

    void DrawConfiguration();
}