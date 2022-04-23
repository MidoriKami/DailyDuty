using DailyDuty.Data.FeaturesSettings;
using DailyDuty.Enums;
using DailyDuty.Graphical;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;
using ImGuiScene;

namespace DailyDuty.ModuleConfiguration.Features
{
    internal class WondrousTailsDutyFinderOverlay : IConfigurable
    {
        public InfoBox? AutomationInformationBox { get; } = new()
        {
            Label = Strings.Common.AutomationInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Features.WondrousTailsDutyFinderOverlayAutomationInformation);
            }
        };

        public InfoBox? TechnicalInformation { get; } = new()
        {
            Label = Strings.Common.TechnicalInformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Features.WondrousTailsDutyFinderOverlayTechnicalDescription);
            }
        };
        public TextureWrap? AboutImage { get; }

        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Common.InformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Features.WondrousTailsDutyFinderOverlayDescription);
            }
        };
        public TabFlags TabFlags => TabFlags.About | TabFlags.Options;
        public ModuleType ModuleType => ModuleType.WondrousTailsDutyFinderOverlay;
        private static WondrousTailsDutyFinderOverlaySettings Settings => Service.SystemConfiguration.Addons.WondrousTailsOverlaySettings;

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


        public WondrousTailsDutyFinderOverlay()
        {
            AboutImage = Image.LoadImage("WondrousTailsDutyFinderOverlay");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Features.WondrousTailsDutyFinderOverlayLabel);
        }

        public void DrawOptionsContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            Options.DrawCentered(0.8f);

            //ImGuiHelpers.ScaledDummy(30.0f);

            ImGuiHelpers.ScaledDummy(10.0f);
        }

        public string ConfigurationPaneLabel => Strings.Features.WondrousTailsDutyFinderOverlayLabel;
    }
}
