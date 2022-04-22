using DailyDuty.Enums;
using DailyDuty.Graphical;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using ImGuiNET;
using ImGuiScene;

namespace DailyDuty.Modules.Features
{
    internal class WondrousTailsDutyFinderOverlay : IConfigurable
    {
        public InfoBox? AutomationInformationBox { get; }
        public InfoBox? TechnicalInformation { get; }
        public TextureWrap? AboutImage { get; }
        public InfoBox? AboutInformationBox { get; }

        public TabFlags TabFlags => TabFlags.About | TabFlags.Options;

        public void DrawTabItem()
        {
            ImGui.Text(Strings.Features.WondrousTailsDutyFinderOverlayLabel);
        }

        public void DrawOptionsContents()
        {
            ImGui.Text("options!");
        }

        public string ConfigurationPaneLabel => Strings.Features.WondrousTailsDutyFinderOverlayLabel;
    }
}
