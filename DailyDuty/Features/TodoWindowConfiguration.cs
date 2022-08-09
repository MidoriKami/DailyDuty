using System.Linq;
using DailyDuty.Data.Components;
using DailyDuty.Enums;
using DailyDuty.Graphical;
using DailyDuty.Interfaces;
using DailyDuty.Localization;
using DailyDuty.Utilities;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using ImGuiScene;

namespace DailyDuty.Features
{
    internal class TodoWindowConfiguration : IConfigurable
    {
        public ModuleType ModuleType => ModuleType.TodoWindow;
        public string ConfigurationPaneLabel => Strings.Features.TodoWindowLabel;
        public InfoBox? AboutInformationBox { get; } = new()
        {
            Label = Strings.Common.InformationLabel,
            ContentsAction = () =>
            {
                ImGui.Text(Strings.Features.TodoWindowInformation);
            }
        };

        private readonly InfoBox options = new()
        {
            Label = Strings.Configuration.OptionsTabLabel,
            ContentsAction = () =>
            {
                if (Draw.Checkbox(Strings.Common.EnabledLabel, ref Settings.Enabled))
                {
                    Service.SystemConfiguration.Save();
                }

                if (Draw.Checkbox(Strings.Common.ClickthroughLabel, ref Settings.ClickThrough))
                {
                    Service.SystemConfiguration.Save();
                }
                ImGuiComponents.HelpMarker(Strings.Common.ClickthroughHelp);

                ImGui.SetNextItemWidth(150 * ImGuiHelpers.GlobalScale);
                ImGui.DragFloat(Strings.Common.OpacityLabel, ref Settings.Opacity, 0.01f, 0.0f, 1.0f);
                if (ImGui.IsItemDeactivatedAfterEdit())
                {
                    Service.SystemConfiguration.Save();
                }
            }
        };

        
        private readonly InfoBox taskSelection = new()
        {
            Label = Strings.Features.TodoWindowTaskSelectionLabel,
            ContentsAction = () =>
            {
                if (Draw.Checkbox(Strings.Features.TodoWindowShowDailyLabel, ref Settings.ShowDaily))
                {
                    Service.SystemConfiguration.Save();
                }

                if (Draw.Checkbox(Strings.Features.TodoWindowShowWeeklyLabel, ref Settings.ShowWeekly))
                {
                    Service.SystemConfiguration.Save();
                }
            }
        };

        private readonly InfoBox dailyTaskSelection = new()
        {
            Label = Strings.Common.DailyTasksLabel,
            ContentsAction = () =>
            {
                var tasks = Service.ModuleManager.GetCompletables(CompletionType.Daily)
                    .Where(taskSettings => taskSettings.GenericSettings.Enabled)
                    .OrderBy(task => task.DisplayName)
                    .ToList();

                foreach (var task in tasks)
                {
                    if (Draw.Checkbox(task.DisplayName, ref task.GenericSettings.ShowTodoTask))
                    {
                        Service.CharacterConfiguration.Save();
                    }
                }
            }
        };

        private readonly InfoBox weeklyTaskSelection = new()
        {
            Label = Strings.Common.WeeklyTasksLabel,
            ContentsAction = () =>
            {
                var tasks = Service.ModuleManager.GetCompletables(CompletionType.Weekly)
                    .Where(taskSettings => taskSettings.GenericSettings.Enabled)
                    .OrderBy(task => task.DisplayName)
                    .ToList();

                foreach (var task in tasks)
                {
                    if (Draw.Checkbox(task.DisplayName, ref task.GenericSettings.ShowTodoTask))
                    {
                        Service.CharacterConfiguration.Save();
                    }
                }
            
            }
        };

        private readonly InfoBox windowHiding = new()
        {
            Label = Strings.Features.TodoWindowHideWindowWhenLabel,
            ContentsAction = () =>
            {
                if (Draw.Checkbox(Strings.Features.TodoWindowHideInDutyLabel, ref Settings.HideInDuty))
                {
                    Service.SystemConfiguration.Save();
                }

                if (Draw.Checkbox(Strings.Features.TodoWindowHideWhenCompleteLabel, ref Settings.HideWhenTasksComplete))
                {
                    Service.SystemConfiguration.Save();
                }
            }
        };

        private readonly InfoBox displayOptions = new()
        {
            Label = Strings.Common.TaskDisplayOptionsLabel,
            ContentsAction = () =>
            {
                if (Draw.Checkbox(Strings.Features.TodoWindowShowWhenCompleteLabel, ref Settings.ShowTasksWhenComplete))
                {
                    Service.SystemConfiguration.Save();
                }

                ImGuiHelpers.ScaledDummy(10.0f);

                if (ImGui.ColorEdit4(Strings.Common.TaskHeaderColorLabel, ref Settings.Colors.HeaderColor, ImGuiColorEditFlags.NoInputs))
                {
                    Service.SystemConfiguration.Save();
                }

                if (ImGui.ColorEdit4(Strings.Common.TaskCompleteTaskColorLabel, ref Settings.Colors.CompleteColor, ImGuiColorEditFlags.NoInputs))
                {
                    Service.SystemConfiguration.Save();
                }

                if (ImGui.ColorEdit4(Strings.Common.TaskIncompleteTaskColorLabel, ref Settings.Colors.IncompleteColor, ImGuiColorEditFlags.NoInputs))
                {
                    Service.SystemConfiguration.Save();
                }
            }
        };

        private readonly InfoBox windowOptions = new()
        {
            Label = Strings.Features.TodoWindowResizeStyleLabel,
            ContentsAction = () =>
            {
                ImGui.SetNextItemWidth(150 * ImGuiHelpers.GlobalScale);
                if(ImGui.BeginCombo($"###StyleSelect", Settings.Style.GetLabel(), ImGuiComboFlags.PopupAlignLeft))
                {
                    if(ImGui.Selectable(TodoWindowStyle.AutoResize.GetLabel(), Settings.Style == TodoWindowStyle.AutoResize))
                        Settings.Style = TodoWindowStyle.AutoResize;

                    if(ImGui.Selectable(TodoWindowStyle.ManualSize.GetLabel(), Settings.Style == TodoWindowStyle.ManualSize))
                        Settings.Style = TodoWindowStyle.ManualSize;
                
                    ImGui.EndCombo();
                }

                if (Settings.Style != TodoWindowStyle.ManualSize)
                {
                    ImGui.TableNextColumn();

                    ImGui.SetNextItemWidth(150 * ImGuiHelpers.GlobalScale);

                    if(ImGui.BeginCombo("###AnchorSelect", Settings.Anchor.GetLabel(), ImGuiComboFlags.PopupAlignLeft))
                    {
                        if(ImGui.Selectable(WindowAnchor.TopLeft.GetLabel(), Settings.Anchor == WindowAnchor.TopLeft))
                            Settings.Anchor = WindowAnchor.TopLeft;

                        if(ImGui.Selectable(WindowAnchor.TopRight.GetLabel(), Settings.Anchor == WindowAnchor.TopRight))
                            Settings.Anchor = WindowAnchor.TopRight;

                        if(ImGui.Selectable(WindowAnchor.BottomLeft.GetLabel(), Settings.Anchor == WindowAnchor.BottomLeft))
                            Settings.Anchor = WindowAnchor.BottomLeft;

                        if(ImGui.Selectable(WindowAnchor.BottomRight.GetLabel(), Settings.Anchor == WindowAnchor.BottomRight))
                            Settings.Anchor = WindowAnchor.BottomRight;

                        ImGui.EndCombo();
                    }
                }

            }
        };

        public InfoBox? AutomationInformationBox => null;
        public InfoBox? TechnicalInformation => null;
        public TextureWrap? AboutImage { get; }
        public TabFlags TabFlags => TabFlags.About | TabFlags.Options;

        private static TodoWindowSettings Settings => Service.SystemConfiguration.Windows.Todo;

        public TodoWindowConfiguration()
        {
            AboutImage = Image.LoadImage("TodoWindow");
        }

        public void DrawTabItem()
        {
            ImGui.TextColored(Settings.Enabled ? Colors.SoftGreen : Colors.SoftRed, Strings.Features.TodoWindowLabel);
        }

        public void DrawOptionsContents()
        {
            ImGuiHelpers.ScaledDummy(10.0f);
            options.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            taskSelection.DrawCentered();

            if (Settings.ShowDaily)
            {
                ImGuiHelpers.ScaledDummy(30.0f);
                dailyTaskSelection.DrawCentered();
            }

            if (Settings.ShowWeekly)
            {
                ImGuiHelpers.ScaledDummy(30.0f);
                weeklyTaskSelection.DrawCentered();
            }

            ImGuiHelpers.ScaledDummy(30.0f);
            displayOptions.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            windowOptions.DrawCentered();

            ImGuiHelpers.ScaledDummy(30.0f);
            windowHiding.DrawCentered();

            ImGuiHelpers.ScaledDummy(20.0f);
        }
    }
}
