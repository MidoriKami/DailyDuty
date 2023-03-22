using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using DailyDuty.Models.Attributes;
using DailyDuty.Models.Enums;
using DailyDuty.System.Commands;
using DailyDuty.Views.Components;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib;
using KamiLib.Atk;
using KamiLib.GameState;
using Newtonsoft.Json;

namespace DailyDuty.System;

public class UiColor
{
    public required ushort ColorKey { get; set; }
}

public class TodoConfig
{
    public bool Enable = true;
    
    [ConfigOption("EnableDailyTasks")]
    public bool DailyTasks = true;
    
    [ConfigOption("EnableWeeklyTasks")]
    public bool WeeklyTasks = true;
    
    [ConfigOption("EnableSpecialTasks")]
    public bool SpecialTasks = true;

    [ConfigOption("HideInQuestEvent")]
    public bool HideDuringQuests = true;
    
    [ConfigOption("HideInDuties")]
    public bool HideInDuties = true;

    [ConfigOption("DailyTasksLabel")]
    public string DailyLabel = "Daily Tasks";
    
    [ConfigOption("WeeklyTasksLabel")]
    public string WeeklyLabel = "Weekly Tasks";
    
    [ConfigOption("SpecialTasksLabel")]
    public string SpecialLabel = "Special Tasks";

    [ConfigOption("Position")]
    public Vector2 TextNodePosition = new(750, 512);
    
    [ConfigOption("TextAlignment")]
    public AlignmentType AlignmentType = AlignmentType.TopLeft;
    
    [ConfigOption("LineSpacing", 5, 30)]
    public int LineSpacing = 20;
    
    [ConfigOption("FontSize", 5, 30)]
    public int FontSize = 14;

    [ConfigOption("TextColor", 1.0f, 1.0f, 1.0f, 1.0f)]
    public Vector4 TextColor = new(1.0f, 1.0f, 1.0f, 1.0f);
    
    [ConfigOption("TextBorderColor", 142 / 255.0f, 106 / 255.0f, 12 / 255.0f, 1.0f)]
    public Vector4 TextBorderColor = new(142 / 255.0f, 106 / 255.0f, 12 / 255.0f, 1.0f);
    
    [ConfigOption("HeaderOutlineColor", 14)]
    public UiColor HeaderGlowKey = new() { ColorKey = 14 };
}

public unsafe class TodoController : IDisposable
{
    public TodoConfig Config = new();
    private bool configChanged;
    private static AtkUnitBase* ParentAddon => (AtkUnitBase*) Service.GameGui.GetAddonByName("NamePlate");
    private AtkTextNode* TodoListNode => GetTextNode();
    private const uint TextNodeId = 1000;
    private readonly TodoCommands todoCommands = new();

    public void Load()
    {
        KamiCommon.CommandManager.AddCommand(todoCommands);
        Config = LoadConfig();
        
        if(ParentAddon is null) PluginLog.Warning("NamePlate is Null on TodoController.Load");
        if(!IsNodeAlreadyCreated()) CreateTextNode();
    }
    
    public void DrawConfig()
    {
        if (TodoListNode is null) return;

        var fields = Config
            .GetType()
            .GetFields(); 
        
        var configOptions = fields
            .Where(field => field.GetCustomAttribute(typeof(ConfigOption)) is not null)
            .Select(field => (field, (ConfigOption) field.GetCustomAttribute(typeof(ConfigOption))!))
            .ToList();
        
        TodoEnableView.Draw(Config, SaveConfig);
        GenericConfigView.Draw(configOptions, Config, SaveConfig, "Todo Display Configuration");
    }

    public void Update()
    {
        if (TodoListNode is not null)
        {
            var seString = new SeStringBuilder();

            if(Config.DailyTasks) UpdateCategory(seString, ModuleType.Daily);
            if(Config.WeeklyTasks) UpdateCategory(seString, ModuleType.Weekly);
            if(Config.SpecialTasks) UpdateCategory(seString, ModuleType.Special);

            var encodedString = seString.Encode();
            
            TodoListNode->SetText(encodedString);
            
            TodoListNode->AtkResNode.ToggleVisibility(encodedString.Length != 0 && Config.Enable);
            if(Config.HideInDuties && Condition.IsBoundByDuty()) TodoListNode->AtkResNode.ToggleVisibility(false);
            if(Config.HideDuringQuests && Condition.IsInCutsceneOrQuestEvent()) TodoListNode->AtkResNode.ToggleVisibility(false);
            
            TodoListNode->TextColor = VectorToByteColor(Config.TextColor);
            TodoListNode->EdgeColor = VectorToByteColor(Config.TextBorderColor);
        }
        
        if(configChanged) SaveConfig();
        configChanged = false;
    }

    private void UpdateCategory(SeStringBuilder builder, ModuleType type)
    {
        var tasks = DailyDutyPlugin.System.ModuleController
            .GetModules(type)
            .Where(module => module.ModuleConfig.ModuleEnabled)
            .Where(module => module.ModuleStatus is ModuleStatus.Incomplete)
            .ToList();

        if (tasks.Count == 0) return;

        var label = type switch
        {
            ModuleType.Daily => Config.DailyLabel,
            ModuleType.Weekly => Config.WeeklyLabel,
            ModuleType.Special => Config.SpecialLabel,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        
        builder.AddUiGlow(label, Config.HeaderGlowKey.ColorKey);
        builder.Add(new NewLinePayload());
        
        foreach (var module in tasks)
        {
            builder.AddText(module.ModuleName.GetLabel());
            builder.Add(new NewLinePayload());
        }
    }
    
    public void Unload()
    {
        KamiCommon.CommandManager.RemoveCommand(todoCommands);
        if (IsNodeAlreadyCreated()) DestroyTextNode();
    }
    
    public void Dispose() => Unload();

    private void RefreshTextNode()
    {
        if (TodoListNode is not null)
        {
            TodoListNode->TextColor = VectorToByteColor(Config.TextColor);
            TodoListNode->EdgeColor = VectorToByteColor(Config.TextBorderColor);
            TodoListNode->LineSpacing = (byte) Config.LineSpacing;
            TodoListNode->AlignmentFontType = (byte) Config.AlignmentType;
            TodoListNode->FontSize = (byte) Config.FontSize;
            TodoListNode->AtkResNode.SetPositionFloat(Config.TextNodePosition.X, Config.TextNodePosition.Y);
        }
    }

    private void CreateTextNode()
    {
        var textNode = IMemorySpace.GetUISpace()->Create<AtkTextNode>();

        var resNode = &textNode->AtkResNode;

        resNode->Type = NodeType.Text;
        resNode->NodeID = TextNodeId;
        resNode->Flags = 8243;

        textNode->TextColor = VectorToByteColor(Config.TextColor);
        textNode->EdgeColor = VectorToByteColor(Config.TextBorderColor);
        textNode->BackgroundColor = new ByteColor { R = 255, G = 255, B = 255, A = 255 };
        textNode->LineSpacing = (byte) Config.LineSpacing;
        textNode->AlignmentFontType = (byte) Config.AlignmentType;
        textNode->FontSize = (byte) Config.FontSize;
        textNode->TextFlags = 0x88;
        
        resNode->SetWidth(200);
        resNode->SetHeight(200);
        resNode->SetPositionFloat(Config.TextNodePosition.X, Config.TextNodePosition.Y);
        
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
                return Config;
            }
            
            var jsonString = File.ReadAllText(dataFile.FullName);
            return (TodoConfig) JsonConvert.DeserializeObject(jsonString, Config.GetType())!;
        }
        catch (Exception exception)
        {
            PluginLog.Error(exception, $"Failed to load data for TodoController");
            return new TodoConfig();
        }
    }
    
    public void SaveConfig()
    {
        RefreshTextNode();

        try
        {
            PluginLog.Debug($"[Todo Config] Saving Todo.config.json");
            var dataFile = GetConfigFileInfo();

            var jsonString = JsonConvert.SerializeObject(Config, Config.GetType(), new JsonSerializerSettings { Formatting = Formatting.Indented });
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