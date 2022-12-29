
using KamiLib.Interfaces;

namespace DailyDuty.Interfaces;

public interface IConfigurationComponent : IDrawable
{
    IModule ParentModule { get; }
    ISelectable Selectable { get; }
}