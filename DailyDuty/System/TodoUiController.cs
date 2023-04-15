using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Models;
using DailyDuty.Models.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Atk;

namespace DailyDuty.System;

public unsafe class TodoUiController : IDisposable
{
    private static AtkUnitBase* AddonNamePlate => (AtkUnitBase*) Service.GameGui.GetAddonByName("NamePlate");

    private readonly ResNode rootNode;

    private const uint ContainerNodeId = 1000;

    private readonly Dictionary<ModuleType, TodoListCategory> categories = new();

    public TodoUiController()
    {
        rootNode = new ResNode(new ResNodeOptions
        {
            Id = ContainerNodeId,
            Position = new Vector2(2560 / 2.0f, 200),
            Size = new Vector2(200.0f, 200.0f),
        });
        
        Node.LinkNodeAtStart(rootNode.GetResourceNode(), AddonNamePlate);

        foreach (var category in Enum.GetValues<ModuleType>())
        {
            var newCategory = new TodoListCategory(rootNode, category);
            categories.Add(category, newCategory);
        }
    }
    
    public void Dispose()
    {
        Node.UnlinkNodeAtStart(rootNode.GetResourceNode(), AddonNamePlate);
        rootNode.Dispose();

        foreach (var category in categories)
        {
            category.Value.Dispose();
        }
        
        categories.Clear();
    }

    public void Update(TodoConfig config)
    {
        foreach (var category in categories)
        {
            category.Value.UpdatePositions(config);
        }

        UpdatePositions(config);
    }

    public void UpdateModule(ModuleType type, ModuleName module, string label, bool visible) => categories[type].UpdateModule(module, label, visible);
    public void UpdateModuleStyle(ModuleType type, ModuleName module, TextNodeOptions options) => categories[type].UpdateModuleStyle(module, options);
    public void UpdateCategoryHeader(ModuleType type, string label, bool show) => categories[type].UpdateCategoryHeader(label, show);
    public void UpdateHeaderStyle(ModuleType type, TextNodeOptions options) => categories[type].UpdateHeaderStyle(options);
    public void UpdateCategory(ModuleType type, bool enabled) => categories[type].SetVisible(enabled);

    private void UpdatePositions(TodoConfig config)
    {
        rootNode.GetResourceNode()->SetPositionFloat(config.Position.X, config.Position.Y);
        
        var cumulativeSize = 0;
        var padding = config.CategorySpacing;
        
        foreach (var category in categories)
        {
            var resNode = category.Value.GetCategoryContainer().GetResourceNode();

            if (resNode->IsVisible)
            {
                resNode->SetPositionFloat(0.0f, cumulativeSize);
                cumulativeSize += resNode->GetHeight() + padding;
            }
        }
        
        rootNode.GetResourceNode()->SetHeight((ushort) cumulativeSize);
    }
    
    public void Show(bool visible) => rootNode.SetVisibility(visible);

    public void Hide() => rootNode.SetVisibility(false);
}