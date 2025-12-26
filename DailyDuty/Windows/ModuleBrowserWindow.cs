using System.Numerics;
using DailyDuty.Classes.Nodes;
using DailyDuty.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.Windows;

public class ModuleBrowserWindow : NativeAddon {
    private SearchNode? searchNode;
    private OptionsNode? optionsNode;
    private DataNodeBase? statusNode;
    private SimpleComponentNode? mainContainerNode;

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
    }

    protected override unsafe void OnUpdate(AtkUnitBase* addon) {
        if (System.ModuleManager.IsUnloading) return;
        
        foreach (var node in optionsNode?.Nodes ?? []) {
            node.Update();
        }

        statusNode?.Update();
    }

    private void OnSearchUpdated(ReadOnlySeString obj) {
        
    }

    private void OnOptionClicked(ModuleOptionNode option) {
        if (selectedOption == option) return;
        
        selectedOption?.IsSelected = false;
        selectedOption?.IsHovered = false;

        selectedOption = option;
        selectedOption.IsSelected = true;
        
        statusNode?.Dispose();
        
        var statusDisplayNode = option.Module.ModuleBase.GetDataNode();
        
        AttachStatusNode(statusDisplayNode);
    }
    
    private void OnCategoryToggled(bool arg1, ModuleType arg2) {
        
    }

    private void AttachStatusNode(DataNodeBase node) {
        if (mainContainerNode is null) return;
        if (searchNode is null) return;
        
        statusNode?.Dispose();
        statusNode = node;
        
        node.Size = new Vector2(mainContainerNode.Width * 3.0f / 5.0f - 8.0f, mainContainerNode.Height - searchNode.Bounds.Bottom - 8.0f);
        node.Position = new Vector2(mainContainerNode.Width * 2.0f / 5.0f + 4.0f, searchNode.Bounds.Bottom + 4.0f);
        node.AttachNode(mainContainerNode);
    }
}
