namespace DailyDuty.Interfaces;

internal interface IStatusComponent : IDrawable
{
    IModule ParentModule { get; }
    ISelectable Selectable { get; }

}