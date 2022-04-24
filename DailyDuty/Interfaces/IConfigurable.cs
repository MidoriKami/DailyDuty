using System;
using System.Linq;
using System.Numerics;
using DailyDuty.Enums;
using DailyDuty.Graphical;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;
using ImGuiScene;

namespace DailyDuty.Interfaces
{
    internal interface IConfigurable : ITabItem
    {
        string ConfigurationPaneLabel { get; }
        InfoBox? AboutInformationBox { get; }
        InfoBox? AutomationInformationBox { get; }
        InfoBox? TechnicalInformation { get; }
        TextureWrap? AboutImage { get; }
        TabFlags TabFlags { get; }

        void ITabItem.DrawConfigurationPane()
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
            if (ImGui.BeginTabBar("SelectionPaneTabBar", ImGuiTabBarFlags.None))
            {
                if (TabFlags.HasFlag(TabFlags.About))
                {
                    if (ImGui.BeginTabItem(Strings.Configuration.AboutTabLabel))
                    {
                        ImGui.BeginChild("AboutContentsChild");

                        DrawAboutContents();

                        ImGui.EndChild();

                        ImGui.EndTabItem();
                    }
                }

                if (TabFlags.HasFlag(TabFlags.Status))
                {
                    if (ImGui.BeginTabItem(Strings.Configuration.StatusTabLabel))
                    {
                        ImGui.BeginChild("StatusContentsChild");

                        DrawStatusContents();

                        ImGui.EndChild();

                        ImGui.EndTabItem();
                    }
                }

                if (TabFlags.HasFlag(TabFlags.Options))
                {
                    if (ImGui.BeginTabItem(Strings.Configuration.OptionsTabLabel))
                    {
                        ImGui.BeginChild("OptionsContentsChild");

                        DrawOptionsContents();

                        ImGui.EndChild();

                        ImGui.EndTabItem();
                    }
                }

                if (TabFlags.HasFlag(TabFlags.Log))
                {
                    if (ImGui.BeginTabItem(Strings.Configuration.LogTabLabel))
                    {
                        ImGui.BeginChild("LogContentsChild");

                        ImGui.Spacing();

                        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, ImGuiHelpers.ScaledVector2(10.0f, 8.0f));

                        var logMessages = Service.LogManager.GetMessages(ModuleType).ToList();

                        if (logMessages.Count == 0)
                        {
                            var contentCenter = ImGui.GetContentRegionAvail() / 2.0f;
                            var textSize = ImGui.CalcTextSize(Strings.Common.EmptyContentsLabel) / 2.0f;

                            ImGui.SetCursorPos(contentCenter - textSize);
                            ImGui.TextWrapped(Strings.Common.EmptyContentsLabel);
                        }
                        else
                        {
                            foreach (var message in logMessages)
                            {
                                message.Draw();
                            }
                        }

                        ImGui.PopStyleVar();

                        ImGui.EndChild();

                        ImGui.EndTabItem();
                    }
                }

                ImGui.EndTabBar();
            }
        }

        void DrawAboutContents()
        {
            var region = ImGui.GetContentRegionAvail();
            var currentPosition = ImGui.GetCursorPos();

            if (AboutImage != null)
            {
                var imageRatio = (float)AboutImage.Height / AboutImage.Width;
                var width = region.X * 0.80f;
                var height = width * imageRatio;
                var elementWidth = new Vector2(width, height);
                ImGui.SetCursorPos(currentPosition with { X = region.X / 2.0f - elementWidth.X / 2.0f, Y = currentPosition.Y + 10.0f * ImGuiHelpers.GlobalScale });
                var startPosition = ImGui.GetCursorScreenPos();

                ImGui.Image(AboutImage.ImGuiHandle, elementWidth);
                Draw.Rectangle(startPosition, elementWidth, 3.0f);

                ImGuiHelpers.ScaledDummy(20.0f);
            }

            ImGui.PushStyleColor(ImGuiCol.Text, Colors.Grey);

            if (AboutInformationBox != null)
            {
                AboutInformationBox.DrawCentered(0.80f);
                ImGuiHelpers.ScaledDummy(30.0f);
            }
            
            if (AutomationInformationBox != null)
            {
                AutomationInformationBox.DrawCentered(0.80f);
                ImGuiHelpers.ScaledDummy(30.0f);
            }

            if (TechnicalInformation != null)
            {
                TechnicalInformation.DrawCentered(0.80f);
                ImGuiHelpers.ScaledDummy(20.0f);
            }

            ImGui.PopStyleColor();
        }

        void DrawOptionsContents()
        {
            throw new NotImplementedException();
        }

        void DrawStatusContents()
        {
            throw new NotImplementedException();
        }
    }
}
