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

        public const ImGuiWindowFlags ManualSize = ImGuiWindowFlags.NoFocusOnAppearing |
                                                   ImGuiWindowFlags.NoTitleBar |
                                                   ImGuiWindowFlags.NoCollapse;

        public const ImGuiWindowFlags LockPosition = ImGuiWindowFlags.NoMove |
                                                     ImGuiWindowFlags.NoResize;

        public const ImGuiWindowFlags Debug = ImGuiWindowFlags.NoBringToFrontOnFocus;
    }
}
