using DailyDuty.Configuration.Character.ModuleSettings;
using DailyDuty.Interfaces;
using DailyDuty.Modules.Enums;
using DailyDuty.UserInterface.Components;
using DailyDuty.UserInterface.Components.InfoBox;

namespace DailyDuty.Modules;

internal class DebugModule : IModule
{
    public static ModuleName Name => ModuleName.DebugModule;
    public static DebugModuleConfiguration Settings => Service.ConfigurationManager.CharacterConfiguration.DebugModule;
    public IConfigurationComponent ConfigurationComponent { get; }
    public IStatusComponent StatusComponent { get; }
    public ILogicComponent LogicComponent { get; }

    public DebugModule()
    {
        ConfigurationComponent = new ModuleConfigurationComponent(this);
        StatusComponent = new ModuleStatusComponent(this);
        LogicComponent = new ModuleLogicComponent(this);
    }

    private class ModuleConfigurationComponent : IConfigurationComponent
    {
        public IModule ParentModule { get; }
        public ISelectable Selectable => new ConfigurationSelectable(Name, this, Settings.Enabled);

        private readonly InfoBox generalSettings = new();

        public ModuleConfigurationComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public void Draw()
        {
            generalSettings
                .AddTitle("Debug Settings")
                .AddConfigCheckbox("Enabled", Settings.Enabled)
                .Draw();
        }
    }

    private class ModuleStatusComponent : IStatusComponent
    {
        public IModule ParentModule { get; }

        public ModuleStatusComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public ISelectable Selectable => new StatusSelectable(Name, this, Settings.Enabled, ParentModule.LogicComponent.GetModuleStatus);

        public void Draw()
        {

        }
    }

    private class ModuleLogicComponent : ILogicComponent
    {
        public IModule ParentModule { get; }

        public ModuleLogicComponent(IModule parentModule)
        {
            ParentModule = parentModule;
        }

        public ModuleStatus GetModuleStatus()
        {
            return ModuleStatus.Complete;
        }
    }
}