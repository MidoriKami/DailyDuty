using System.Diagnostics;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib.Drawing;

namespace DailyDuty.UserInterface.Windows;

public class AboutWindow : Window
{
    public AboutWindow() : base("DailyDuty About")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(370, 230),
            MaximumSize = new Vector2(370, 230)
        };

        Flags |= ImGuiWindowFlags.NoResize;
    }

    public override void Draw()
    {
        ImGui.PushStyleColor(ImGuiCol.Button, 0xFF000000 | 0x005E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xDD000000 | 0x005E5BFFC);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xAA000000 | 0x005E5BFF);
        
        InfoBox.Instance
            .AddTitle("Support", out var innerWidth)
            .AddString("If you would like the work that I do, and want to help support me as thanks, you can donate to my Ko-Fi")
            .AddString("Even if you choose not to donate, know that I genuinely appreciate that you are enjoying using what I have put my heart into creating for you")
            .AddDummy(10.0f)
            .AddButton("Support me on Ko-Fi", () => Process.Start(new ProcessStartInfo {FileName = "https://ko-fi.com/midorikami", UseShellExecute = true}), new Vector2(innerWidth, 27.0f))
            .Draw();
        
        ImGui.PopStyleColor(3);
    }
}