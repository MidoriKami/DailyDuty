using System;
using Dalamud.Game;

namespace DailyDuty.System;

public class DailyDutySystem : IDisposable
{
    public readonly ModuleController ModuleController;
    private readonly AddonController addonController;
    
    public DailyDutySystem()
    {
        ModuleController = new ModuleController();
        addonController = new AddonController();
        
        if (Service.ClientState.IsLoggedIn)
        {
            OnLogin(this, EventArgs.Empty);
        }
        
        Service.Framework.Update += OnFrameworkUpdate;
        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
        Service.ClientState.TerritoryChanged += OnZoneChange;
        AddonController.AddonSetup += OnAddonSetup;
    }

    public void Dispose()
    {
        Service.Framework.Update -= OnFrameworkUpdate;
        Service.ClientState.Login -= OnLogin;
        Service.ClientState.Logout -= OnLogout;
        Service.ClientState.TerritoryChanged -= OnZoneChange;
        AddonController.AddonSetup -= OnAddonSetup;

        ModuleController.Dispose();
        addonController.Dispose();
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
    }
    
    private void OnLogin(object? sender, EventArgs e)
    {
        ModuleController.LoadModules();
    }
    
    private void OnLogout(object? sender, EventArgs e)
    {
        ModuleController.UnloadModules();
    }
    
    private void OnZoneChange(object? sender, ushort territoryTypeId)
    {
        ModuleController.ZoneChange(territoryTypeId);
    }
    
    private void OnAddonSetup(SetupAddonArgs addonInfo)
    {
        ModuleController.AddonSetup(addonInfo);
    }
}