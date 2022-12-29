using System.Diagnostics;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib;
using KamiLib.InfoBoxSystem;

namespace DailyDuty.UserInterface.Windows;

public class AboutWindow : Window
{
    public AboutWindow() : base("DailyDuty About")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(575, 625),
            MaximumSize = new Vector2(575, 625)
        };

        Flags |= ImGuiWindowFlags.NoResize;
    }

    public override void Draw()
    {
        ImGuiHelpers.ScaledDummy(10.0f);
        
        InfoBox.Instance
            .AddTitle("About")
            .AddString("DailyDuty is a automatic tracker of various daily and weekly tasks")
            .AddString("DailyDuty can only track tasks being completed while the plugin is running")
            .AddString("If for any reason the plugin wasn't able to be active while you completed a task, try speaking to the relevant npc and the data should re-sync automatically")
            .AddString("If you have any issues or questions, join the goatplace discord\n(link available on the XIVLauncher website)")
            .Draw();
        
        InfoBox.Instance
            .AddTitle("Thank You")
            .AddString("DailyDuty has been a hobby project of mine for the last 11 months, each and every module in this plugin was the result of many hours of research, reverse engineer, debugging, and help from the community.")
            .AddString("Originally this project was just to help me remember to do some of the various tasks each day, but as more and more people started using the plugin and providing their feedback this project has evolved greatly.")
            .AddString("I want to give a special thanks to Khayle for insisting DailyDuty should be translatable, and another thanks to everyone to provided translations.")
            .Draw();
        
        ImGui.PushStyleColor(ImGuiCol.Button, 0xFF000000 | 0x005E5BFF);
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xDD000000 | 0x005E5BFFC);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xAA000000 | 0x005E5BFF);
        
        InfoBox.Instance
            .AddTitle("Support")
            .AddString("If you would like to help me out, \nI have setup a Ko-Fi specifically for this plugin.")
            .AddString("Even if you choose not to donate, know that I genuinely appreciate that you are enjoying using what I have put my heart into creating for you. <3")
            .AddAction(() => ImGuiHelpers.ScaledDummy(10.0f))
            .AddButton("Support me on Ko-Fi", () => Process.Start(new ProcessStartInfo {FileName = "https://ko-fi.com/midorikami", UseShellExecute = true}), ImGuiHelpers.ScaledVector2(InfoBox.Instance.InnerWidth, 27.0f))
            .Draw();
        
        ImGui.PopStyleColor(3);
    }

    public static void DrawInfoButton()
    {
        var windowSize = ImGui.GetWindowSize();

        var position = windowSize - ImGuiHelpers.ScaledVector2(70.0f, 50.0f);

        ImGui.PushFont(UiBuilder.IconFont);
        ImGui.SetCursorPos(position);
        ImGui.PushStyleColor(ImGuiCol.Button, 0x000000FF);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xFFA06020);

        if(ImGui.BeginChild("###ButtonChild", ImGuiHelpers.ScaledVector2(60.0f)))
        {
            if (ImGui.Button(FontAwesomeIcon.InfoCircle.ToIconString(), ImGuiHelpers.ScaledVector2(40.0f)))
            {
                var window = KamiCommon.WindowManager.GetWindowOfType<AboutWindow>();
                if (window != null)
                {
                    window.IsOpen = true;
                }
            }
        }        
        ImGui.EndChild();
        
        ImGui.PopStyleColor();
        ImGui.PopStyleColor();
        ImGui.PopFont();
    }
}