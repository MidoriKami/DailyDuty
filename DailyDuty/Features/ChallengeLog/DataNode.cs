using System.Collections.Generic;
using DailyDuty.ConfigurationWindow.Nodes;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;

namespace DailyDuty.Features.ChallengeLog;

public class DataNode(ChallengeLog module) : StatusNode<ChallengeLog>(module) {

    private readonly List<StatusNode> statusNodes = [];

    protected override void BuildNode(VerticalListNode container) {

        foreach (var contentsRow in Services.DataManager.GetExcelSheet<ContentsNote>()) {
            if (contentsRow is not { Name.ByteLength: > 0 } ) continue;

            var newStatusNode = new StatusNode {
                Data = contentsRow,
                Width = container.Width,
                Height = 26.0f,
            };
            
            container.AddNode(newStatusNode);
            statusNodes.Add(newStatusNode);
        }
    }

    public override void Update() {
        base.Update();
        
        foreach (var node in statusNodes) {
            node.Update();
        }
    }
}
