using System;
using System.Globalization;
using System.Reflection;
using DailyDuty.System.Localization;
using ImGuiNET;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.Attributes;

public class ModuleResetTime : LocalDateTimeDisplay
{
    public ModuleResetTime(string category, int group) : base(null, category, group) { }

    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null)
    {
        var dateTime = GetValue<DateTime>(obj, field);
        
        if (dateTime != DateTime.MaxValue)
        {
            if (ImGui.BeginTable("##ResetTable", 2, ImGuiTableFlags.SizingStretchSame))
            {
                ImGui.TableNextColumn();
                ImGui.Text(Strings.NextReset);
            
                ImGui.TableNextColumn();
                ImGui.Text(dateTime.ToLocalTime().ToString(CultureInfo.CurrentCulture));

                ImGui.TableNextColumn();
                ImGui.Text(Strings.RemainingTime);
            
                ImGui.TableNextColumn();
                var timeRemaining = dateTime - DateTime.UtcNow;
                ImGui.Text($"{timeRemaining.Days}.{timeRemaining.Hours:00}:{timeRemaining.Minutes:00}:{timeRemaining.Seconds:00}");
            
                ImGui.EndTable();
            }
        }
        else
        {
            ImGui.Text(Strings.AwaitingUserAction);
        }
    }
}