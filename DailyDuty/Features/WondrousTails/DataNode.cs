using DailyDuty.Classes.Nodes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.WondrousTails;

public class DataNode(WondrousTails module) : DataNodeBase<WondrousTails>(module) {
    private readonly WondrousTails module = module;

    private TextNode? hasBook;
    private TextNode? deadline;
    private TextNode? secondChance;
    private TextNode? placedStickers;
    private TextNode? newBookAvailable;
    private TextNode? isExpired;

    protected override void BuildNode(VerticalListNode container) {
        container.AddNode([
            new HorizontalFlexNode {
                Height = 28.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "Book Obtained",
                        AlignmentType = AlignmentType.Left,
                    },
                    hasBook = new TextNode {
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 28.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "Deadline",
                        AlignmentType = AlignmentType.Left,
                    },
                    deadline = new TextNode {
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 28.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "Second Chance Points",
                        AlignmentType = AlignmentType.Left,
                    },
                    secondChance = new TextNode {
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 28.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "Placed Stickers",
                        AlignmentType = AlignmentType.Left,
                    },
                    placedStickers = new TextNode {
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 28.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "New Book Available",
                        AlignmentType = AlignmentType.Left,
                    },
                    newBookAvailable = new TextNode {
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
            new HorizontalFlexNode {
                Height = 28.0f,
                AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                InitialNodes = [
                    new TextNode {
                        String = "Book Expired",
                        AlignmentType = AlignmentType.Left,
                    },
                    isExpired = new TextNode {
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
        ]);
    }

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
