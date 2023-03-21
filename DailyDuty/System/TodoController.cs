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
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Atk;
using KamiLib.GameState;
using Newtonsoft.Json;

namespace DailyDuty.System;

public class TodoConfig
{
    public bool Enable = true;
    
    public bool DailyTasks = true;
    public bool WeeklyTasks = true;
    public bool SpecialTasks = true;

    public bool HideDuringQuests = true;
    public bool HideInDuties = true;

    public string DailyLabel = "Daily Tasks";
    public string WeeklyLabel = "Weekly Tasks";
    public string SpecialLabel = "Special Tasks";

    public Vector4 TextColor = new(1.0f, 1.0f, 1.0f, 1.0f);
    public Vector4 TextBorderColor = new(142 / 255.0f, 106 / 255.0f, 12 / 255.0f, 1.0f);
    public ushort HeaderGlowKey = 14;

    public Vector2 TextNodePosition = new(750, 512);
    public AlignmentType AlignmentType = AlignmentType.TopLeft;
}

public unsafe class TodoController : IDisposable
{
    private TodoConfig config = new();
    private bool configChanged;
    private static AtkUnitBase* ParentAddon => (AtkUnitBase*) Service.GameGui.GetAddonByName("NamePlate");
    private AtkTextNode* TodoListNode => GetTextNode();
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
        if (TodoListNode is null) return;

        TodoEnableView.Draw(config, SaveConfig);
        TodoPositionEditView.Draw(TodoListNode, config, SaveConfig);
        TodoDisplayOptionsView.Draw(TodoListNode, config, SaveConfig);
        TodoHeaderLabelEditView.Draw(config, SaveConfig);
        TodoTextColorPickerView.Draw(config, SaveConfig);
    }

    public void Update()
    {
        if (slowUpdateStopwatch.Elapsed > TimeSpan.FromMilliseconds(250))
        {
            SlowUpdate();
            slowUpdateStopwatch.Restart();
        }

        if (TodoListNode is not null)
        {
            TodoListNode->AtkResNode.ToggleVisibility(true);
            if(config.HideInDuties && Condition.IsBoundByDuty()) TodoListNode->AtkResNode.ToggleVisibility(false);
            if(config.HideDuringQuests && Condition.IsInCutsceneOrQuestEvent()) TodoListNode->AtkResNode.ToggleVisibility(false);
            
            TodoListNode->TextColor = VectorToByteColor(config.TextColor);
            TodoListNode->EdgeColor = VectorToByteColor(config.TextBorderColor);
        }
        
        if(configChanged) SaveConfig();
        configChanged = false;
    }

    private void SlowUpdate()
    {
        if (TodoListNode is not null)
        {
            var seString = new SeStringBuilder();

            if(config.DailyTasks) UpdateDaily(seString);
            if(config.WeeklyTasks) UpdateWeekly(seString);
            if(config.SpecialTasks) UpdateSpecial(seString);
            
            TodoListNode->SetText(seString.Encode());
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
        
        builder.AddUiGlow(config.DailyLabel, config.HeaderGlowKey);
        builder.Add(new NewLinePayload());
        
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
        builder.AddUiGlow(config.WeeklyLabel, config.HeaderGlowKey);
        builder.Add(new NewLinePayload());
        
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
        builder.AddUiGlow(config.SpecialLabel, config.HeaderGlowKey);
        builder.Add(new NewLinePayload());
        
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

        textNode->TextColor = VectorToByteColor(config.TextColor);
        textNode->EdgeColor = VectorToByteColor(config.TextBorderColor);
        textNode->BackgroundColor = new ByteColor { R = 255, G = 255, B = 255, A = 255 };
        textNode->LineSpacing = 16;
        textNode->AlignmentFontType = (byte) AlignmentType.BottomLeft;
        textNode->FontSize = 14;
        textNode->TextFlags = 0x88;
        textNode->TextFlags2 = 0;
        
        resNode->SetWidth(200);
        resNode->SetHeight(200);
        resNode->SetPositionFloat(config.TextNodePosition.X, config.TextNodePosition.Y);
        
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
                SaveConfig();
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
    
    private void SaveConfig()
    {
        try
        {
            PluginLog.Debug($"[Todo Config] Saving Todo.config.json");
            var dataFile = GetConfigFileInfo();

            var jsonString = JsonConvert.SerializeObject(config, config.GetType(), new JsonSerializerSettings { Formatting = Formatting.Indented });
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

    private ByteColor VectorToByteColor(Vector4 vector) => new()
    {
        R = (byte)(vector.X * 255),
        G = (byte)(vector.Y * 255),
        B = (byte)(vector.Z * 255),
        A = (byte)(vector.W * 255),
    };
}