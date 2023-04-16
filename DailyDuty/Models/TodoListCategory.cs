using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Atk;

namespace DailyDuty.Models;

public unsafe class TodoListCategory : IDisposable
{
    private const uint HeaderNodeBaseId = 2000;
    private const uint ModuleNodeBaseId = 3000;
    private const uint SubCategoryBaseId = 4000;
    
    private static AtkUnitBase* AddonNamePlate => (AtkUnitBase*) Service.GameGui.GetAddonByName("NamePlate");

    private readonly ModuleType moduleType;
    private readonly Dictionary<ModuleName, TextNode> moduleNodes = new();
    private readonly TextNode headerNode;
    private readonly ResNode categoryResNode;

    public TodoListCategory(ResNode parent, ModuleType type)
    {
        moduleType = type;
        
        categoryResNode = new ResNode(new ResNodeOptions
        {
            Id = SubCategoryBaseId + (uint) type,
            Position = Vector2.Zero,
            Size = new Vector2(200.0f, 500.0f)
        });
        parent.AddResourceNode(categoryResNode, AddonNamePlate);

        headerNode = new TextNode(new TextNodeOptions
        {
            Id = HeaderNodeBaseId + (uint) type,
        });
        
        headerNode.SetText(type.GetLabel());
        categoryResNode.AddResourceNode(headerNode, AddonNamePlate);

        foreach (var module in DailyDutySystem.ModuleController.GetModules(type))
        {
            var textNode = new TextNode(new TextNodeOptions
            {
                Id = ModuleNodeBaseId + (uint) module.ModuleName,
            });
            
            moduleNodes.Add(module.ModuleName, textNode);
            textNode.SetText(module.ModuleName.GetLabel());
            categoryResNode.AddResourceNode(textNode, AddonNamePlate);
        }
    }

    public void Dispose()
    {
        headerNode.Dispose();

        foreach (var moduleNode in moduleNodes)
        {
            moduleNode.Value.Dispose();
        }
        
        moduleNodes.Clear();
        categoryResNode.Dispose();
    }

    public void UpdatePositions(TodoConfig config)
    {
        var startPosition = headerNode.GetResourceNode()->IsVisible ? headerNode.GetResourceNode()->Height : (ushort)0;
        
        var headerResNode = headerNode.GetResourceNode();
        var headerPositionX = config.RightAlign ? categoryResNode.GetResourceNode()->Width - headerResNode->Width : 0.0f;
        headerResNode->SetPositionFloat(headerPositionX, 0.0f);

        ushort largestWidth = 0; 
        
        foreach (var module in moduleNodes)
        {
            var moduleResNode = module.Value.GetResourceNode();
            if (moduleResNode->Width > largestWidth) largestWidth = moduleResNode->Width;

            if (moduleResNode->IsVisible)
            {
                var xPos = config.RightAlign ? categoryResNode.GetResourceNode()->Width - moduleResNode->Width : 0.0f;
                
                moduleResNode->SetPositionFloat(xPos, startPosition);
                startPosition += moduleResNode->GetHeight();
            }
        }
        
        categoryResNode.GetResourceNode()->SetHeight(startPosition);
        categoryResNode.GetResourceNode()->SetWidth(largestWidth);

    }
    
    public void UpdateModule(ModuleName module, string label, bool visible)
    {
        moduleNodes[module].SetText(label);
        moduleNodes[module].SetVisible(visible);
    }

    public void UpdateCategoryHeader(string label, bool visible)
    {
        headerNode.SetText(label);
        headerNode.SetVisible(visible);
    }

    public void UpdateModuleStyle(ModuleName module, TextNodeOptions options)
    {
        options.Id = ModuleNodeBaseId + (uint) module;
        
        moduleNodes[module].UpdateOptions(options);
    }

    public void UpdateHeaderStyle(TextNodeOptions options)
    {
        options.Id = HeaderNodeBaseId + (uint) moduleType;
        headerNode.UpdateOptions(options);
    }
    
    public ResNode GetCategoryContainer() => categoryResNode;
    public void SetVisible(bool show) => categoryResNode.SetVisibility(show);

}