using DailyDuty.ConfigurationSystem;

namespace DailyDuty.System.Modules
{
    internal class CustomDeliveriesModule : Module
    {
        private Weekly.CustomDeliveriesSettings Settings => Service.Configuration.CharacterSettingsMap[Service.Configuration.CurrentCharacter].CustomDeliveriesSettings;

        public CustomDeliveriesModule()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void Dispose()
        {
        }

        public override bool IsCompleted()
        {
            return false;
        }
    }
}
