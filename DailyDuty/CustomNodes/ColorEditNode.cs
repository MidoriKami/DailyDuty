using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Addons;
using KamiToolKit.Premade.Nodes;
using Lumina.Text.ReadOnly;

namespace DailyDuty.CustomNodes;

public class ColorEditNode : SimpleOverlayNode {

    private readonly ColorPreviewNode previewNode;
    private readonly TextNode labelNode;
    
    private ColorPickerAddon? colorPicker = new() {
        InternalName = "ColorPicker",
        Title = "Color Picker",
    };

    public ColorEditNode() {
        DisableCollisionNode = true;

        previewNode = new ColorPreviewNode();
        previewNode.AttachNode(this);

        labelNode = new TextNode {
            AlignmentType = AlignmentType.Left,
        };
        labelNode.AttachNode(this);

        previewNode.CollisionNode.ShowClickableCursor = true;
        previewNode.CollisionNode.AddEvent(AtkEventType.MouseClick, OnClicked);

        labelNode.ShowClickableCursor = true;
        labelNode.AddEvent(AtkEventType.MouseClick, OnClicked);
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        base.Dispose(disposing, isNativeDestructor);
        
        colorPicker?.Dispose();
        colorPicker = null;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        previewNode.Size = new Vector2(Height - 2.0f, Height - 2.0f);
        previewNode.Position = new Vector2(1.0f, 1.0f);

        labelNode.Size = new Vector2(Width - Height - 8.0f, Height);
        labelNode.Position = new Vector2(previewNode.Bounds.Right + 6.0f, 0.0f);
    }
    
    private void OnClicked() {
        var originalColor = CurrentColor;
        colorPicker?.DefaultColor = DefaultColor;
        colorPicker?.InitialColor = CurrentColor;
            
        colorPicker?.OnColorPreviewed = color => {
            previewNode.Color = color;
            CurrentColor = color;
            OnColorPreviewed?.Invoke(color);
        };

        colorPicker?.OnColorCancelled = () => {
            CurrentColor = originalColor;
            OnColorCancelled?.Invoke();
        };

        colorPicker?.OnColorConfirmed = color => {
            CurrentColor = color;
            OnColorConfirmed?.Invoke(color);
        };

        colorPicker?.Toggle();
    }

    public Vector4 CurrentColor {
        get => previewNode.Color;
        set => previewNode.Color = value;
    }

    public ReadOnlySeString Label {
        get => labelNode.SeString;
        set => labelNode.SeString = value;
    }

    public Vector4? DefaultColor { get; set; }
    
    public Action? OnColorCancelled { get; set; }
    public Action<Vector4>? OnColorPreviewed { get; set; }
    public Action<Vector4>? OnColorConfirmed { get; set; }
}
