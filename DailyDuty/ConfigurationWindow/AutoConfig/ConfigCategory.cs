using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyDuty.ConfigurationWindow.AutoConfig.ConfigEntries;
using DailyDuty.ConfigurationWindow.AutoConfig.NodeEntries;
using DailyDuty.Extensions;
using DailyDuty.Interfaces;
using KamiToolKit.Nodes;

namespace DailyDuty.ConfigurationWindow.AutoConfig;

public class ConfigCategory : IDisposable {
    public required string CategoryLabel { get; init; }
    public required ISavable ConfigObject { get; init; }

    private readonly List<IConfigEntry> configEntries = [];

    public TabbedVerticalListNode BuildNode() {
        var tabbedListNode = new TabbedVerticalListNode {
            FitWidth = true,
        };
          
        tabbedListNode.AddNode(new ResNode {
            Size = new Vector2(4.0f, 4.0f),
        });
        
        tabbedListNode.AddNode(new CategoryTextNode {
            String = CategoryLabel,
        });
            
        tabbedListNode.AddTab(1);

        foreach (var entry in configEntries) {
            if (entry is IndentEntry) {
                tabbedListNode.AddTab(1);
                continue;
            }
            
            tabbedListNode.AddNode(entry.BuildNode());
        }

        tabbedListNode.SubtractTab(1);

        return tabbedListNode;
    }
    
    public ConfigCategory AddCheckbox(string label, string memberName) {
        var memberInfo = ConfigObject.GetType().GetMember(memberName).FirstOrDefault();
        if (memberInfo is null) return this;

        var initialValue = memberInfo.GetValue<bool>(ConfigObject);

        configEntries.Add(new CheckBoxConfig {
            Label = label,
            MemberInfo = memberInfo,
            Config = ConfigObject,
            InitialState = initialValue,
        });

        return this;
    }

    public ConfigCategory AddIntSlider(string label, int min, int max, string memberName) {
        var memberInfo = ConfigObject.GetType().GetMember(memberName).FirstOrDefault();
        if (memberInfo is null) return this;

        var initialValue = memberInfo.GetValue<int>(ConfigObject);

        configEntries.Add(new IntSliderConfig {
            Label = label,
            MemberInfo = memberInfo,
            Config = ConfigObject,
            Range = min..max,
            InitialValue = initialValue,
        });
        
        return this;
    }

    public ConfigCategory AddFloatSlider(string label, float min, float max, int decimalPlaces, float speed, string memberName) {
        var memberInfo = ConfigObject.GetType().GetMember(memberName).FirstOrDefault();
        if (memberInfo is null) return this;

        var initialValue = memberInfo.GetValue<float>(ConfigObject);

        configEntries.Add(new FloatSliderConfig {
            Label = label,
            MemberInfo = memberInfo,
            Config = ConfigObject,
            DecimalPlaces = decimalPlaces,
            MaxValue = max,
            MinValue = min,
            InitialValue = initialValue,
            StepSpeed = speed,
        });

        return this;
    }

    public ConfigCategory AddIndent() {
        configEntries.Add(new IndentEntry());
        return this;
    }

    public ConfigCategory AddColorEdit(string label, string memberName, Vector4? defaultColor = null) {
        var memberInfo = ConfigObject.GetType().GetMember(memberName).FirstOrDefault();
        if (memberInfo is null) return this;

        var initialValue = memberInfo.GetValue<Vector4>(ConfigObject);

        configEntries.Add(new ColorConfig {
            Label = label,
            MemberInfo = memberInfo,
            Config = ConfigObject,
            Color = initialValue,
            DefaultColor = defaultColor,
        });

        return this;
    }

    public ConfigCategory AddInputInt(string label, int step, Range range, string memberName) {
        var memberInfo = ConfigObject.GetType().GetMember(memberName).FirstOrDefault();
        if (memberInfo is null) return this;
        
        var initialValue = memberInfo.GetValue<int>(ConfigObject);
        
        configEntries.Add(new IntInputConfig {
            Label = label,
            MemberInfo = memberInfo,
            Config = ConfigObject,
            Step = step,
            Range = range,
            InitialValue = initialValue,
        });

        return this;
    }

    // Input elements can't represent floats, so it will be truncated to an int
    public ConfigCategory AddInputFloat(string label, int step, Range range, string memberName) {
        var memberInfo = ConfigObject.GetType().GetMember(memberName).FirstOrDefault();
        if (memberInfo is null) return this;
        
        var initialValue = memberInfo.GetValue<float>(ConfigObject);

        configEntries.Add(new FloatInputConfig {
            Label = label,
            MemberInfo = memberInfo,
            Config = ConfigObject,
            Step = step,
            Range = range,
            InitialValue = initialValue,
        });

        return this;
    }

    public ConfigCategory AddSelectIcon(string label, string memberName) {
        var memberInfo = ConfigObject.GetType().GetMember(memberName).FirstOrDefault();
        if (memberInfo is null) return this;
        
        var initialValue = memberInfo.GetValue<uint>(ConfigObject);
        
        configEntries.Add(new SelectIconConfig {
            Label = label,
            MemberInfo = memberInfo,
            Config = ConfigObject,
            InitialIcon = initialValue,
        });

        return this;
    }
    
    public ConfigCategory AddMultiSelectIcon(string label, string memberName, bool includeInput, params uint[] icons) {
        var memberInfo = ConfigObject.GetType().GetMember(memberName).FirstOrDefault();
        if (memberInfo is null) return this;
        
        var initialValue = memberInfo.GetValue<uint>(ConfigObject);
        
        configEntries.Add(new MultiSelectIconConfig {
            Label = label,
            MemberInfo = memberInfo,
            Config = ConfigObject,
            InitialIcon = initialValue,
            AllowManualInput = includeInput,
            Options = icons.ToList(),
        });

        return this;
    }

    public ConfigCategory AddNodeConfig(TextNodeStyle primaryTargetStyle) {
        configEntries.Add(new TextNodeConfig {
            StyleObject = primaryTargetStyle,
        });

        return this;
    }

    public ConfigCategory AddNodeConfig(NodeStyle nodeStyle) {
        configEntries.Add(new NodeConfig<NodeStyle> {
            StyleObject = nodeStyle,
        });

        return this;
    }

    public void Dispose() {
        foreach (var entry in configEntries) {
            entry.Dispose();
        }
    }
}
