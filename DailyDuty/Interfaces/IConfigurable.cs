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
                        ImGui.BeginChild("AboutContentsChild", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.AlwaysVerticalScrollbar);

                        DrawAboutContents();

                        ImGui.EndChild();

                        ImGui.EndTabItem();
                    }
                }

                if (TabFlags.HasFlag(TabFlags.Status))
                {
                    if (ImGui.BeginTabItem(Strings.Configuration.StatusTabLabel))
                    {
                        ImGui.BeginChild("StatusContentsChild", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.AlwaysVerticalScrollbar);

                        DrawStatusContents();

                        ImGui.EndChild();

                        ImGui.EndTabItem();
                    }
                }

                if (TabFlags.HasFlag(TabFlags.Options))
                {
                    if (ImGui.BeginTabItem(Strings.Configuration.OptionsTabLabel))
                    {
                        ImGui.BeginChild("OptionsContentsChild", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.AlwaysVerticalScrollbar);

                        DrawOptionsContents();

                        ImGui.EndChild();

                        ImGui.EndTabItem();
                    }
                }

                if (TabFlags.HasFlag(TabFlags.Log))
                {
                    if (ImGui.BeginTabItem(Strings.Configuration.LogTabLabel))
                    {
                        ImGui.BeginChild("LogContentsChild", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.AlwaysVerticalScrollbar);

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
                var imageSize = new Vector2(width, height);
                var insetVector = new Vector2(2.5f);

                ImGui.SetCursorPos(currentPosition with { X = region.X / 2.0f - imageSize.X / 2.0f, Y = currentPosition.Y + 10.0f * ImGuiHelpers.GlobalScale });
                var startPosition = ImGui.GetCursorScreenPos();
                var imageStart = startPosition + insetVector;
                var imageStop = startPosition + imageSize - insetVector;

                ImGui.GetWindowDrawList().AddRectFilled(startPosition, startPosition + imageSize, ImGui.GetColorU32(Colors.White), 40.0f);

                ImGui.GetWindowDrawList().AddImageRounded(AboutImage.ImGuiHandle, imageStart, imageStop, Vector2.Zero, Vector2.One, ImGui.GetColorU32(Colors.White), 40.0f);

                ImGui.SetCursorScreenPos(startPosition + imageSize);
                ImGuiHelpers.ScaledDummy(20.0f);
            }

            ImGui.PushStyleColor(ImGuiCol.Text, Colors.Grey);

            if (AboutInformationBox != null)
            {
                AboutInformationBox.DrawCentered();
                ImGuiHelpers.ScaledDummy(30.0f);
            }
            
            if (AutomationInformationBox != null)
            {
                AutomationInformationBox.DrawCentered();
                ImGuiHelpers.ScaledDummy(30.0f);
            }

            if (TechnicalInformation != null)
            {
                TechnicalInformation.DrawCentered();
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
