using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Windows;

public class ModuleBrowserWindow : NativeAddon {
    private LayoutListNode? layoutNode;

    private ResNode? optionContainerNode;
    private TextNode? selectOptionLabelNode;

    private Dictionary<LoadedModule, NodeBase>? moduleNodes;

    private Dictionary<ReadOnlySeString, List<LoadedModule>>? allModuleOptions;

    protected override unsafe void OnSetup(AtkUnitBase* addon, Span<AtkValue> _) {
        moduleNodes = [];

        allModuleOptions = [];

        var modules = System.ModuleManager.LoadedModules ?? [];

        foreach (var moduleGroup in modules.GroupBy(module => module.FeatureBase.ModuleInfo.Type)) {
            allModuleOptions.TryAdd(moduleGroup.Key.Description, []);

            foreach (var loadedModule in moduleGroup) {
                allModuleOptions[moduleGroup.Key.Description].Add(loadedModule);
            }
        }

        layoutNode = new VerticalListNode {
            Position = ContentStartPosition,
            Size = ContentSize,
            FitWidth = true,
            InitialNodes = [
                new SearchNode {
                    Height = 28.0f,
                    OnSearchUpdated = OnSearchUpdated,
                },
                new HorizontalListNode {
                    Height = ContentSize.Y - 28.0f,
                    FitHeight = true,
                    InitialNodes = [
                        new TreeListNode<LoadedModule, ModuleOptionNode> {
                            Width = ContentSize.X * 3.85f / 10.0f,
                            Options = allModuleOptions,
                            OnItemSelected = OnModuleSelected,
                        },
                        optionContainerNode = new ResNode {
                            Width = ContentSize.X * 6.15f / 10.0f,
                        },
                    ],
                },
            ],
        };
        layoutNode.AttachNode(this);

        selectOptionLabelNode = new TextNode {
            Size = optionContainerNode.Size,
            FontSize = 14,
            String = Strings.ModuleBrowserWindow_SelectOption,
            AlignmentType = AlignmentType.Center,
        };
        selectOptionLabelNode.AttachNode(optionContainerNode);

        foreach (var module in modules) {
            var displayNode = module.FeatureBase.DisplayNode;
            displayNode.IsVisible = false;
            displayNode.Size = optionContainerNode.Size;

            displayNode.AttachNode(optionContainerNode);
            moduleNodes.TryAdd(module, displayNode);
        }
    }

    protected override unsafe void OnFinalize(AtkUnitBase* addon) {
        base.OnFinalize(addon);

        layoutNode = null;

        optionContainerNode = null;

        moduleNodes?.Clear();
        moduleNodes = null;
    }

    private void OnModuleSelected(LoadedModule? obj) {
        foreach (var (_, node) in moduleNodes ?? []) {
            node.IsVisible = false;
        }

        selectOptionLabelNode?.IsVisible = obj is null;

        if (obj is not null && (moduleNodes?.TryGetValue(obj, out var moduleNode) ?? false)) {
            moduleNode.IsVisible = true;
        }
    }

    protected override unsafe void OnUpdate(AtkUnitBase* addon) {
        if (System.ModuleManager.IsUnloading) return;

        foreach (var (_, node) in moduleNodes ?? []) {
            if (node is UpdatableNode { IsVisible: true } updatableNode) {
                updatableNode.Update();
            }
        }
    }

    private void OnSearchUpdated(ReadOnlySeString searchTerm) {
        // List<ModuleOptionNode> validOptions = [];

        // foreach (var node in optionsNode?.Nodes ?? []) {
        //     var isTarget = node.ModuleInfo.IsMatch(searchTerm.ToString());
        //     node.IsVisible = isTarget;
        //
        //     if (isTarget) {
        //         validOptions.Add(node);
        //     }
        // }
        //
        // foreach (var categoryNode in optionsNode?.CategoryNodes ?? []) {
        //     categoryNode.IsVisible = validOptions.Any(option => option.ModuleInfo.Type.Description == categoryNode.String.ToString());
        //     categoryNode.RecalculateLayout();
        // }
        //
        // if (validOptions.All(option => option != selectedOption)) {
        //     UnselectCurrentOption();
        // }
        //
        // optionsNode?.RecalculateLayout();
    }

    // private void OnOptionClicked(ModuleOptionNode option) {
    //     // // if (mainContainerNode is null) return;
    //     // if (selectedOption == option) return;
    //     //
    //     // UnselectCurrentOption();
    //     //
    //     // selectedOption = option;
    //     // selectedOption.IsSelected = true;
    //
    //     // if (option.Module.FeatureBase is ModuleBase module) {
    //     //     AttachStatusNode(module.DataNode);
    //     // }
    //     // else if (option.Module.FeatureBase is not ModuleBase) {
    //     //     AttachFeatureNode(option.Module.FeatureBase.DisplayNode);
    //     // }
    // }

    // private void AttachFeatureNode(NodeBase node) {
    //     if (contentsNode is null) return;
    //
    //     SelectOptionNode(node);
    //
    //     node.Size = contentsNode.Size;
    //     node.AttachNode(contentsNode);
    //
    //     if (node is LayoutListNode layoutListNode) {
    //         layoutListNode.RecalculateLayout();
    //     }
    // }

    // private void OnCategoryToggled(bool isVisible, ModuleType category) {
    //     // UnselectCurrentOption();
    //     // optionsNode?.RecalculateLayout();
    // }

    // private void AttachStatusNode(DataNodeBase node) {
    //     if (contentsNode is null) return;
    //
    //     SelectOptionNode(node);
    //
    //     node.SelectTab(dataTabSelected);
    //
    //     node.TabSelected = tab => {
    //         dataTabSelected = tab;
    //     };
    //
    //     node.Size = contentsNode.Size;
    //     node.AttachNode(contentsNode);
    // }

    // private void SelectOptionNode(NodeBase option) {
    //     statusNode?.Dispose();
    //     statusNode = option;
    //     selectOptionLabelNode?.IsVisible = false;
    // }

    // private void UnselectCurrentOption() {
    //     // selectedOption?.IsSelected = false;
    //     // selectedOption?.IsHovered = false;
    //     // selectedOption = null;
    //
    //     // statusNode?.Dispose();
    //     // statusNode = null;
    //     //
    //     // selectOptionLabelNode?.IsVisible = true;
    // }
}
