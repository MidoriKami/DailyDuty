
namespace DailyDuty.Interfaces;

internal interface IConfigurationComponent : IDrawable
{
    IModule ParentModule { get; }
    ISelectable Selectable { get; }
}