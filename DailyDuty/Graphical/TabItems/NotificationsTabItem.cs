using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Components;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Graphical.TabItems
{
    internal class NotificationsTabItem : ITabItem
    {
        public ModuleType ModuleType => ModuleType.NotificationSettings;

        private static SystemSettings Settings => Service.SystemConfiguration.System;

        private readonly InfoBox notificationThrottle = new InfoBox()
        {
            Label = Strings.Configuration.NotificationsThrottleLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Configuration.NotificationsThrottleDescription);
                
                ImGuiHelpers.ScaledDummy(10.0f);

                ImGui.PushItemWidth(50 * ImGuiHelpers.GlobalScale);
                ImGui.InputInt("", ref Settings.MinutesBetweenThrottledMessages, 0, 0);

                ImGui.SameLine();
                ImGui.Text(Strings.Common.MinutesLabel);
            }
        };

        public void DrawTabItem()
        {
            ImGui.Text(Strings.Configuration.NotificationsConfigurationLabel);
        }

        public void DrawConfigurationPane()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            notificationThrottle.DrawCentered();
            
            //ImGuiHelpers.ScaledDummy(30.0f);

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
