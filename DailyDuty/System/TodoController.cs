using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.Views.Components;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using KamiLib.Atk;
using Newtonsoft.Json;

namespace DailyDuty.System;

public class TodoConfig
{
    public Vector2 TextNodePosition = new(750, 512);
    public AlignmentType AlignmentType = AlignmentType.TopLeft;
}

public unsafe class TodoController : IDisposable
{
    private TodoConfig config = new();
    private bool configChanged;
    private static AtkUnitBase* ParentAddon => (AtkUnitBase*) Service.GameGui.GetAddonByName("NamePlate");
    private AtkTextNode* TextNode => GetTextNode();
    private const uint TextNodeId = 1000;
    private readonly Stopwatch slowUpdateStopwatch = Stopwatch.StartNew();
    
    public void Load()
    {
        config = LoadConfig();
        
        if(ParentAddon is null) PluginLog.Warning("NamePlate is Null on TodoController.Load");
        if(!IsNodeAlreadyCreated()) CreateTextNode();
    }

    public void DrawConfig()
    {
        if (TextNode is null) return;
        
        DrawPositionEdit();
        
        var enumValue = (Enum) config.AlignmentType;
        if (GenericEnumView.DrawEnumCombo(ref enumValue))
        {
            config.AlignmentType = (AlignmentType) enumValue;
            if (TextNode is not null) TextNode->AlignmentFontType = (byte) config.AlignmentType;
            configChanged = true;
        }
    }
    private void DrawPositionEdit()
    {
        var position = new Vector2(TextNode->AtkResNode.X, TextNode->AtkResNode.Y);
        ImGui.PushItemWidth(200.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.SliderFloat($"##xPos_atkUnitBase#{(ulong) TextNode:X}", ref position.X, position.X - 10, position.X + 10))
        {
            TextNode->AtkResNode.SetPositionFloat(position.X, position.Y);
            config.TextNodePosition = position;
        }
        if (ImGui.IsItemDeactivatedAfterEdit())
        {
            configChanged = true;
        }

        ImGui.SameLine();
        ImGui.PushItemWidth(200.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.SliderFloat($"Position##yPos_atkUnitBase#{(ulong) TextNode:X}", ref position.Y, position.Y - 10, position.Y + 10))
        {
            TextNode->AtkResNode.SetPositionFloat(position.X, position.Y);
            config.TextNodePosition = position;
        }
        if (ImGui.IsItemDeactivatedAfterEdit())
        {
            configChanged = true;
        }
    }

    public void Update()
    {
        if (slowUpdateStopwatch.Elapsed > TimeSpan.FromMilliseconds(250))
        {
            SlowUpdate();
            slowUpdateStopwatch.Restart();
        }
        
        if(configChanged) SaveConfig(config);
        configChanged = false;
    }

    private void SlowUpdate()
    {
        if (TextNode is not null)
        {
            var seString = new SeStringBuilder();

            UpdateDaily(seString);
            UpdateWeekly(seString);
            UpdateSpecial(seString);
            
            TextNode->SetText(seString.Encode());
        }
    }

    private void UpdateDaily(SeStringBuilder builder)
    {
        var dailyTasks = DailyDutyPlugin.System.ModuleController
            .GetModules(ModuleType.Daily)
            .Where(module => module.ModuleConfig.ModuleEnabled)
            .Where(module => module.ModuleStatus is ModuleStatus.Incomplete)
            .ToList();

        if (dailyTasks.Count == 0) return;
        
        builder.AddText("Daily Tasks");
        builder.Add(new NewLinePayload());
        
        // Daily
        foreach (var dailyModule in dailyTasks)
        {
            if (!dailyModule.ModuleConfig.ModuleEnabled) continue;
            if (dailyModule.ModuleStatus is not ModuleStatus.Incomplete) continue;

            builder.AddText(dailyModule.ModuleName.GetLabel());
            builder.Add(new NewLinePayload());
        }
    }

    private void UpdateWeekly(SeStringBuilder builder)
    {
        var weeklyTasks = DailyDutyPlugin.System.ModuleController
            .GetModules(ModuleType.Weekly)
            .Where(module => module.ModuleConfig.ModuleEnabled)
            .Where(module => module.ModuleStatus is ModuleStatus.Incomplete)
            .ToList();

        if (weeklyTasks.Count == 0) return;
        
        builder.Add(new NewLinePayload());
        builder.AddText("Weekly Tasks");
        builder.Add(new NewLinePayload());
        
        // Weekly
        foreach (var weeklyModule in weeklyTasks)
        {
            builder.AddText(weeklyModule.ModuleName.GetLabel());
            builder.Add(new NewLinePayload());
        }
    }

    private void UpdateSpecial(SeStringBuilder builder)
    {
        var specialTasks = DailyDutyPlugin.System.ModuleController
            .GetModules(ModuleType.Special)
            .Where(module => module.ModuleConfig.ModuleEnabled)
            .Where(module => module.ModuleStatus is ModuleStatus.Incomplete)
            .ToList();

        if (specialTasks.Count == 0) return;
        
        builder.Add(new NewLinePayload());
        builder.AddText("Special Tasks");
        builder.Add(new NewLinePayload());
        
        // Special
        foreach (var specialModule in specialTasks)
        {
            builder.AddText(specialModule.ModuleName.GetLabel());
            builder.Add(new NewLinePayload());
        }
    }

    public void Unload()
    {
        if (IsNodeAlreadyCreated()) DestroyTextNode();
    }
    
    public void Dispose() => Unload();

    private void CreateTextNode()
    {
        var textNode = IMemorySpace.GetUISpace()->Create<AtkTextNode>();

        var resNode = &textNode->AtkResNode;

        resNode->Type = NodeType.Text;
        resNode->NodeID = TextNodeId;
        resNode->Flags = 8243;

        textNode->TextColor = new ByteColor { R = 255, G = 255, B = 255, A = 255 };
        textNode->EdgeColor = new ByteColor { R = 142, G = 106, B = 12 , A = 255 };
        textNode->BackgroundColor = new ByteColor { R = 255, G = 255, B = 255, A = 255 };
        textNode->LineSpacing = 16;
        textNode->AlignmentFontType = (byte) AlignmentType.BottomLeft;
        textNode->FontSize = 14;
        textNode->TextFlags = 0x88;
        textNode->TextFlags2 = 0;
        
        resNode->SetWidth(200);
        resNode->SetHeight(200);
        resNode->SetPositionFloat(config.TextNodePosition.X, config.TextNodePosition.Y);

        var seString = new SeStringBuilder()
            .AddText("Daily Tasks")
            .Add(new NewLinePayload())
            .AddText("That thing you keep not doing")
            .Add(new NewLinePayload())
            .AddText("That other thing")
            .Add(new NewLinePayload())
            .Add(new NewLinePayload())
            .AddText("Weekly Tasks")
            .Add(new NewLinePayload())
            .AddText("Some Weekly Task")
            .Add(new NewLinePayload())
            .Add(new NewLinePayload())
            .AddText("Special Tasks")
            .Add(new NewLinePayload())
            .AddText("Some Special Task")
            .Build();

        textNode->SetText(seString.Encode());
        
        LinkNodeAtEnd(resNode, ParentAddon);
    }

    private void DestroyTextNode()
    {
        var node = GetTextNode();
        
        if (node->AtkResNode.PrevSiblingNode is not null)
            node->AtkResNode.PrevSiblingNode->NextSiblingNode = node->AtkResNode.NextSiblingNode;
            
        if (node->AtkResNode.NextSiblingNode is not null)
            node->AtkResNode.NextSiblingNode->PrevSiblingNode = node->AtkResNode.PrevSiblingNode;
            
        ParentAddon->UldManager.UpdateDrawNodeList();
        
        node->AtkResNode.Destroy(false);
        IMemorySpace.Free(node, (ulong)sizeof(AtkTextNode));
    }

    private static void LinkNodeAtEnd(AtkResNode* resNode, AtkUnitBase* parent)
    {
        var node = parent->RootNode->ChildNode;
        while (node->PrevSiblingNode != null) node = node->PrevSiblingNode;

        node->PrevSiblingNode = resNode;
        resNode->NextSiblingNode = node;
        resNode->ParentNode = node->ParentNode;
        
        parent->UldManager.UpdateDrawNodeList();
    }

    private bool IsNodeAlreadyCreated() => GetTextNode() is not null;
    private AtkTextNode* GetTextNode() => Node.GetNodeByID<AtkTextNode>(ParentAddon->UldManager, TextNodeId);
    
    private TodoConfig LoadConfig()
    {
        try
        {
            PluginLog.Debug($"[Todo Config] Loading Todo.config.json");
            var dataFile = GetConfigFileInfo();
            
            if (dataFile is { Exists: false })
            {
                SaveConfig(config);
                return config;
            }
            
            var jsonString = File.ReadAllText(dataFile.FullName);
            return (TodoConfig) JsonConvert.DeserializeObject(jsonString, config.GetType())!;
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to load data for TodoController");
            return new TodoConfig();
        }
    }
    
    private void SaveConfig(TodoConfig data)
    {
        try
        {
            PluginLog.Debug($"[Todo Config] Saving Todo.config.json");
            var dataFile = GetConfigFileInfo();

            var jsonString = JsonConvert.SerializeObject(data, data.GetType(), new JsonSerializerSettings { Formatting = Formatting.Indented });
            File.WriteAllText(dataFile.FullName, jsonString);
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to save data for TodoController");
        }
    }
    
    private FileInfo GetConfigFileInfo()
    {
        var contentId = PlayerState.Instance()->ContentId;
        var configDirectory = GetCharacterDirectory(contentId);
        return new FileInfo(Path.Combine(configDirectory.FullName, "Todo.config.json"));
    }
    
    private static DirectoryInfo GetCharacterDirectory(ulong contentId)
    {
        var directoryInfo = new DirectoryInfo(Path.Combine(Service.PluginInterface.ConfigDirectory.FullName, contentId.ToString()));

        if (directoryInfo is { Exists: false })
        {
            directoryInfo.Create();
        }

        return directoryInfo;
    }
}