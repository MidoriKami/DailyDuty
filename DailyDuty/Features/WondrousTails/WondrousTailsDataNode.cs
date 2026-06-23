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
                        String = Strings.WondrousTails_BookObtained,
                    },
                    hasBook = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.WondrousTails_Deadline,
                    },
                    deadline = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.WondrousTails_SecondChancePoints,
                    },
                    secondChance = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.WondrousTails_PlacedStickers,
                    },
                    placedStickers = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.WondrousTails_NewBookAvailable,
                    },
                    newBookAvailable = new TextNode(),
                ],
            },
            new HorizontalFlexNode {
                Height = 32.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = Strings.WondrousTails_BookExpired,
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
