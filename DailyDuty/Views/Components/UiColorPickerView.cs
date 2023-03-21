using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace DailyDuty.Views.Components;

public class ColorInfo
{
    public ushort Index = ushort.MaxValue;
    public byte R;
    public byte G;
    public byte B;
    public byte A;

    public static ColorInfo FromUiColor(ushort index, uint foreground)
    {
        return new ColorInfo
        {
            Index = index,
            R = (byte)((foreground >> 24) & 0xFF),
            G = (byte)((foreground >> 16) & 0xFF),
            B = (byte)((foreground >> 8) & 0xFF),
            A = (byte)((foreground >> 0) & 0xFF),
        };
    }

    public override string ToString()
    {
        return $"#{R:X2}{G:X2}{B:X2}:{Index}";
    }

    public Vector4 Vec4 => new(R / 255f, G / 255f, B / 255f, A / 255f);
}

public class UiColorPickerView
{
    private static bool ColorSelector(ref ushort index)
    {
        const ImGuiColorEditFlags paletteButtonFlags = ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.NoPicker | ImGuiColorEditFlags.NoTooltip;
        
        ImGui.BeginGroup(); // Lock X position
        ImGui.Text("Current color:");
        ImGui.SameLine();
        var selectedColor = ConvertToVector4(LuminaCache<UIColor>.Instance.GetRow(index)!.UIGlow);
        ImGui.ColorButton($"SelectedColor", selectedColor);
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
            if (ImGui.ColorButton($"##palette", loopColor, paletteButtonFlags, new Vector2(20, 20)))
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
        return shouldClose;
    }

    public static bool SelectorButton(ref ushort index)
    {
        var colorSelected = false;
        var selectedColor = ConvertToVector4(LuminaCache<UIColor>.Instance.GetRow(index)!.UIGlow);
        if (ImGui.ColorButton($"##pickerButton", selectedColor))
        {
            ImGui.OpenPopup($"popup");
        }

        if (ImGui.BeginPopup($"popup"))
        {
            colorSelected = ColorSelector(ref index);
            ImGui.EndPopup();
        }
        return colorSelected;
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