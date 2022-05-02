using System;
using System.Globalization;
using DailyDuty.Data.Components;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
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
                Settings.MinutesBetweenThrottledMessages = Math.Max(0, Settings.MinutesBetweenThrottledMessages);

                ImGui.SameLine();
                ImGui.Text(Strings.Common.MinutesLabel);
            }
        };

        private readonly InfoBox messageSuppression = new()
        {
            Label = Strings.Configuration.NotificationsDelayLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Configuration.NotificationsDelayDescription);

                ImGuiHelpers.ScaledDummy(10.0f);
                if (Draw.Checkbox(Strings.Common.EnabledLabel, ref Settings.MessageDelay))
                {
                    Service.SystemConfiguration.Save();
                }

                if (Settings.MessageDelay)
                {
                    var clientLanguage = Service.PluginInterface.UiLanguage;
                    var cultureInfo = new CultureInfo(clientLanguage);

                    ImGuiHelpers.ScaledDummy(5.0f);

                    ImGui.PushItemWidth(150.0f * ImGuiHelpers.GlobalScale);
                    if (ImGui.BeginCombo("###Weekday", cultureInfo.DateTimeFormat.GetDayName(Settings.DelayDay)))
                    {
                        foreach (var day in Enum.GetValues<DayOfWeek>())
                        {
                            if (ImGui.Selectable(cultureInfo.DateTimeFormat.GetDayName(day), Settings.DelayDay == day))
                            {
                                Settings.DelayDay = day;
                                Service.SystemConfiguration.Save();
                            }
                        }

                        ImGui.EndCombo();
                    }
                }
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

            ImGuiHelpers.ScaledDummy(30.0f);
            messageSuppression.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
