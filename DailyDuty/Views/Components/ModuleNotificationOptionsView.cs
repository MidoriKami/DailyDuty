using System;
using DailyDuty.Abstracts;
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
        ImGui.Text("Notification Options");
        ImGui.Separator();
        ImGuiHelpers.ScaledIndent(15.0f);
        
        DrawMessageToggles(moduleConfig, saveConfig);
        
        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);

        var statusMessage = moduleConfig.OnLoginMessage || moduleConfig.OnZoneChangeMessage;
        var resetMessage = moduleConfig.ResetMessage;

        if (statusMessage || resetMessage)
        {
            ImGui.Text("Notification Customization");
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
        if (ImGui.Checkbox("Enable Custom Reset Message", ref moduleConfig.UseCustomResetMessage)) saveConfig();
        if (moduleConfig.UseCustomResetMessage)
        {
            ImGui.InputTextWithHint("##CustomResetMessage", "Reset Message", ref moduleConfig.CustomResetMessage, 2048);
            if (ImGui.IsItemDeactivatedAfterEdit()) saveConfig();
        }
    }
    
    private static void DrawCustomStatusMessage(ModuleConfigBase moduleConfig, Action saveConfig)
    {
        if (ImGui.Checkbox("Enable Custom Status Message", ref moduleConfig.UseCustomStatusMessage)) saveConfig();
        if (moduleConfig.UseCustomStatusMessage)
        {
            ImGui.InputTextWithHint("##CustomStatusMessage", "Status Message", ref moduleConfig.CustomStatusMessage, 2048);
            if (ImGui.IsItemDeactivatedAfterEdit()) saveConfig();
        }
    }
    
    private static void DrawCustomChatChannel(ModuleConfigBase moduleConfig, Action saveConfig)
    {
        if (ImGui.Checkbox("Enable Custom Channel", ref moduleConfig.UseCustomChannel)) saveConfig();

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
        if (ImGui.Checkbox("Send Status on Login", ref moduleConfig.OnLoginMessage)) saveConfig();
        ImGuiComponents.HelpMarker("Sends a status notification to chat when you login");
        
        if (ImGui.Checkbox("Send Status on Zone Change", ref moduleConfig.OnZoneChangeMessage)) saveConfig();
        ImGuiComponents.HelpMarker("Sends a status notification to chat when you change zones\nLimited to once per 5 mins");
        
        if (ImGui.Checkbox("Send Message on Reset", ref moduleConfig.ResetMessage)) saveConfig();
        ImGuiComponents.HelpMarker("Sends a notification to chat if a reset recently occured");
    }
}