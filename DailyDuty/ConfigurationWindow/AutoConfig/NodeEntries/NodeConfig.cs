using System.Numerics;
using DailyDuty.ConfigurationWindow.AutoConfig.ConfigEntries;
using KamiToolKit;
using KamiToolKit.Nodes;

namespace DailyDuty.ConfigurationWindow.AutoConfig.NodeEntries;

public class NodeConfig<T> : IConfigEntry where T : NodeStyle, new() {

    public required T StyleObject { get; init; }

    private const float ElementStartOffset = 100.0f;

    public NodeBase BuildNode() {
        var layoutNode = new VerticalListNode {
            Height = 24.0f,
            FitContents = true,
            FitWidth = true,
        };
    
        BuildOptions(layoutNode);
    
        return layoutNode;
    }
    
    protected virtual void BuildOptions(VerticalListNode container) {
        container.AddNode(BuildPositionEdit());
    }

    private LabelLayoutNode BuildPositionEdit() {
        var container = new LabelLayoutNode {
            Height = 28.0f,
            FillWidth = true,
        };

        var labelNode = new LabelTextNode {
            String = "Position",
            Size = new Vector2(ElementStartOffset, 28.0f),
        };
        container.AddNode(labelNode);

        var xPosition = new NumericInputNode {
            Height = 28.0f,
            Value = (int) StyleObject.Position.X,
            Min = int.MinValue,
            OnValueUpdate = newValue => {
                StyleObject.Position = StyleObject.Position with { X = newValue };
                SaveStyleObject();
            },
        };
        container.AddNode(xPosition);

        var yPosition = new NumericInputNode {
            Height = 28.0f,
            Min = int.MinValue,
            Value = (int) StyleObject.Position.Y,
            OnValueUpdate = newValue => {
                StyleObject.Position = StyleObject.Position with { Y = newValue };
                SaveStyleObject();
            },
        };
        container.AddNode(yPosition);

        return container;
    }
    
    protected void SaveStyleObject() {
        StyleObject.StyleChanged?.Invoke();
    }

    public virtual void Dispose() { }
}
