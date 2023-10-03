using System;
using DailyDuty.Models;
using Dalamud.Plugin.Services;
using KamiLib.FileIO;

namespace DailyDuty.System;

public class DailyDutySystem : IDisposable
{
    public static ModuleController ModuleController = null!;
    public readonly TodoController TodoController;
    public SystemConfig SystemConfig;

    public DailyDutySystem()
    {
        SystemConfig = new SystemConfig();
        
        LocalizationController.Instance.Initialize();
        PayloadController.Instance.Initialize();
        ModuleController = new ModuleController();
        TodoController = new TodoController();

        if (Service.ClientState.IsLoggedIn)
        {
            OnLogin();
        }
        
        Service.Framework.Update += OnFrameworkUpdate;
        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
        Service.ClientState.TerritoryChanged += OnZoneChange;
        Service.ClientState.EnterPvP += OnEnterPvP;
        Service.ClientState.LeavePvP += OnLeavePvP;
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
        Service.PluginInterface.UiBuilder.Draw -= OnDraw;

        ModuleController.Dispose();
        TodoController.Dispose();
        LocalizationController.Cleanup();
        PayloadController.Cleanup();
    }

    private void OnFrameworkUpdate(IFramework framework)
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
    
    private void OnLogin()
    {
        LoadSystemConfig();
        
        ModuleController.LoadModules();
        
        TodoController.Load();
    }
    
    private void OnLogout()
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
    
    private void OnZoneChange(ushort territoryTypeId)
    {
        if (Service.ClientState.IsPvP) return;
        if (!Service.ClientState.IsLoggedIn) return;
        
        ModuleController.ZoneChange(territoryTypeId);
    }
    
    private void OnLeavePvP() => TodoController.Show();

    private void OnEnterPvP() => TodoController.Hide();

    private void LoadSystemConfig()
    {
        SystemConfig = CharacterFileController.LoadFile<SystemConfig>("System.config.json", SystemConfig);
        
        Service.Log.Debug($"[DailyDutySystem] Logging into character: {Service.ClientState.LocalPlayer?.Name}, updating System.config.json");

        SystemConfig.CharacterName = Service.ClientState.LocalPlayer?.Name.ToString() ?? "Unable to Read Name";
        SystemConfig.CharacterWorld = Service.ClientState.LocalPlayer?.HomeWorld.GameData?.Name.ToString() ?? "Unable to Read World";
        SaveSystemConfig();
    }

    public void SaveSystemConfig() => CharacterFileController.SaveFile("System.config.json", SystemConfig.GetType(), SystemConfig);
}