using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Components;
using DailyDuty.Enums;
using DailyDuty.Graphical;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using ImGuiNET;
using ImGuiScene;

namespace DailyDuty.Features
{
    internal class TodoWindowConfiguration : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.TodoWindow;
        public string ConfigurationPaneLabel => Strings.Features.TodoWindowLabel;
        public InfoBox? AboutInformationBox { get; }
        public InfoBox? AutomationInformationBox { get; }
        public InfoBox? TechnicalInformation { get; }
        public TextureWrap? AboutImage { get; }
        public TabFlags TabFlags => TabFlags.About | TabFlags.Options;

        private static TodoWindowSettings Settings => Service.SystemConfiguration.Windows.Todo;

        public TodoWindowConfiguration()
        {

        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Features.DutyRouletteDutyFinderOverlayLabel);
        }

        public void DrawOptionsContents()
        {
            
        }
    }
}
