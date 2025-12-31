using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Classes.Nodes;

public class WondrousTailsNode : SimpleComponentNode {

    private readonly ImageNode foreground;

	public WondrousTailsNode() {
        DisableCollisionNode = true;
        
        ImageNode background = new SimpleImageNode {
            Size = new Vector2(22.0f, 22.0f),
            Position = Vector2.One,
            TexturePath = "ui/uld/WeeklyBingo.tex",
            TextureCoordinates = new Vector2(74.0f, 62.0f),
            TextureSize = new Vector2(22.0f, 22.0f),
            IsVisible = true,
            WrapMode = WrapMode.Tile,
            ImageNodeFlags = 0,
        };

        background.ShowClickableCursor = true;
        background.AddFlags(NodeFlags.HasCollision, NodeFlags.IsTopNode);
        background.TextTooltip = "[DailyDuty] This duty is a Wondrous Tails task";
        
		background.AttachNode(this);
						
		foreground = new SimpleImageNode {
			Size = new Vector2(22.0f, 22.0f),
			TexturePath = "ui/uld/WeeklyBingo.tex",
			TextureCoordinates = new Vector2(95.0f, 63.0f),
			TextureSize = new Vector2(24.0f, 24.0f),
			WrapMode = WrapMode.Tile,
			ImageNodeFlags = 0,
		};
		foreground.AttachNode(this);
	}

	public bool IsTaskAvailable {
		get => foreground.IsVisible;
		set => foreground.IsVisible = value;
	}
}
