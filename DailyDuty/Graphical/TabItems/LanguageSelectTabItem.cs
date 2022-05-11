using System.Collections.Generic;
using System.Linq;
using DailyDuty.Data.Components;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using Dalamud.Interface;
using ImGuiNET;

namespace DailyDuty.Graphical.TabItems
{
    internal class LanguageSelectTabItem : ITabItem
    {
        public ModuleType ModuleType => ModuleType.LanguageSelect;

        private static readonly Dictionary<string, string> Languages = new()
        {
            {string.Empty, "Language Not Set"},
            {"de", "Deutsch"},
            {"es", "Español"},
            {"fr", "Français"},
            {"it", "Italiano"},
            {"ja", "日本語"},
            {"no", "Norsk"},
            {"pt", "Português"},
            {"ru", "Русский"},
            {"en", "English"}
        };

        private static SystemSettings Settings => Service.SystemConfiguration.System;

        private readonly InfoBox languageSelectBox = new()
        {
            Label = Strings.Configuration.LanguageSelectLabel,
            ContentsAction = () =>
            {
                var contentWidth = ImGui.GetContentRegionAvail();

                ImGui.SetNextItemWidth(contentWidth.X * 0.60f);
                if (ImGui.BeginCombo("", Languages[Settings.SelectedLanguage]))
                {
                    foreach (var (languageCode, language) in Languages.OrderBy(language => language.Key))
                    {
                        if(languageCode == string.Empty) continue;

                        if(ImGui.Selectable(language, Settings.SelectedLanguage == languageCode))
                        {
                            Settings.SelectedLanguage = languageCode;

                            DailyDutyPlugin.LoadLocalization(languageCode);

                            Service.SystemConfiguration.Save();
                        }
                    }

                    ImGui.EndCombo();
                }
            }
        };

        public void DrawTabItem()
        {
            ImGui.Text(Strings.Configuration.LanguageLabel);
        }

        public void DrawConfigurationPane()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            languageSelectBox.DrawCentered();
            
            //ImGuiHelpers.ScaledDummy(30.0f);

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
