using System;
using DailyDuty.Interfaces;
using KamiToolKit.Controls.Builders;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes;

namespace DailyDuty.Nodes.Module;

public class ModuleInfoNode : ResNode, INativeUpdatable {
    private readonly IModule module;

    private readonly ILayoutNode layoutContainer;

    private TextNode statusNode = null!;
    private TextNode nextResetDateNode = null!;
    private TextNode nextResetTimeRemainingNode = null!;
    private TextNode? windowEndDateNode;
    private TextNode? windowEndTimeRemainingNode;

    public ModuleInfoNode(IModule module) {
        this.module = module;

        layoutContainer = NativeUiBuilder.Create(buildAction => buildAction
            .TextCentered(module.DisplayName, 14)
            .Separator()
            .Dummy(1.0f)
            .TabbedContent(tabBar => tabBar
                .AddTab("Status", BuildStatusTab)
                .AddTab("Data", BuildDataTab)
                .AddTab("Config", BuildConfigTab)));

        layoutContainer.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        layoutContainer.Size = Size;
    }

    public void Update() {
        statusNode.String = module.GetStatus().ToString();
        UpdateTimerNode(nextResetDateNode, nextResetTimeRemainingNode, module.GetNextResetTime());

        if (windowEndDateNode is not null && windowEndTimeRemainingNode is not null) {

            if (module.GetWindowEndTime() is { } windowEndTime) {
                windowEndDateNode.IsVisible = true;
                windowEndTimeRemainingNode.IsVisible = true;
                UpdateTimerNode(windowEndDateNode, windowEndTimeRemainingNode, windowEndTime);
            }
            else {
                windowEndDateNode.IsVisible = false;
                windowEndTimeRemainingNode.IsVisible = false;
            }

        }

        module.UpdateDataUi();
    }

    private static void UpdateTimerNode(TextNode? dateNode, TextNode? remainingNode, DateTime time) {
        if (time == DateTime.MaxValue) {
            dateNode?.String = "N/A";
            remainingNode?.String = "N/A";
            return;
        }

        dateNode?.String = $"{time.ToLocalTime():dddd, MMMM dd, yyyy hh:mm tt}";

        var remaining = time.ToUniversalTime() - DateTime.UtcNow;
        remainingNode?.String = remaining.TotalSeconds > 0 ? $"{remaining:d\\.hh\\:mm\\:ss}" : "No Longer Available";
    }

    private void BuildStatusTab(NodeBuilder statusBuilder) {
        statusBuilder
            .Dummy(10.0f)
            .TextCentered(Strings.GenericDataNode_ModuleStatus, 14)
            .Separator()
            .Dummy(6.0f)
            .TextCentered("STATUS TEXT", 14, out statusNode)
            .Dummy(28.0f)
            .TextCentered(Strings.GenericDataNode_NextReset, 14)
            .Separator()
            .Dummy(6.0f)
            .TextCentered("NEXT RESET", 14, out nextResetDateNode)
            .Dummy(28.0f)
            .TextCentered(Strings.GenericDataNode_TimeRemaining, 14)
            .Separator()
            .Dummy(6.0f)
            .TextCentered("TIME REMAINING", 14, out nextResetTimeRemainingNode);

        if (module.GetWindowEndTime() is not null) {
            statusBuilder
                .Dummy(28.0f)
                .TextCentered(Strings.GenericDataNode_AvailableUntil, 14)
                .Separator()
                .Dummy(6.0f)
                .TextCentered("AVAILABLE UNTIL", 14, out windowEndDateNode)
                .Dummy(28.0f)
                .TextCentered(Strings.GenericDataNode_TimeRemaining, 14)
                .Separator()
                .Dummy(6.0f)
                .TextCentered("REMAINING", 14, out windowEndTimeRemainingNode);
        }
    }

    private void BuildDataTab(NodeBuilder dataBuilder)
        => dataBuilder
            .Dummy(10.0f)
            .TextCentered(Strings.DataNodeBase_ModuleData, 14)
            .Separator()
            .Dummy(5.0f)
            .BuildAction(module.BuildDataUi);

    private void BuildConfigTab(NodeBuilder configBuilder) {

        // If it's IConfigurable then it has its own options it wants to present, so make a tab bar.
        if (module is IConfigurable configurable) {
            configBuilder
                .TabbedContent(tabBuilder => tabBuilder
                    .AddTab(Strings.NotificationSettingsNode_NotificationSettings, builder => builder
                        .Spacer(10.0f)
                        .BuildAction(module.BuildGenericConfig))
                    .AddTab(Strings.ConfigNodeBase_ModuleSettings, builder => builder
                        .Spacer(10.0f)
                        .TextCentered(Strings.ConfigNodeBase_ModuleSettings, 14)
                        .Separator()
                        .VerticalList(configurable.BuildConfigUi, node => {
                            node.FirstItemSpacing = 5.0f;
                            node.ItemSpacing = 4.0f;
                        })));
        }

        // If it isn't then don't bother with the tab bar, just add the stuff!
        else {
            module.BuildGenericConfig(configBuilder);
        }
    }
}
