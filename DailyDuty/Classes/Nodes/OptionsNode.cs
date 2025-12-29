using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DailyDuty.Enums;
using KamiToolKit.Nodes;

namespace DailyDuty.Classes.Nodes;

public class OptionsNode : SimpleComponentNode {

    private readonly ScrollingAreaNode<TreeListNode> optionsList;
    private readonly List<ModuleOptionNode> optionNodes = [];

    public IReadOnlyList<ModuleOptionNode> Nodes => optionNodes;
    public ReadOnlyCollection<TreeListCategoryNode> CategoryNodes => optionsList.ContentNode.CategoryNodes;
    
    public OptionsNode() {
        optionsList = new ScrollingAreaNode<TreeListNode> {
            ContentHeight = 1000.0f,
            ScrollSpeed = 24,
        };
        optionsList.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        optionsList.Size = Size;
    }

    public void SetOptions(List<LoadedModule>? modules) {
        if (modules is null) return;
        
        var categoryGroups = modules.GroupBy(module => module.FeatureBase.ModuleInfo.Type);
        uint optionIndex = 0;

        foreach (var categoryGroup in categoryGroups) {

            var newCategoryNode = new TreeListCategoryNode {
                SeString = categoryGroup.Key.Description,
                OnToggle = isVisible => OnCategoryToggled(isVisible, categoryGroup.Key),
            };

            foreach (var module in categoryGroup) {
                var newOptionNode = new ModuleOptionNode {
                    NodeId = optionIndex++,
                    Height = 38.0f,
                    Module = module,
                    IsVisible = true,
                };

                newOptionNode.OnClick = () => OnOptionClicked(newOptionNode);
                newCategoryNode.AddNode(newOptionNode);
                optionNodes.Add(newOptionNode);
            }
            
            optionsList.ContentNode.AddCategoryNode(newCategoryNode);
        }
        
        optionsList.ContentHeight = optionsList.ContentNode.CategoryNodes.Sum(node => node.Height) + 20.0f;
    }

    private void OnOptionClicked(ModuleOptionNode optionNode)
        => OptionClicked?.Invoke(optionNode);

    private void OnCategoryToggled(bool isVisible, ModuleType categoryGroupKey)
        => CategoryToggled?.Invoke(isVisible, categoryGroupKey);

    public required Action<ModuleOptionNode>? OptionClicked { get; set; }
    public required Action<bool, ModuleType>? CategoryToggled { get; set; }

    public void RecalculateLayout() {
        optionsList.ContentNode.RefreshLayout();
        optionsList.ContentHeight = optionsList.ContentNode.CategoryNodes.Sum(node => node.Height) + 20.0f;
    }
}
