using Resources;
using DailyDuty.Classes;
using DailyDuty.CustomNodes;
using KamiToolKit;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.WondrousTails;

public class WondrousTailsDataNode(WondrousTails module) : DataNodeBase<WondrousTails>(module) {
    private readonly WondrousTails module = module;

    private TextNode? hasBook;
    private TextNode? deadline;
    private TextNode? secondChance;
    private TextNode? placedStickers;
    private TextNode? newBookAvailable;
    private TextNode? isExpired;

    protected override NodeBase BuildDataNode() => new VerticalListNode {
        FitWidth = true,
        InitialNodes = [
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.ResourceManager.GetString("Book Obtained", Strings.Culture) ?? "Book Obtained",
                    },
                    hasBook = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.ResourceManager.GetString("Deadline", Strings.Culture) ?? "Deadline",
                    },
                    deadline = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.ResourceManager.GetString("Second Chance Points", Strings.Culture) ?? "Second Chance Points",
                    },
                    secondChance = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.ResourceManager.GetString("Placed Stickers", Strings.Culture) ?? "Placed Stickers",
                    },
                    placedStickers = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.ResourceManager.GetString("New Book Available", Strings.Culture) ?? "New Book Available",
                    },
                    newBookAvailable = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.ResourceManager.GetString("Book Expired", Strings.Culture) ?? "Book Expired",
                    },
                    isExpired = new TextNode(),
                ],
            },
        ],
    };

    public override void Update() {
        base.Update();

        var bookDeadline = module.Deadline.ToLocalTime();

        hasBook?.String = module.PlayerHasBook.ToString();
        deadline?.String = $"{bookDeadline.ToShortDateString()} {bookDeadline.ToShortTimeString()}";
        secondChance?.String = module.SecondChancePoints.ToString();
        placedStickers?.String = module.PlacedStickers.ToString();
        newBookAvailable?.String = module.IsNewBookAvailable.ToString();
        isExpired?.String = module.IsBookExpired.ToString();
    }
}
