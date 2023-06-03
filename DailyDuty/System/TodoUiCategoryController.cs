using System;
using System.Collections.Generic;
using System.Numerics;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Atk;

namespace DailyDuty.Models;

public unsafe class TodoUiCategoryController : IDisposable
{
    private const uint HeaderNodeBaseId = 2000;
    private const uint ModuleNodeBaseId = 3000;
    private const uint SubCategoryBaseId = 4000;
    
    private static AtkUnitBase* AddonNamePlate => (AtkUnitBase*) Service.GameGui.GetAddonByName("NamePlate");

    private readonly ModuleType moduleType;
    private readonly Dictionary<ModuleName, TextNode> moduleNodes = new();
    private readonly TextNode headerNode;
    private readonly ResNode categoryResNode;

    public TodoUiCategoryController(ResNode parent, ModuleType type)
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
        
        headerNode.Node->SetText(type.GetLabel());
        categoryResNode.AddResourceNode(headerNode, AddonNamePlate);

        foreach (var module in DailyDutySystem.ModuleController.GetModules(type))
        {
            var textNode = new TextNode(new TextNodeOptions
            {
                Id = ModuleNodeBaseId + (uint) module.ModuleName,
            });

            if (module.HasTooltip)
            {
                textNode.EnableTooltip(AddonNamePlate, module.ModuleName.GetLabel());
            }
            
            moduleNodes.Add(module.ModuleName, textNode);
            textNode.Node->SetText(module.ModuleName.GetLabel());
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
        
        categoryResNode.Dispose();
    }

    public void UpdatePositions(TodoConfig config)
    {
        ushort startPosition = (ushort) (headerNode.ResourceNode->IsVisible ? headerNode.ResourceNode->Height + config.HeaderSpacing : config.HeaderSpacing);
        
        var headerResNode = headerNode.ResourceNode;
        var headerPositionX = config.RightAlign ? categoryResNode.ResourceNode->Width - headerResNode->Width : 0.0f;
        headerResNode->SetPositionFloat(headerPositionX, 0.0f);

        ushort largestWidth = 0;
        var anyVisible = false;
        
        foreach (var module in moduleNodes)
        {
            var moduleResNode = module.Value.ResourceNode;

            if (moduleResNode->IsVisible)
            {
                var xPos = config.RightAlign ? categoryResNode.ResourceNode->Width - moduleResNode->Width : 0.0f;
                
                moduleResNode->SetPositionFloat(xPos, startPosition);
                startPosition += (ushort) (moduleResNode->GetHeight() + config.ModuleSpacing);
                anyVisible = true;
                if (moduleResNode->Width > largestWidth) largestWidth = moduleResNode->Width;
            }
        }
        
        categoryResNode.ResourceNode->SetHeight(startPosition);
        categoryResNode.ResourceNode->SetWidth(largestWidth);

        if(!anyVisible) categoryResNode.ResourceNode->ToggleVisibility(false);
    }
    
    public void UpdateModule(ModuleName module, string label, string tooltip, bool visible)
    {
        moduleNodes[module].Node->SetText(label);
        moduleNodes[module].Node->AtkResNode.ToggleVisibility(visible);
        moduleNodes[module].UpdateTooltip(visible ? tooltip : string.Empty);
    }
    
    public void UpdateCategoryHeader(string label, bool visible)
    {
        headerNode.Node->SetText(label);
        headerNode.Node->AtkResNode.ToggleVisibility(visible);
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
    
    public TextNode GetHeaderNode() => headerNode;
    public ResNode GetCategoryContainer() => categoryResNode;
    public void SetVisible(bool show) => categoryResNode.ResourceNode->ToggleVisibility(show);


}