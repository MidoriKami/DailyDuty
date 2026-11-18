using System;
using System.Linq;
using DailyDuty.Enums;
using DailyDuty.Interfaces;
using DailyDuty.Nodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Components.Builders;
using KamiToolKit.Enums;
using KamiToolKit.Extensions;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Addons;

public class ConfigWindow : NativeAddon {
    private ConfigInfoNode? infoNode;

    private TextNode? selectOptionTextNode;

    private ListNode<IModule, ModuleListNode>? dailyModulesNode;
    private ListNode<IModule, ModuleListNode>? weeklyModulesNode;
    private ListNode<IModule, ModuleListNode>? otherModulesNode;
    private ListNode<IFeature, FeatureListNode>? featuresNode;

    protected override unsafe void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValueSpan) {
        NativeUiBuilder.Create(ContentSize, ContentStartPosition, buildAction => buildAction
            .Search(OnSearchUpdated)
            .RatioSplit([0.3f, 0.7f],
                configureSplitNode => {
                    configureSplitNode.FitContents = false;
                    configureSplitNode.LayoutFill = LayoutFill.Both;
                },
                leftSplit => leftSplit
                    .TabbedContent(tabBuilder => tabBuilder
                        .AddTab("Modules", BuildModulesTab)
                        .AddTab("Features", BuildFeaturesTab), tabChanged: OnTabChanged),
                rightSplit => rightSplit
                    .AddNode(infoNode = new ConfigInfoNode())))
        .AttachNode(this);

        selectOptionTextNode = new TextNode {
            Size = infoNode!.Size,
            AlignmentType = AlignmentType.Center,
            LayoutFill = LayoutFill.Both,
            String = Strings.ModuleBrowserWindow_SelectOption,
        };
        selectOptionTextNode.AttachNode(infoNode);

        base.OnSetup(addon, atkValueSpan);
    }

    protected override unsafe void OnUpdate(AtkUnitBase* addon) {
        base.OnUpdate(addon);

        dailyModulesNode?.Update();
        weeklyModulesNode?.Update();
        otherModulesNode?.Update();
        featuresNode?.Update();

        infoNode?.Update();
    }

    protected override unsafe void OnFinalize(AtkUnitBase* addon) {
        base.OnFinalize(addon);

        selectOptionTextNode = null;

        dailyModulesNode = null;
        weeklyModulesNode = null;
        otherModulesNode = null;
        featuresNode = null;

        infoNode = null;
    }

    private void ClearSelection() {
        dailyModulesNode?.ClearSelection();
        weeklyModulesNode?.ClearSelection();
        otherModulesNode?.ClearSelection();
        featuresNode?.ClearSelection();

        infoNode?.Clear();

        selectOptionTextNode?.IsVisible = true;
    }

    private void OnTabChanged() => ClearSelection();

    private void BuildModulesTab(NodeBuilder modulesTab) => modulesTab
        .TabbedContent(modulesTabBuilder => modulesTabBuilder
            .AddTab("Daily", dailyTabBuilder => dailyTabBuilder
                .Spacer(6.0f)
                .List(System.Controller.DailyModules, OnModuleSelected, out dailyModulesNode, configureList => {
                    configureList.ItemSpacing = 4.0f;
                    configureList.OnItemStateChanged += OnModuleStateChanged;
                }))
            .AddTab("Weekly", weeklyTabBuilder => weeklyTabBuilder
                .Spacer(6.0f)
                .List(System.Controller.WeeklyModules, OnModuleSelected, out weeklyModulesNode, configureList => {
                    configureList.ItemSpacing = 4.0f;
                    configureList.OnItemStateChanged += OnModuleStateChanged;
                }))
            .AddTab("Other", otherTabBuilder => otherTabBuilder
                .Spacer(6.0f)
                .List(System.Controller.OtherModules, OnModuleSelected, out otherModulesNode, configureList => {
                    configureList.ItemSpacing = 4.0f;
                    configureList.OnItemStateChanged += OnModuleStateChanged;
                })), tabChanged: OnTabChanged);

    private void BuildFeaturesTab(NodeBuilder featuresBuilder) => featuresBuilder
        .Spacer(6.0f)
        .List(System.Controller.Features, OnFeatureSelected, out featuresNode, configureList => {
            configureList.ItemSpacing = 4.0f;
            configureList.OnItemStateChanged += OnFeatureStateChanged;
        });

    private void OnSearchUpdated(ReadOnlySeString searchString) {
        var regex = searchString.AsRegex();

        dailyModulesNode?.Options = [
            .. System.Controller.DailyModules.Where(module => regex.IsMatch(module.DisplayName)),
        ];

        weeklyModulesNode?.Options = [
            .. System.Controller.WeeklyModules.Where(module => regex.IsMatch(module.DisplayName)),
        ];

        otherModulesNode?.Options = [
            .. System.Controller.OtherModules.Where(module => regex.IsMatch(module.DisplayName)),
        ];

        featuresNode?.Options = [
            .. System.Controller.Features.Where(feature => regex.IsMatch(feature.DisplayName)),
        ];
    }

    private void OnModuleSelected(IModule? module) {
        if (module is not null) {
            infoNode?.SetModuleInfo(module);
            selectOptionTextNode?.IsVisible = false;
        }
    }

    private void OnFeatureSelected(IFeature? feature) {
        if (feature is not null) {
            infoNode?.SetFeatureInfo(feature);
            selectOptionTextNode?.IsVisible = false;
        }
    }

    private void OnModuleStateChanged(ModuleListNode node, IModule module) {
        if (infoNode?.CurrentItem == module) {
            ClearSelection();
        }
        else if (module.LoadState is State.Enabled) {
            OnModuleSelected(module);
        }
    }

    private void OnFeatureStateChanged(FeatureListNode node, IFeature feature) {
        if (infoNode?.CurrentItem == feature) {
            ClearSelection();
        }
        else if (feature.LoadState is State.Enabled) {
            OnFeatureSelected(feature);
        }
    }
}
