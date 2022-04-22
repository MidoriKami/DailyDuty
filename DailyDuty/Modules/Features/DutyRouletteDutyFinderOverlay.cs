using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Enums;
using DailyDuty.Graphical;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using ImGuiNET;
using ImGuiScene;

namespace DailyDuty.Modules.Features
{
    internal class DutyRouletteDutyFinderOverlay : IConfigurable
    {
        public string ConfigurationPaneLabel { get; } = Strings.Features.DutyRouletteDutyFinderOverlayLabel;

        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Features.InformationLabel,
            ContentsAction = () =>
            {
                ImGui.TextColored(Colors.SoftRed, Strings.Features.DutyRouletteDutyFinderOverlayInformationDisclaimer);
                ImGui.Text(Strings.Features.DutyRouletteDutyFinderOverlayDescription);
            }

        };

        public InfoBox? AutomationInformationBox { get; } = new()
        {
            Label = Strings.Features.DutyRouletteDutyFinderOverlayAutomationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Features.DutyRouletteDutyFinderOverlayAutomationInformation);
            }
        };

        public InfoBox? TechnicalInformation { get; } = new()
        {

        };

        public DutyRouletteDutyFinderOverlay()
        {
            var assemblyLocation = Service.PluginInterface.AssemblyLocation.DirectoryName!;
            var imagePath = Path.Combine(assemblyLocation, $@"images\DutyRouletteDutyFinderOverlay.png");

            AboutImage = Service.PluginInterface.UiBuilder.LoadImage(imagePath);
        }

        public TextureWrap? AboutImage { get; }

        public TabFlags TabFlags => TabFlags.About | TabFlags.Options;

        public void DrawTabItem()
        {
            ImGui.Text(Strings.Features.DutyRouletteDutyFinderOverlayLabel);
        }

        public void DrawOptionsContents()
        {
            
        }
    }
}
