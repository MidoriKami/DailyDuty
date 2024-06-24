﻿using System;
using System.Collections.Generic;
using System.Linq;
using DailyDuty.Interfaces;
using DailyDuty.Modules.BaseModules;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace DailyDuty.Classes;

public class ModuleController : IDisposable {
    public List<BaseModule> Modules { get; }
    private readonly GoldSaucerMessageController goldSaucerMessageController;
    private bool modulesLoaded;

    public ModuleController() {
        Modules = new List<BaseModule>();
        goldSaucerMessageController = new GoldSaucerMessageController();

        foreach (var t in GetType().Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BaseModule))))
        {
            if (t.IsAbstract) continue;
            var module = (BaseModule?) Activator.CreateInstance(t);
            if (module is null) continue;
            
            Modules.Add(module);
        }

        goldSaucerMessageController.GoldSaucerUpdate += OnGoldSaucerMessage;
        Service.Chat.ChatMessage += OnChatMessage;
    }
    
    public void Dispose() {
        goldSaucerMessageController.GoldSaucerUpdate -= OnGoldSaucerMessage;
        Service.Chat.ChatMessage -= OnChatMessage;

        goldSaucerMessageController.Dispose();

        foreach (var module in Modules.OfType<IDisposable>()) {
            module.Dispose();
        }
    }

    public IEnumerable<BaseModule> GetModules(ModuleType? type = null) => 
        type is null ? Modules : Modules.Where(module => module.ModuleType == type);

    public void UpdateModules() {
        if (!modulesLoaded) return; 
        
        foreach (var module in Modules) {
            module.Update();
        }
    }

    public void LoadModules() {
        foreach (var module in Modules) {
            module.Load();
        }

        modulesLoaded = true;
    }
    
    public void UnloadModules() {
        foreach (var module in Modules) {
            module.Unload();
        }

        modulesLoaded = false;
    }

    public void ResetModules() {
        var now = DateTime.UtcNow;
        
        foreach (var module in Modules) {
            if (now >= module.GetNextReset()) {
                module.Reset();
            }
        }
    }

    public void ZoneChange(uint newZone) {
        foreach (var module in Modules) {
            module.ZoneChange(newZone);
        }
    }
    
    private void OnGoldSaucerMessage(object? sender, GoldSaucerEventArgs e) {
        foreach (var module in Modules.OfType<IGoldSaucerMessageReceiver>()) {
            module.GoldSaucerUpdate(sender, e);
        }
    }

    private void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled) {
        foreach (var module in Modules.OfType<IChatMessageReceiver>()) {
            module.OnChatMessage(type, timestamp, ref sender, ref message, ref isHandled);
        }
    }
}