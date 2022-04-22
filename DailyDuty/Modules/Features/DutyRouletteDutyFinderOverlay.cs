using System.IO;
using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Graphical;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;
using ImGuiScene;

namespace DailyDuty.Modules.Features
{
    internal class DutyRouletteDutyFinderOverlay : IConfigurable
    {
        public string ConfigurationPaneLabel { get; } = Strings.Features.DutyRouletteDutyFinderOverlayLabel;

        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Common.InformationLabel,
            ContentsAction = () =>
            {
                ImGui.TextColored(Colors.SoftRed, Strings.Features.DutyRouletteDutyFinderOverlayInformationDisclaimer);
                ImGui.Text(Strings.Features.DutyRouletteDutyFinderOverlayDescription);
            }
        };
        public InfoBox? AutomationInformationBox { get; } = new()
        {
            Label = Strings.Common.DataCollectionLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Features.DutyRouletteDutyFinderOverlayAutomationInformation);
            }
        };
        public InfoBox? TechnicalInformation { get; } = new()
        {
            Label = Strings.Common.TechnicalInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Features.DutyRouletteDutyFinderOverlayTechnicalDescription);
            }
        };

        public readonly InfoBox Options = new()
        {
            Label = Strings.Configuration.OptionsTabLabel,
            ContentsAction = () =>
            {
                Draw.Checkbox(Strings.Common.EnabledLabel, ref Settings.Enabled);
            },
        };

        public TextureWrap? AboutImage { get; }
        public TabFlags TabFlags => TabFlags.About | TabFlags.Options;
        private static DutyRouletteDutyFinderOverlaySettings Settings => Service.SystemConfiguration.Addons.DutyRouletteOverlaySettings;

        public DutyRouletteDutyFinderOverlay()
        {
            var assemblyLocation = Service.PluginInterface.AssemblyLocation.DirectoryName!;
            var imagePath = Path.Combine(assemblyLocation, $@"images\DutyRouletteDutyFinderOverlay.png");

            AboutImage = Service.PluginInterface.UiBuilder.LoadImage(imagePath);
        }

        public void DrawTabItem()
        {
            if (Settings.Enabled)
            {
                ImGui.TextColored(Colors.SoftGreen, Strings.Features.DutyRouletteDutyFinderOverlayLabel);
            }
            else
            {
                ImGui.TextColored(Colors.SoftRed, Strings.Features.DutyRouletteDutyFinderOverlayLabel);
            }
        }

        public void DrawOptionsContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);

            Options.DrawCentered(0.8f);
        }
    }
}
