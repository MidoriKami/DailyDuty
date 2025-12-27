using System.Drawing;
using System.Numerics;
using DailyDuty.Classes.Nodes;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.DomanEnclave;

public unsafe class DataNode(DomanEnclave module) : DataNodeBase<DomanEnclave>(module) {

    private TextNode? allowanceText;
    private TextNode? donatedText;
    private TextNode? allowanceRemaining;
    private TextNode? warningText;
    
    protected override void BuildNode(VerticalListNode container) {
        container.AddNode([
            new HorizontalListNode {
                FitToContentHeight = true,
                ItemSpacing = 4.0f,
                InitialNodes = [
                    new TextNode {
                        Size = new Vector2(225.0f, 28.0f),
                        String = "Current Max Allowance",
                        AlignmentType = AlignmentType.Left,
                        Height = 32.0f,
                    },
                    allowanceText = new TextNode {
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
                        String = "Donated This Week",
                        AlignmentType = AlignmentType.Left,
                        Height = 32.0f,
                    },
                    donatedText = new TextNode {
                        Size = new Vector2(225.0f, 32.0f),
                        AlignmentType = AlignmentType.Left,
                        String = "Donated Not Updated",
                    },
                ],
            },
            new HorizontalListNode {
                FitToContentHeight = true,
                ItemSpacing = 4.0f,
                InitialNodes = [
                    new TextNode {
                        Size = new Vector2(225.0f, 28.0f),
                        String = "Allowance Remaining",
                        AlignmentType = AlignmentType.Left,
                        Height = 32.0f,
                    },
                    allowanceRemaining = new TextNode {
                        Size = new Vector2(225.0f, 32.0f),
                        AlignmentType = AlignmentType.Left,
                        String = "Remaining Not Updated",
                    },
                ],
            },
            warningText = new TextNode {
                MultiplyColor = KnownColor.Orange.Vector().Fade(0.40f).AsVector3(),
                String = "Status is unavailable, visit the Doman Enclave to update",
                AlignmentType = AlignmentType.Center,
                Height = 32.0f,
            },
        ]);
    }

    public override void Update() {
        base.Update();

        allowanceText?.String = Allowance.ToString();
        donatedText?.String = Donated.ToString();
        allowanceRemaining?.String = RemainingAllowance.ToString();

        warningText?.IsVisible = Allowance is 0;
    }
    
    private static int Allowance => DomanEnclaveManager.Instance()->State.Allowance;
    private static int Donated => DomanEnclaveManager.Instance()->State.Donated;
    private static int RemainingAllowance => Allowance - Donated;
}
