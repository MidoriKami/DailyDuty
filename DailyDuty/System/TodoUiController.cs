using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyDuty.Models;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Atk;

namespace DailyDuty.System;

public unsafe class TodoUiController : IDisposable
{
    private static AtkUnitBase* AddonNamePlate => (AtkUnitBase*) Service.GameGui.GetAddonByName("NamePlate");

    private readonly ResNode rootNode;

    private const uint ContainerNodeId = 1000;
    private const uint BackgroundImageBaseId = 5000;
    private const uint ExtrasBaseId = 6000;
    public const float EdgeSize = 10f;

    private readonly Dictionary<ModuleType, TodoUiCategoryController> categories = new();
    private readonly ImageNode backgroundImageNode;
    private readonly TextNode previewModeTextNode;

    public TodoUiController()
    {
        rootNode = new ResNode(new ResNodeOptions
        {
            Id = ContainerNodeId,
            Position = new Vector2(2560 / 2.0f, 200),
            Size = new Vector2(200.0f, 200.0f),
        });
        
        Node.LinkNodeAtStart(rootNode.ResourceNode, AddonNamePlate);
        
        backgroundImageNode = new ImageNode(new ImageNodeOptions
        {
            Id = BackgroundImageBaseId,
            Color = new Vector4(1.0f, 1.0f, 1.0f, 0.25f),
        });

        rootNode.AddResourceNode(backgroundImageNode, AddonNamePlate);

        previewModeTextNode = new TextNode(new TextNodeOptions
        {
            Id = ExtrasBaseId,
            Alignment = AlignmentType.Center,
            Flags = TextFlags.Edge,
            Type = NodeType.Text,
            BackgroundColor = KnownColor.Black.AsVector4(),
            EdgeColor = KnownColor.Black.AsVector4(),
            TextColor = KnownColor.OrangeRed.AsVector4(),
            FontSize = 16
        });
        previewModeTextNode.Node->SetText("Preview Mode is Enabled");
        rootNode.AddResourceNode(previewModeTextNode, AddonNamePlate);

        foreach (var category in Enum.GetValues<ModuleType>())
        {
            var newCategory = new TodoUiCategoryController(rootNode, category);
            categories.Add(category, newCategory);
        }
        
        AddonNamePlate->UpdateCollisionNodeList(false);
    }
    
    public void Dispose()
    {
        if (AddonNamePlate is not null)
        {
            Node.UnlinkNodeAtStart(rootNode.ResourceNode, AddonNamePlate);        
            AddonNamePlate->UpdateCollisionNodeList(false);
        }

        rootNode.Dispose();
        backgroundImageNode.Dispose();
        previewModeTextNode.Dispose();

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
    
    private void UpdatePositions(TodoConfig config)
    {
        rootNode.ResourceNode->SetPositionFloat(config.Position.X, config.Position.Y);
        
        var cumulativeSize = 0;
        var padding = config.CategorySpacing;

        ushort largestWidth = 0;
        var anyVisible = false;
        
        foreach (var category in categories)
        {
            if (category.Value.GetHeaderNode().ResourceNode->Width > largestWidth) largestWidth = category.Value.GetHeaderNode().ResourceNode->Width;

            var resNode = category.Value.GetCategoryContainer().ResourceNode;

            if (resNode->IsVisible)
            {
                anyVisible = true;
                var xPos = config.RightAlign ? rootNode.ResourceNode->Width - resNode->Width : 0.0f;
                
                resNode->SetPositionFloat(xPos, cumulativeSize);
                cumulativeSize += resNode->GetHeight() + padding;
                if (resNode->Width > largestWidth) largestWidth = resNode->Width;
            }
        }

        var finalHeight = (ushort) (cumulativeSize - config.CategorySpacing);
        
        rootNode.ResourceNode->SetPositionFloat(
            config.Position.X - (config.Anchor.HasFlag(WindowAnchor.TopRight) ? largestWidth : 0),
            config.Position.Y - (config.Anchor.HasFlag(WindowAnchor.BottomLeft) ? finalHeight : 0)
        );
        rootNode.ResourceNode->SetHeight(finalHeight);
        rootNode.ResourceNode->SetWidth(largestWidth);
        
        backgroundImageNode.ResourceNode->ToggleVisibility(config.BackgroundImage && anyVisible);
        backgroundImageNode.ResourceNode->SetPositionFloat(-EdgeSize, -EdgeSize);
        backgroundImageNode.ResourceNode->SetHeight((ushort)(finalHeight + EdgeSize * 2));
        backgroundImageNode.ResourceNode->SetWidth((ushort)(largestWidth + EdgeSize * 2));
        
        previewModeTextNode.Node->AtkResNode.ToggleVisibility(config.PreviewMode);
        previewModeTextNode.ResourceNode->SetWidth(largestWidth);
        previewModeTextNode.ResourceNode->SetPositionFloat(0.0f, -24.0f);
    }
    
    public void UpdateCategoryStyle(ModuleType type, ImageNodeOptions options)
    {
        options.Id = BackgroundImageBaseId;
        backgroundImageNode.UpdateOptions(options);

        backgroundImageNode.ResourceNode->Color.A = (byte) (options.Color.W * 255);
        backgroundImageNode.ResourceNode->AddRed = (byte) (options.Color.X * 255);
        backgroundImageNode.ResourceNode->AddGreen = (byte) (options.Color.Y * 255);
        backgroundImageNode.ResourceNode->AddBlue = (byte) (options.Color.Z * 255);
    }

    public void UpdateModule(ModuleType type, ModuleName module, string label, string tooltip, bool visible) => categories[type].UpdateModule(module, label, tooltip, visible);
    public void UpdateModuleStyle(ModuleType type, ModuleName module, TextNodeOptions options) => categories[type].UpdateModuleStyle(module, options);
    public void UpdateCategoryHeader(ModuleType type, string label, bool show) => categories[type].UpdateCategoryHeader(label, show);
    public void UpdateHeaderStyle(ModuleType type, TextNodeOptions options) => categories[type].UpdateHeaderStyle(options);
    public void UpdateCategory(ModuleType type, bool enabled) => categories[type].SetVisible(enabled);
    public void Show(bool visible) => rootNode.ResourceNode->ToggleVisibility(visible);
    public void Hide() => rootNode.ResourceNode->ToggleVisibility(false);
    public Vector2 GetSize() => new(rootNode.ResourceNode->Width, rootNode.ResourceNode->Height);
}