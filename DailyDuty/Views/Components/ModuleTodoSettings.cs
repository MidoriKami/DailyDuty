using System;
using System.Linq;
using System.Reflection;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.System.Localization;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public class ModuleTodoSettings
{
    public static void Draw(ModuleTodoOptions config, Action saveAction)
    {
        ImGui.Text(Strings.TodoDisplayOptions);
        ImGui.Separator();
        ImGuiHelpers.ScaledIndent(15.0f);
        
        var fields = config
            .GetType()
            .GetFields(); 
        
        var configOptions = fields
            .Where(field => field.GetCustomAttribute(typeof(ConfigOption)) is not null)
            .Select(field => (field, (ConfigOption) field.GetCustomAttribute(typeof(ConfigOption))!))
            .ToList();
        
        GenericConfigView.Draw(configOptions, config, () => {
            saveAction.Invoke();
            config.StyleChanged = true;
        }, Strings.TodoDisplayConfiguration);


        // if (ImGui.Checkbox(Strings.Enable, ref config.Enabled))
        // {
        //     saveAction.Invoke();
        //     config.StyleChanged = true;
        // }
        //
        // DrawCustomLabelOption(config, saveAction);
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
    
    private static void DrawCustomLabelOption(ModuleTodoOptions config, Action saveAction)
    {
        if (ImGui.Checkbox("Use Custom Label", ref config.UseCustomTodoLabel))
        {
            saveAction.Invoke();
        }

        if (config.UseCustomTodoLabel)
        {
            ImGui.PushFont(DailyDutyPlugin.System.FontController.Axis12.ImFont);
            ImGui.InputTextWithHint("##CustomResetMessage", Strings.TaskLabel, ref config.CustomTodoLabel, 2048);
            ImGui.PopFont();
            if (ImGui.IsItemDeactivatedAfterEdit()) saveAction.Invoke();
        }
    }
}