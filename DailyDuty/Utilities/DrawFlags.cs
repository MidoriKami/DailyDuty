using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace DailyDuty.Utilities
{
    internal static class DrawFlags
    {
        public static ImGuiWindowFlags DefaultFlags = ImGuiWindowFlags.NoFocusOnAppearing |
                                                       ImGuiWindowFlags.NoTitleBar |
                                                       ImGuiWindowFlags.NoScrollbar |
                                                       ImGuiWindowFlags.NoCollapse;

        public static ImGuiWindowFlags AutoResize = ImGuiWindowFlags.NoFocusOnAppearing |
                                                    ImGuiWindowFlags.NoTitleBar |
                                                    ImGuiWindowFlags.NoScrollbar |
                                                    ImGuiWindowFlags.NoCollapse |
                                                    ImGuiWindowFlags.AlwaysAutoResize;

        public const ImGuiWindowFlags ClickThroughFlags = ImGuiWindowFlags.NoFocusOnAppearing |
                                                          ImGuiWindowFlags.NoDecoration |
                                                          ImGuiWindowFlags.NoInputs |
                                                          ImGuiWindowFlags.AlwaysAutoResize;
    }
}
