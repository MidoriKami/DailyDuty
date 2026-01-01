using System;
using System.Numerics;
using DailyDuty.CustomNodes;
using DailyDuty.Utilities;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.FashionReport;

public class DataNode(FashionReport module) : DataNodeBase<FashionReport>(module) {
    private readonly FashionReport module = module;

    private TextNode? allowancesRemaining;
    private TextNode? highestScore;
    private TextNode? fashionReportAvailable;

    protected override void BuildNode(VerticalListNode container) {
        container.AddNode([
            new HorizontalListNode {
                FitToContentHeight = true,
                ItemSpacing = 4.0f,
                InitialNodes = [
                    new TextNode {
                        Size = new Vector2(225.0f, 28.0f),
                        String = "Allowances Remaining",
                        AlignmentType = AlignmentType.Left,
                        Height = 32.0f,
                    },
                    allowancesRemaining = new TextNode {
                        Size = new Vector2(225.0f, 32.0f),
                        AlignmentType = AlignmentType.Left,
                        String = "Allowances Not Updated",
                    },
                ], 
            },
            new HorizontalListNode {
                FitToContentHeight = true,
                ItemSpacing = 4.0f,
                InitialNodes = [
                    new TextNode {
                        Size = new Vector2(225.0f, 28.0f),
                        String = "Highest Score",
                        AlignmentType = AlignmentType.Left,
                        Height = 32.0f,
                    },
                    highestScore = new TextNode {
                        Size = new Vector2(225.0f, 32.0f),
                        AlignmentType = AlignmentType.Left,
                        String = "Highest Score Not Updated",
                    },
                ],
            },
            new HorizontalListNode {
                FitToContentHeight = true,
                ItemSpacing = 4.0f,
                InitialNodes = [
                    new TextNode {
                        Size = new Vector2(225.0f, 28.0f),
                        String = "Fashion Report Available",
                        AlignmentType = AlignmentType.Left,
                        Height = 32.0f,
                    },
                    fashionReportAvailable = new TextNode {
                        Size = new Vector2(225.0f, 32.0f),
                        AlignmentType = AlignmentType.Left,
                        String = "Available Not Updated",
                    },
                ],
            },
        ]);
    }

    public override void Update() {
        base.Update();

        allowancesRemaining?.String = module.ModuleData.AllowancesRemaining.ToString();
        highestScore?.String = module.ModuleData.HighestWeeklyScore.ToString();
        fashionReportAvailable?.String = IsFashionReportAvailable ? "Available" : "Not Available";
    }
    
    private bool IsFashionReportAvailable 
        => DateTime.UtcNow > Time.NextWeeklyReset().AddDays(-4) && DateTime.UtcNow < Time.NextWeeklyReset();
}
