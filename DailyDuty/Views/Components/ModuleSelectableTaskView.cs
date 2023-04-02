using System.Reflection;
using DailyDuty.Abstracts;
using DailyDuty.Interfaces;
using DailyDuty.Models;
using KamiLib.Interfaces;
using Action = System.Action;

namespace DailyDuty.Views.Components;

public static class ModuleSelectableTaskView
{
    public static void DrawConfig(FieldInfo? field, ModuleConfigBase moduleConfig, Action saveAction)
    {
        if (field is null) return;
        
        if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(LuminaTaskConfigList<>))
        {
            var data = (IConfigDrawable) field.GetValue(moduleConfig)!;
            
            data.Draw(saveAction);
        }
    }
    
    public static void DrawData(FieldInfo? field, ModuleDataBase moduleData)
    {
        if (field is null) return;

        if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(LuminaTaskDataList<>))
        {
            var data = (IDrawable) field.GetValue(moduleData)!;
            
            data.Draw();
        }
    }
}
