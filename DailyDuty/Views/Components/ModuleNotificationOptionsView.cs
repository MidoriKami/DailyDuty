using System;
using DailyDuty.Abstracts;
using DailyDuty.System.Localization;
using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Utility;
using ImGuiNET;

namespace DailyDuty.Views.Components;

public static class ModuleNotificationOptionsView
{
    public static void Draw(ModuleConfigBase moduleConfig, Action saveConfig)
    {
        ImGui.Text(Strings.NotificationOptions);
        ImGui.Separator();
        ImGuiHelpers.ScaledIndent(15.0f);
        
        DrawMessageToggles(moduleConfig, saveConfig);
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);

        var statusMessage = moduleConfig.OnLoginMessage || moduleConfig.OnZoneChangeMessage;
        var resetMessage = moduleConfig.ResetMessage;

        if (statusMessage || resetMessage)
        {
            ImGui.Text(Strings.NotificationCustomization);
            ImGui.Separator();
            ImGuiHelpers.ScaledIndent(15.0f);

            if (statusMessage)
            {
                DrawCustomChatChannel(moduleConfig, saveConfig);
                DrawCustomStatusMessage(moduleConfig, saveConfig);
            }

            if (resetMessage)
            {
                DrawCustomResetMessage(moduleConfig, saveConfig);
            }

            ImGuiHelpers.ScaledDummy(10.0f);
            ImGuiHelpers.ScaledIndent(-15.0f);
        }
    }
    
    private static void DrawCustomResetMessage(ModuleConfigBase moduleConfig, Action saveConfig)
    {
        if (ImGui.Checkbox(Strings.EnableCustomResetMessage, ref moduleConfig.UseCustomResetMessage)) saveConfig();
        if (moduleConfig.UseCustomResetMessage)
        {
            ImGui.PushFont(DailyDutyPlugin.System.FontController.Axis12.ImFont);
            ImGui.InputTextWithHint("##CustomResetMessage", Strings.ResetMessage, ref moduleConfig.CustomResetMessage, 2048);
            ImGui.PopFont();
            if (ImGui.IsItemDeactivatedAfterEdit()) saveConfig();
        }
    }
    
    private static void DrawCustomStatusMessage(ModuleConfigBase moduleConfig, Action saveConfig)
    {
        if (ImGui.Checkbox(Strings.EnableCustomStatusMessage, ref moduleConfig.UseCustomStatusMessage)) saveConfig();
        if (moduleConfig.UseCustomStatusMessage)
        {
            ImGui.PushFont(DailyDutyPlugin.System.FontController.Axis12.ImFont);
            ImGui.InputTextWithHint("##CustomStatusMessage", Strings.StatusMessage, ref moduleConfig.CustomStatusMessage, 2048);
            ImGui.PopFont();
            if (ImGui.IsItemDeactivatedAfterEdit()) saveConfig();
        }
    }
    
    private static void DrawCustomChatChannel(ModuleConfigBase moduleConfig, Action saveConfig)
    {
        if (ImGui.Checkbox(Strings.EnableCustomChannel, ref moduleConfig.UseCustomChannel)) saveConfig();

        if (moduleConfig.UseCustomChannel)
        {
            if (ImGui.BeginCombo("##CustomChannel", moduleConfig.MessageChatChannel.ToString()))
            {
                foreach (var value in Enum.GetValues<XivChatType>())
                {
                    var label = value.GetAttribute<XivChatTypeInfoAttribute>()?.FancyName ?? value.ToString();
                    
                    if (ImGui.Selectable(label, moduleConfig.MessageChatChannel == value))
                    {
                        moduleConfig.MessageChatChannel = value;
                        saveConfig.Invoke();
                    }
                }
                
                ImGui.EndCombo();
            }
        }
    }
    
    private static void DrawMessageToggles(ModuleConfigBase moduleConfig, Action saveConfig)
    {
        if (ImGui.Checkbox(Strings.SendStatusOnLogin, ref moduleConfig.OnLoginMessage)) saveConfig();
        ImGuiComponents.HelpMarker(Strings.SendStatusOnLoginHelp);
        
        if (ImGui.Checkbox(Strings.SendStatusOnZoneChange, ref moduleConfig.OnZoneChangeMessage)) saveConfig();
        ImGuiComponents.HelpMarker(Strings.SendStatusOnZoneChangeHelp);
        
        if (ImGui.Checkbox(Strings.SendMessageOnReset, ref moduleConfig.ResetMessage)) saveConfig();
        ImGuiComponents.HelpMarker(Strings.SendMessageOnResetHelp);
    }
}