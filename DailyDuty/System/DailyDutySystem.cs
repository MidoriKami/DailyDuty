using System;
using DailyDuty.Models;
using Dalamud.Game;
using Dalamud.Logging;
using KamiLib.Utilities;

namespace DailyDuty.System;

public class DailyDutySystem : IDisposable
{
    public static ModuleController ModuleController = null!;
    private readonly AddonController addonController;
    public readonly TodoController TodoController;
    public SystemConfig SystemConfig;

    public DailyDutySystem()
    {
        SystemConfig = new SystemConfig();
        
        LocalizationController.Instance.Initialize();
        PayloadController.Instance.Initialize();
        ModuleController = new ModuleController();
        addonController = new AddonController();
        TodoController = new TodoController();

        if (Service.ClientState.IsLoggedIn)
        {
            OnLogin(this, EventArgs.Empty);
        }
        
        Service.Framework.Update += OnFrameworkUpdate;
        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
        Service.ClientState.TerritoryChanged += OnZoneChange;
        Service.ClientState.EnterPvP += OnEnterPvP;
        Service.ClientState.LeavePvP += OnLeavePvP;
        AddonController.AddonPreSetup += OnAddonPreSetup;
        AddonController.AddonPostSetup += OnAddonPostSetup;
        AddonController.AddonFinalize += OnAddonFinalize;
        Service.PluginInterface.UiBuilder.Draw += OnDraw;
    }

    public void Dispose()
    {
        Service.Framework.Update -= OnFrameworkUpdate;
        Service.ClientState.Login -= OnLogin;
        Service.ClientState.Logout -= OnLogout;
        Service.ClientState.TerritoryChanged -= OnZoneChange;
        Service.ClientState.EnterPvP -= OnEnterPvP;
        Service.ClientState.LeavePvP -= OnLeavePvP;
        AddonController.AddonPreSetup -= OnAddonPreSetup;
        AddonController.AddonPostSetup -= OnAddonPostSetup;
        AddonController.AddonFinalize -= OnAddonFinalize;
        Service.PluginInterface.UiBuilder.Draw -= OnDraw;

        ModuleController.Dispose();
        addonController.Dispose();
        TodoController.Dispose();
        LocalizationController.Cleanup();
        PayloadController.Cleanup();
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;

        // Check for reset, and reset modules that need it 
        ModuleController.ResetModules();
        
        // Update All Modules
        ModuleController.UpdateModules();
        
        // Update TodoDisplay
        TodoController.Update();
    }
    
    private void OnLogin(object? sender, EventArgs e)
    {
        LoadSystemConfig();
        
        ModuleController.LoadModules();
        
        TodoController.Load();
    }
    
    private void OnLogout(object? sender, EventArgs e)
    {
        ModuleController.UnloadModules();
        
        TodoController.Unload();
    }
    
    private void OnDraw()
    {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;
        
        TodoController.DrawExtras();
    }
    
    private void OnZoneChange(object? sender, ushort territoryTypeId)
    {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;
        
        ModuleController.ZoneChange(territoryTypeId);
    }
    
    private void OnAddonPreSetup(AddonArgs addonInfo)
    {
        if (Service.ClientState.IsPvP) return;
        
        ModuleController.AddonPreSetup(addonInfo);
    }
    
    private void OnAddonPostSetup(AddonArgs addonInfo)
    {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.LocalContentId is 0) return;
        
        ModuleController.AddonPostSetup(addonInfo);

        if (addonInfo.AddonName == "NamePlate") TodoController.Load();
    }
    
    private void OnAddonFinalize(AddonArgs addonInfo)
    {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState.LocalContentId is 0) return;
        
        ModuleController.AddonFinalize(addonInfo);
        
        if (addonInfo.AddonName == "NamePlate") TodoController.Unload();
    }
    
    private void OnLeavePvP() => TodoController.Show();

    private void OnEnterPvP() => TodoController.Hide();

    private void LoadSystemConfig()
    {
        SystemConfig = CharacterFileController.LoadFile<SystemConfig>("System.config.json", SystemConfig);
        
        PluginLog.Debug($"[DailyDutySystem] Logging into character: {Service.ClientState.LocalPlayer?.Name}, updating System.config.json");

        SystemConfig.CharacterName = Service.ClientState.LocalPlayer?.Name.ToString() ?? "Unable to Read Name";
        SystemConfig.CharacterWorld = Service.ClientState.LocalPlayer?.HomeWorld.GameData?.Name.ToString() ?? "Unable to Read World";
        SaveSystemConfig();
    }

    public void SaveSystemConfig() => CharacterFileController.SaveFile("System.config.json", SystemConfig.GetType(), SystemConfig);
}