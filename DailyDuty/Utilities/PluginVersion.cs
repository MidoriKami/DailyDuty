using System.Reflection;
using ImGuiNET;
using KamiLib.Drawing;

namespace DailyDuty.Utilities;

public class PluginVersion
{
    private static PluginVersion? _instance;
    public static PluginVersion Instance => _instance ??= new PluginVersion();

    private readonly string versionText;
    
    private PluginVersion()
    {
        versionText = GetVersionText();
    }
    
    private static string GetVersionText()
    {
        var assemblyInformation = Assembly.GetExecutingAssembly().FullName!.Split(',');

        var versionString = assemblyInformation[1].Replace('=', ' ');

        var commitInfo = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown";
        return $"{versionString} - {commitInfo}";
    }
    
    public void DrawVersionText()
    {
        var region = ImGui.GetContentRegionAvail();

        var versionTextSize = ImGui.CalcTextSize(versionText) / 2.0f;
        var cursorStart = ImGui.GetCursorPos();
        cursorStart.X += region.X / 2.0f - versionTextSize.X;

        ImGui.SetCursorPos(cursorStart);
        ImGui.TextColored(Colors.Grey, versionText);
    }
}