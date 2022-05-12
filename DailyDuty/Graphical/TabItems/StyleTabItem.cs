using DailyDuty.Data.Components;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Graphical.TabItems
{
    internal class StyleTabItem : ITabItem
    {
        public ModuleType ModuleType => ModuleType.StyleSettings;
        private static MainWindowSettings Settings => Service.SystemConfiguration.Windows.MainWindow;

        private readonly InfoBox style = new()
        {
            Label = Strings.Common.StyleOptionsLabel,
            ContentsAction = () =>
            {
                ImGui.SetNextItemWidth(175 * ImGuiHelpers.GlobalScale);
                ImGui.DragFloat(Strings.Common.OpacityLabel, ref Settings.Opacity, 0.01f, 0.0f, 1.0f);

                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    Service.SystemConfiguration.Save();
                }
            }
        };

        public void DrawTabItem()
        {
            ImGui.Text(Strings.Common.StyleLabel);
        }

        public void DrawConfigurationPane()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            style.DrawCentered();
            
            //ImGuiHelpers.ScaledDummy(30.0f);

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
