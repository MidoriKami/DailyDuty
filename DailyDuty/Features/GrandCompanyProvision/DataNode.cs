using System;
using DailyDuty.Classes.Nodes;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace DailyDuty.Features.GrandCompanyProvision;

public unsafe class DataNode(GrandCompanyProvision module) : DataNodeBase<GrandCompanyProvision>(module) {
    private readonly GrandCompanyProvision module = module;

    private TextNode? miner;
    private TextNode? botanist;
    private TextNode? fisher;

    protected override void BuildNode(VerticalListNode container) {
        container.AddNode([
            new HorizontalListNode {
                Height = 24.0f,
                FitHeight = true,
                InitialNodes = [
                    new TextNode {
                        Width = 200.0f,
                        String = "Miner Completed",
                        AlignmentType = AlignmentType.Left,
                    },
                    miner = new TextNode {
                        Width = 100.0f,
                        String = "Miner Data Not Set",
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
            new HorizontalListNode {
                Height = 24.0f,
                FitHeight = true,
                InitialNodes = [
                    new TextNode {
                        Width = 200.0f,
                        String = "Botanist Completed",
                        AlignmentType = AlignmentType.Left,
                    },
                    botanist = new TextNode {
                        Width = 100.0f,
                        String = "Botanist Data Not Set",
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
            new HorizontalListNode {
                Height = 24.0f,
                FitHeight = true,
                InitialNodes = [
                    new TextNode {
                        Width = 200.0f,
                        String = "Fisher Completed",
                        AlignmentType = AlignmentType.Left,
                    },
                    fisher = new TextNode {
                        Width = 100.0f,
                        String = "Fisher Data Not Set",
                        AlignmentType = AlignmentType.Left,
                    },
                ],
            },
        ]);

        var agent = AgentGrandCompanySupply.Instance();
        
        if (agent->IsAgentActive()) {
            var itemSpan = new Span<GrandCompanyItem>(agent->ItemArray, agent->NumItems);

            foreach (var entry in itemSpan) {
                container.AddNode(new HorizontalListNode {
                    Height = 24.0f,
                    FitHeight = true,
                    InitialNodes = [                    
                        new TextNode {
                            Width = 200.0f,
                            String = entry.ItemName.ToString(),
                            AlignmentType = AlignmentType.Left,
                        },
                        new TextNode {
                            Width = 100.0f,
                            String = entry.IsTurnInAvailable ? "Available" : "Not Available",
                            AlignmentType = AlignmentType.Left,
                        },
                    ],
                });
            }
        }
    }

    public override void Update() {
        base.Update();

        miner?.String = module.ModuleData.MinerComplete ? "Complete" : "Not Completed";
        botanist?.String = module.ModuleData.BotanistComplete ? "Complete" : "Not Completed";
        fisher?.String = module.ModuleData.FisherComplete ? "Complete" : "Not Completed";
    }
}
