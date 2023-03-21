using System.Numerics;
using ImGuiNET;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Views.Components;

public class UiColorPickerView
{
    private static void ColorSelector(ref ushort index)
    {
        const ImGuiColorEditFlags paletteButtonFlags = ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.NoPicker | ImGuiColorEditFlags.NoTooltip;
        
        ImGui.BeginGroup(); // Lock X position
        ImGui.Text("Current color:");
        ImGui.SameLine();
        
        var selectedColor = ConvertToVector4(LuminaCache<UIColor>.Instance.GetRow(index)!.UIGlow);
        ImGui.ColorButton("SelectedColor", selectedColor);
        var shouldClose = false;

        var counter = 0;
        foreach (var color in LuminaCache<UIColor>.Instance)
        {
            ImGui.PushID($"ColorNumber{color}");
            if (counter++ % 15 != 0)
            {
                ImGui.SameLine(0f, ImGui.GetStyle().ItemSpacing.Y);
            }

            var loopColor = ConvertToVector4(color.UIGlow);
            if (ImGui.ColorButton("##palette", loopColor, paletteButtonFlags, new Vector2(20, 20)))
            {
                index = (ushort) color.RowId;
                shouldClose = true;
            }
            ImGui.PopID();
        }

        ImGui.EndGroup();

        shouldClose |= ImGui.Button("Close");
        ImGui.SameLine();
        if (ImGui.Button("Default"))
        {
            shouldClose = true;
            index = 14;
        }

        if (shouldClose)
        {
            ImGui.CloseCurrentPopup();
        }
    }

    public static void SelectorButton(ref ushort index)
    {
        var selectedColor = ConvertToVector4(LuminaCache<UIColor>.Instance.GetRow(index)!.UIGlow);
        if (ImGui.ColorButton("##pickerButton", selectedColor))
        {
            ImGui.OpenPopup("colorPickerPopup");
        }

        if (ImGui.BeginPopup("colorPickerPopup"))
        {
            ColorSelector(ref index);
            ImGui.EndPopup();
        }
    }
    
    private static Vector4 ConvertToVector4(uint color)
    {
        var r = (byte)(color >> 24);
        var g = (byte)(color >> 16);
        var b = (byte)(color >> 8);
        var a = (byte)color;

        return new Vector4(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
    }
}