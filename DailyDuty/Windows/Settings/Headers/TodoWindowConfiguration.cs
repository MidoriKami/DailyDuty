using DailyDuty.Data.SettingsObjects.Windows;
using DailyDuty.Data.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Interface;
using ImGuiNET;
using NotImplementedException = System.NotImplementedException;

namespace DailyDuty.Windows.Settings.Headers
{
    internal class TodoWindowConfiguration : ICollapsibleHeader
    {
        private TodoWindowSettings Settings => Service.Configuration.Windows.Todo;

        public string HeaderText => "Todo Window Configuration";

        void ICollapsibleHeader.DrawContents()
        {
            ImGui.Indent(15 * ImGuiHelpers.GlobalScale);

            var numColumns = Service.Configuration.System.SingleColumnSettings ? 1 : (int)(ImGui.GetWindowSize().X / 250.0f);

            ImGui.BeginTable("Todo Configuration Table)", numColumns);

            ImGui.TableNextColumn();
            Draw.Checkbox("Show Todo Window", ref Settings.Open, "Shows/Hides the Todo Window");

            ImGui.TableNextColumn();
            Draw.Checkbox("Daily Tasks", ref Settings.ShowDaily, "Show/Hide Daily Tasks category");

            ImGui.TableNextColumn();
            Draw.Checkbox("Weekly Tasks", ref Settings.ShowWeekly, "Show/Hide Weekly Tasks category");

            ImGui.TableNextColumn();
            ImGui.Checkbox("Hide when Bound By Duty", ref Settings.HideInDuty);

            ImGui.TableNextColumn();
            Draw.Checkbox("Hide Completed Categories", ref Settings.HideWhenTasksComplete);

            ImGui.TableNextColumn();
            Draw.Checkbox("Show Completed Tasks", ref Settings.ShowTasksWhenComplete, "Show all tracked tasks, using complete/incomplete colors");

            StyleSelect();

            AnchorSelect();

            OpacitySlider();
        
            EditColors();

            ImGui.EndTable();
            
            ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
        }

        private void StyleSelect()
        {
            ImGui.TableNextColumn();
            Draw.Checkbox("Enable Click-through", ref Settings.ClickThrough, "Enables/Disables the ability to move the Todo Window");

            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth(150 * ImGuiHelpers.GlobalScale);
            if(ImGui.BeginCombo($"Window Style", Settings.Style.ToString(), ImGuiComboFlags.PopupAlignLeft))
            {
                if(ImGui.Selectable("Auto Resize", Settings.Style == TodoWindowStyle.AutoResize))
                    Settings.Style = TodoWindowStyle.AutoResize;

                if(ImGui.Selectable("Manual Size", Settings.Style == TodoWindowStyle.ManualSize))
                    Settings.Style = TodoWindowStyle.ManualSize;
                
                ImGui.EndCombo();
            }
        }

        public void Dispose()
        {
            
        }

        private void AnchorSelect()
        {
            if (Settings.Style != TodoWindowStyle.ManualSize)
            {
                ImGui.TableNextColumn();

                var val = (Settings.Anchor.HasFlag(WindowAnchor.Bottom) ? "Bottom " : "Top ") +
                          (Settings.Anchor.HasFlag(WindowAnchor.Right)  ? "Right" : "Left");
            
                ImGui.SetNextItemWidth(150 * ImGuiHelpers.GlobalScale);

                if(ImGui.BeginCombo($"Anchor Point", val, ImGuiComboFlags.PopupAlignLeft))
                {
                    if(ImGui.Selectable("Top Left", Settings.Anchor == WindowAnchor.TopLeft))
                        Settings.Anchor = WindowAnchor.TopLeft;

                    if(ImGui.Selectable("Top Right", Settings.Anchor == WindowAnchor.TopRight))
                        Settings.Anchor = WindowAnchor.TopRight;

                    if(ImGui.Selectable("Bottom Left", Settings.Anchor == WindowAnchor.BottomLeft))
                        Settings.Anchor = WindowAnchor.BottomLeft;

                    if(ImGui.Selectable("Bottom Right", Settings.Anchor == WindowAnchor.BottomRight))
                        Settings.Anchor = WindowAnchor.BottomRight;

                    ImGui.EndCombo();
                }
            }
        }

        private void OpacitySlider()
        {
            ImGui.TableNextColumn();
            ImGui.SetNextItemWidth(150 * ImGuiHelpers.GlobalScale);
            ImGui.DragFloat($"Opacity", ref Settings.Opacity, 0.01f, 0.0f, 1.0f);
        }

        private void EditColors()
        {
            ImGui.TableNextColumn();
            ImGui.ColorEdit4("Header Color", ref Settings.Colors.HeaderColor, ImGuiColorEditFlags.NoInputs);

            ImGui.TableNextColumn();
            ImGui.ColorEdit4("Completed Task Color", ref Settings.Colors.CompleteColor, ImGuiColorEditFlags.NoInputs);

            ImGui.TableNextColumn();
            ImGui.ColorEdit4("Incomplete Task Color", ref Settings.Colors.IncompleteColor, ImGuiColorEditFlags.NoInputs);
        }
    }
}