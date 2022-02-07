using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyDuty.Data.Enums;
using DailyDuty.Data.ModuleData.TreasureMapModule;
using DailyDuty.Data.SettingsObjects;
using DailyDuty.Data.SettingsObjects.DailySettings;
using DailyDuty.Interfaces;
using DailyDuty.Utilities;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Logging;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using Condition = DailyDuty.Utilities.Condition;

namespace DailyDuty.Modules.Daily;

internal class TreasureMap : 
    IConfigurable,
    IZoneChangeThrottledNotification,
    IZoneChangeAlwaysNotification,
    ILoginNotification,
    ICompletable
{

    private TreasureMapSettings Settings => Service.Configuration.Current().TreasureMap;
    public CompletionType Type => CompletionType.Daily;
    public string HeaderText => "Treasure Map";
    public GenericSettings GenericSettings => Settings;

    private readonly HashSet<int> mapLevels;
    private int SelectedMinimumMapLevel
    {
        get
        {
            if (Settings.MinimumMapLevel == 0)
            {
                Settings.MinimumMapLevel = mapLevels.First();
            }

            return Settings.MinimumMapLevel;
        }
        set => Settings.MinimumMapLevel = value;
    }

    public TreasureMap()
    {
        Service.Chat.ChatMessage += OnChatMap;

        mapLevels = MapList.Maps.Select(m => m.Level).ToHashSet();
    }
    public void Dispose()
    {
        Service.Chat.ChatMessage -= OnChatMap;
    }

    public bool IsCompleted()
    {
        return IsTreasureMapAvailable() == false;
    }

    void IZoneChangeAlwaysNotification.SendNotification()
    {
        if (Condition.IsBoundByDuty() == true) return;

        if (Settings.HarvestableMapNotification == true && TimeUntilNextMap() == TimeSpan.Zero)
        {
            var e = Service.ClientState.TerritoryType;
            var maps = GetMapsForTerritory(e);

            foreach (var map in maps)
            {
                if (map.Level >= Settings.MinimumMapLevel)
                {
                    var mapName = Service.DataManager.GetExcelSheet<Item>()!.GetRow(map.ItemID)!.Name; 

                    Chat.Print(HeaderText, $"A '{mapName}' is available for harvest in this area");
                }
            }
        }
    }

    public void SendNotification()
    {
        if (IsTreasureMapAvailable() && Condition.IsBoundByDuty() == false)
        {
            Chat.Print(HeaderText, "Treasure Map Available");
        }
    }

    public void NotificationOptions()
    {
        Draw.OnLoginReminderCheckbox(Settings, HeaderText);
        Draw.OnTerritoryChangeCheckbox(Settings, HeaderText);

        Draw.NotificationField("Map Acquisition Notification", HeaderText, ref Settings.NotifyOnAcquisition, "Confirm Map Acquisition with a chat message");
        Draw.NotificationField("Harvestable Map Notification",HeaderText, ref Settings.HarvestableMapNotification, "Show a notification in chat when there are harvestable Treasure Maps available in the current area");

        DrawMinimumMapLevelComboBox();
    }

    public void EditModeOptions()
    {
    }

    public void DisplayData()
    {
        DisplayLastMapCollectedTime();

        DisplayTimeUntilNextMap();
    }

    //
    //  Implementation
    //

    private TimeSpan TimeUntilNextMap()
    {
        var lastMapTime = Settings.LastMapGathered;
        var nextAvailableTime = lastMapTime.AddHours(18);

        if (DateTime.Now >= nextAvailableTime)
        {
            return TimeSpan.Zero;
        }
        else
        {
            return nextAvailableTime - DateTime.Now;
        }
    }

    private HashSet<Data.ModuleData.TreasureMapModule.TreasureMap> GetMapsForTerritory(uint territory)
    {
        return Data.ModuleData.TreasureMapModule.MapList.Maps
            .Where(m => m.HarvestData.Any(data => data.Value.Contains(territory)))
            .Where(m => m.Level >= Settings.MinimumMapLevel)
            .ToHashSet();
    }

    private bool IsTreasureMapAvailable()
    {
        return TimeUntilNextMap() == TimeSpan.Zero;
    }

    // Based on https://github.com/Ottermandias/Accountant/blob/main/Accountant/Manager/TimerManager.MapManager.cs#L75
    private void OnChatMap(XivChatType type, uint senderID, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (Settings.Enabled == false) return;

        if ((int)type != 2115 || !Service.Condition[ConditionFlag.Gathering])
            return;

        if (message.Payloads.FirstOrDefault(p => p is ItemPayload) is not ItemPayload item)
            return;

        if (!IsMap(item.ItemId))
            return;

        if (Settings.NotifyOnAcquisition == true)
        {
            var mapName = item.Item!.Name.ToString();

            Chat.Print(HeaderText, $"A '{mapName}' has been gathered");
            Chat.Print(HeaderText, $"Your next map will be available on {DateTime.Now.AddHours(18)}");
        }

        Settings.LastMapGathered = DateTime.Now;
        Service.Configuration.Save();
    }

    private bool IsMap(uint itemID)
    {
        var map = GetMapByID(itemID);

        return map != null;
    }

    private Data.ModuleData.TreasureMapModule.TreasureMap? GetMapByID(uint itemID)
    {
        return MapList.Maps.FirstOrDefault(map => map.ItemID == itemID);
    }

    private void DisplayLastMapCollectedTime()
    {
        ImGui.Text("Last Map Collected");
        ImGui.SameLine();

        ImGui.Text(Settings.LastMapGathered == new DateTime() ? "Never" : $"{Settings.LastMapGathered}");
        ImGui.Spacing();
    }
    private void DisplayTimeUntilNextMap()
    {
        ImGui.Text("Time Until Next Map");
        ImGui.SameLine();

        var span = TimeUntilNextMap();
        if (span == TimeSpan.Zero)
        {
            ImGui.TextColored(new(0, 255, 0, 255), $" {span.FormatAuto()}");
        }
        else
        {
            ImGui.Text($" {span.FormatAuto()}");
        }
        ImGui.Spacing();
    }

    private void DrawMinimumMapLevelComboBox()
    {
        if (Settings.HarvestableMapNotification == false) return;

        ImGui.Indent(15 *ImGuiHelpers.GlobalScale);

        ImGui.PushItemWidth(50 * ImGuiHelpers.GlobalScale);

        if (ImGui.BeginCombo("Minimum Map Level", SelectedMinimumMapLevel.ToString(), ImGuiComboFlags.PopupAlignLeft))
        {
            foreach (var element in mapLevels)
            {
                bool isSelected = element == SelectedMinimumMapLevel;
                if (ImGui.Selectable(element.ToString(), isSelected))
                {
                    SelectedMinimumMapLevel = element;
                    Settings.MinimumMapLevel = SelectedMinimumMapLevel;
                }

                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }

            ImGui.EndCombo();
        }

        ImGuiComponents.HelpMarker("Only show notifications that a map is available if the map is at least this level");

        ImGui.PopItemWidth();

        ImGui.Indent(-15 * ImGuiHelpers.GlobalScale);
    }

}