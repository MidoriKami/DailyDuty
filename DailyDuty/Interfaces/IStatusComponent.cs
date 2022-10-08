namespace DailyDuty.Interfaces;

public interface IStatusComponent : IDrawable
{
    IModule ParentModule { get; }
    ISelectable Selectable { get; }
}