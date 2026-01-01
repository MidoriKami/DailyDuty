using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Windows;

public class ModuleBrowserWindow : NativeAddon {
    private SearchNode? searchNode;
    private OptionsNode? optionsNode;
    private NodeBase? statusNode;
    private SimpleComponentNode? mainContainerNode;
    private SimpleComponentNode? contentsNode;
    private TextNode? selectOptionLabelNode;

    private ModuleOptionNode? selectedOption;

    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        statusNode = null;
        
        mainContainerNode = new SimpleComponentNode {
            Position = ContentStartPosition,
            Size = ContentSize,
        };
        mainContainerNode.AttachNode(this);
        
        searchNode = new SearchNode {
            Size = new Vector2(mainContainerNode.Width, 28.0f),
            OnSearchUpdated = OnSearchUpdated,
        };
        
        optionsNode = new OptionsNode {
            Position = new Vector2(0.0f, searchNode.Bounds.Bottom + 4.0f),
            Size = new Vector2(mainContainerNode.Width * 2.0f / 5.0f - 8.0f, mainContainerNode.Height - searchNode.Bounds.Bottom - 4.0f),
            OptionClicked = OnOptionClicked,
            CategoryToggled = OnCategoryToggled,
        };
        
        optionsNode.AttachNode(mainContainerNode);
        searchNode.AttachNode(mainContainerNode);
        
        optionsNode.SetOptions(System.ModuleManager.LoadedModules);
        
        contentsNode = new SimpleComponentNode {
            Size = new Vector2(mainContainerNode.Width * 3.0f / 5.0f - 8.0f, mainContainerNode.Height - searchNode.Bounds.Bottom - 8.0f),
            Position = new Vector2(mainContainerNode.Width * 2.0f / 5.0f + 4.0f, searchNode.Bounds.Bottom + 4.0f),
        };
        contentsNode.AttachNode(mainContainerNode);
        
        selectOptionLabelNode = new TextNode {
            Size = contentsNode.Size,
            FontSize = 14,
            String = "Please select an option on the left",
            AlignmentType = AlignmentType.Center,
        };
        selectOptionLabelNode.AttachNode(contentsNode);
    }

    protected override unsafe void OnUpdate(AtkUnitBase* addon) {
        if (System.ModuleManager.IsUnloading) return;
        
        foreach (var node in optionsNode?.Nodes ?? []) {
            node.Update();
        }

        if (statusNode is UpdatableNode updatableNode) {
            updatableNode.Update();
        }
    }

    private void OnSearchUpdated(ReadOnlySeString searchTerm) {
        List<ModuleOptionNode> validOptions = [];
        
        foreach (var node in optionsNode?.Nodes ?? []) {
            var isTarget = node.ModuleInfo.IsMatch(searchTerm.ToString());
            node.IsVisible = isTarget;
            
            if (isTarget) {
                validOptions.Add(node);
            }
        }
        
        foreach (var categoryNode in optionsNode?.CategoryNodes ?? []) {
            categoryNode.IsVisible = validOptions.Any(option => option.ModuleInfo.Type.Description == categoryNode.SeString.ToString());
            categoryNode.RecalculateLayout();
        }

        if (validOptions.All(option => option != selectedOption)) {
            UnselectCurrentOption();
        }

        optionsNode?.RecalculateLayout();
    }

    private void OnOptionClicked(ModuleOptionNode option) {
        if (mainContainerNode is null) return;
        if (selectedOption == option) return;
        
        UnselectCurrentOption();

        selectedOption = option;
        selectedOption.IsSelected = true;
        
        if (option.Module.FeatureBase is ModuleBase module) {
            AttachStatusNode(module.DataNode);
        }
        else if (option.Module.FeatureBase is not ModuleBase) {
            AttachFeatureNode(option.Module.FeatureBase.DisplayNode);
        }
    }

    private void AttachFeatureNode(NodeBase node) {
        if (contentsNode is null) return;

        SelectOptionNode(node);

        node.Size = contentsNode.Size;
        node.AttachNode(contentsNode);

        if (node is LayoutListNode layoutListNode) {
            layoutListNode.RecalculateLayout();
        }
    }

    private void OnCategoryToggled(bool isVisible, ModuleType category) {
        UnselectCurrentOption();
        optionsNode?.RecalculateLayout();
    }

    private void AttachStatusNode(DataNodeBase node) {
        if (contentsNode is null) return;
        
        SelectOptionNode(node);

        node.Size = contentsNode.Size;
        node.AttachNode(contentsNode);
    }

    private void SelectOptionNode(NodeBase option) {
        statusNode?.Dispose();
        statusNode = option;
        selectOptionLabelNode?.IsVisible = false;
    }
    
    private void UnselectCurrentOption() {
        selectedOption?.IsSelected = false;
        selectedOption?.IsHovered = false;
        selectedOption = null;
        
        statusNode?.Dispose();
        statusNode = null;

        selectOptionLabelNode?.IsVisible = true;
    }
}
