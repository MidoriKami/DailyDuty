using System;
using DailyDuty.Interfaces;
using KamiToolKit.Components.Builders;
using KamiToolKit.Nodes;

namespace DailyDuty.Nodes;

public class ModuleInfoNode : ResNode, INativeUpdatable {
    private readonly IModule module;

    private readonly VerticalListNode layoutContainer;

    public ModuleInfoNode(IModule module) {
        this.module = module;

        layoutContainer = NativeUiBuilder.Create(buildAction => buildAction
            .TextCentered(module.Name, 14)
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
        layoutContainer.RecalculateLayout();
    }

    public void Update() {
        statusNode.String = module.GetStatus().ToString();
        nextResetNode.String = $"{module.NextResetTime.ToLocalTime():dddd, MMMM dd, yyyy hh:mm tt}";
        timeRemainingNode.String = $"{module.NextResetTime.ToUniversalTime() - DateTime.UtcNow:d\\.hh\\:mm\\:ss}";

        module.UpdateDataUi();

        if (module is IUpdatingConfigurable updatingConfigurable) {
            updatingConfigurable.UpdateConfigUi();
        }
    }

    private TextNode statusNode = null!;
    private TextNode nextResetNode = null!;
    private TextNode timeRemainingNode = null!;

    private void BuildStatusTab(NodeBuilder statusBuilder) => statusBuilder
        .Dummy(10.0f)
        .TextCentered(Strings.GenericDataNode_ModuleStatus, 14)
        .Separator()
        .Dummy(6.0f)
        .TextCentered("STATUS TEXT", 14, out statusNode)
        .Dummy(28.0f)
        .TextCentered(Strings.GenericDataNode_NextReset, 14)
        .Separator()
        .Dummy(6.0f)
        .TextCentered("NEXT RESET", 14, out nextResetNode)
        .Dummy(28.0f)
        .TextCentered(Strings.GenericDataNode_TimeRemaining, 14)
        .Separator()
        .Dummy(6.0f)
        .TextCentered("TIME REMAINING", 14, out timeRemainingNode);

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
                    .AddTab(Strings.NotificationSettingsNode_NotificationSettings, builder => {
                        builder.Spacer(10.0f);
                        module.BuildGenericConfig(builder);
                    })
                    .AddTab(Strings.ConfigNodeBase_ModuleSettings, builder => builder
                        .Spacer(10.0f)
                        .TextCentered(Strings.ConfigNodeBase_ModuleSettings, 14)
                        .Separator()
                        .VerticalList(innerBuilder => configurable.BuildConfigUi(innerBuilder), node => {
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
