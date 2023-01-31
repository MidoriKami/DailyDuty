using DailyDuty.DataModels;
using DailyDuty.Localization;
using KamiLib.Drawing;
using KamiLib.Interfaces;

namespace DailyDuty.Interfaces;

public interface ITodoComponent
{
    IModule ParentModule { get; }
    CompletionType CompletionType { get; }
    bool HasLongLabel { get; }
    string GetShortTaskLabel();
    string GetLongTaskLabel();

    TodoConfigurationRow GetTodoConfigurationRow() => new(this);
}

public class TodoConfigurationRow : IInfoBoxTableConfigurationRow
{
    private readonly ITodoComponent todoComponent;
    
    public TodoConfigurationRow(ITodoComponent component)
    {
        todoComponent = component;
    }

    public void GetConfigurationRow(InfoBoxTable owner)
    {
        if (todoComponent.HasLongLabel)
        {
            owner
                .BeginRow()
                .AddConfigCheckbox(todoComponent.ParentModule.Name.GetTranslatedString(), todoComponent.ParentModule.GenericSettings.TodoTaskEnabled)
                .AddConfigCheckbox(Strings.Todo_UseLongLabel, todoComponent.ParentModule.GenericSettings.TodoUseLongLabel, additionalID: todoComponent.ParentModule.Name.GetTranslatedString())
                .EndRow();
        }
        else
        {
            owner
                .BeginRow()
                .AddConfigCheckbox(todoComponent.ParentModule.Name.GetTranslatedString(), todoComponent.ParentModule.GenericSettings.TodoTaskEnabled)
                .EndRow();
        }
    }
}