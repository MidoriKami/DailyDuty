using DailyDuty.Data.ModuleSettings;
using DailyDuty.Enums;
using DailyDuty.Graphical;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;
using ImGuiScene;

namespace DailyDuty.ModuleConfiguration
{
    internal class DutyRouletteDutyFinderOverlay : IConfigurable
    {
        public string ConfigurationPaneLabel => Strings.Features.DutyRouletteDutyFinderOverlayLabel;
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
            Label = Strings.Common.AutomationInformationLabel,
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
        public TextureWrap? AboutImage { get; }
        public TabFlags TabFlags => TabFlags.About | TabFlags.Options;
        public ModuleType ModuleType => ModuleType.DutyRouletteDutyFinderOverlay;
        private static DutyRouletteDutyFinderOverlaySettings Settings => Service.SystemConfiguration.Addons.DutyRouletteOverlaySettings;

        public readonly InfoBox Options = new()
        {
            Label = Strings.Configuration.OptionsTabLabel,
            ContentsAction = () =>
            {
                if (Draw.Checkbox(Strings.Common.EnabledLabel, ref Settings.Enabled))
                {
                    Service.SystemConfiguration.Save();
                }
            },
        };

        public DutyRouletteDutyFinderOverlay()
        {
            AboutImage = Image.LoadImage("DutyRouletteDutyFinderOverlay");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Features.DutyRouletteDutyFinderOverlayLabel);
        }

        public void DrawOptionsContents()
        {

            ImGuiHelpers.ScaledDummy(10.0f);
            Options.DrawCentered();
            
            ImGuiHelpers.ScaledDummy(30.0f);


            ImGuiHelpers.ScaledDummy(10.0f);
        }
    }
}
