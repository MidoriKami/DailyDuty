using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Enums;
using DailyDuty.Graphical;
using DailyDuty.Localization;
using Dalamud.Interface;
using ImGuiNET;
using ImGuiScene;

namespace DailyDuty.Interfaces
{
    internal interface IConfigurable
    {
        string ConfigurationPaneLabel { get; }

        InfoBox? AboutInformationBox { get; }
        InfoBox? AutomationInformationBox { get; }

        InfoBox? TechnicalInformation { get; }

        TextureWrap? AboutImage { get; }

        TabFlags TabFlags { get; }

        void DrawTabItem();
        
        void DrawConfigurationPane()
        {
            var contentWidth = ImGui.GetContentRegionAvail().X;
            var textWidth = ImGui.CalcTextSize(ConfigurationPaneLabel).X;
            var textStart = contentWidth / 2.0f - textWidth / 2.0f;

            ImGui.SetCursorPos(ImGui.GetCursorPos() with {X = textStart});
            ImGui.Text(ConfigurationPaneLabel);

            ImGui.Spacing();

            DrawTabs();
        }

        void DrawTabs()
        {
            if (ImGui.BeginTabBar("SelectionPaneTabBar", ImGuiTabBarFlags.Reorderable))
            {
                if (TabFlags.HasFlag(TabFlags.About))
                {
                    if (ImGui.BeginTabItem(Strings.Configuration.AboutTabLabel))
                    {
                        DrawAboutContents();

                        ImGui.EndTabItem();
                    }
                }

                if (TabFlags.HasFlag(TabFlags.Status))
                {
                    if (ImGui.BeginTabItem(Strings.Configuration.StatusTabLabel))
                    {
                        DrawStatusContents();

                        ImGui.EndTabItem();
                    }
                }

                if (TabFlags.HasFlag(TabFlags.Options))
                {
                    if (ImGui.BeginTabItem(Strings.Configuration.OptionsTabLabel))
                    {
                        DrawOptionsContents();

                        ImGui.EndTabItem();
                    }
                }

                if (TabFlags.HasFlag(TabFlags.Log))
                {
                    if (ImGui.BeginTabItem(Strings.Configuration.LogTabLabel))
                    {
                        DrawLogContents();

                        ImGui.EndTabItem();
                    }
                }

                ImGui.EndTabBar();
            }
        }

        void DrawAboutContents()
        {
            
            var region = ImGui.GetContentRegionAvail();
            var elementWidth = new Vector2(region.X * 0.80f, region.X * 0.45f);
            var currentPosition = ImGui.GetCursorPos();

            if (AboutImage != null)
            {
                ImGuiHelpers.ScaledDummy(20.0f);
                ImGui.SetCursorPos(currentPosition with {X = region.X / 2.0f - elementWidth.X / 2.0f });
                ImGui.Image(AboutImage.ImGuiHandle, elementWidth);
            }

            if (AboutInformationBox != null)
            {
                ImGuiHelpers.ScaledDummy(20.0f);
                currentPosition = ImGui.GetCursorPos();
                ImGui.SetCursorPos(currentPosition with {X = region.X / 2.0f - elementWidth.X / 2.0f });

                AboutInformationBox.Size = elementWidth;
                AboutInformationBox.Draw();
            }

            
            if (AutomationInformationBox != null)
            {
                ImGuiHelpers.ScaledDummy(20.0f);
                currentPosition = ImGui.GetCursorPos();
                ImGui.SetCursorPos(currentPosition with {X = region.X / 2.0f - elementWidth.X / 2.0f });

                AutomationInformationBox.Size = elementWidth;
                AutomationInformationBox.Draw();
            }

            if (TechnicalInformation != null)
            {
                ImGuiHelpers.ScaledDummy(20.0f);
                currentPosition = ImGui.GetCursorPos();
                ImGui.SetCursorPos(currentPosition with {X = region.X / 2.0f - elementWidth.X / 2.0f });

                TechnicalInformation.Size = elementWidth;
                TechnicalInformation.Draw();
            }
        }

        void DrawOptionsContents()
        {
            throw new NotImplementedException();
        }

        void DrawStatusContents()
        {
            throw new NotImplementedException();
        }

        void DrawLogContents()
        {
            throw new NotImplementedException();
        }
    }
}
