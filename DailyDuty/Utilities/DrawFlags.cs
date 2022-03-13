using ImGuiNET;

namespace DailyDuty.Utilities
{
    internal static class DrawFlags
    {
        public const ImGuiWindowFlags DefaultFlags = ImGuiWindowFlags.NoFocusOnAppearing |
                                                     ImGuiWindowFlags.NoTitleBar |
                                                     ImGuiWindowFlags.NoScrollbar |
                                                     ImGuiWindowFlags.NoCollapse;

        public const ImGuiWindowFlags AutoResize = ImGuiWindowFlags.NoFocusOnAppearing |
                                                   ImGuiWindowFlags.NoTitleBar |
                                                   ImGuiWindowFlags.NoScrollbar |
                                                   ImGuiWindowFlags.NoCollapse |
                                                   ImGuiWindowFlags.AlwaysAutoResize;

        public const ImGuiWindowFlags ClickThroughFlags = ImGuiWindowFlags.NoFocusOnAppearing |
                                                          ImGuiWindowFlags.NoDecoration |
                                                          ImGuiWindowFlags.NoInputs |
                                                          ImGuiWindowFlags.AlwaysAutoResize;

        public const ImGuiWindowFlags Debug = ImGuiWindowFlags.NoBringToFrontOnFocus;
    }
}
