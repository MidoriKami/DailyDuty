using System.ComponentModel;
using Dalamud.Utility;

namespace DailyDuty.Data.ModuleData.HuntMarks
{
    public enum ExpansionType
    {    
        [Description("A Realm Reborn")]
        RealmReborn,

        [Description("Heavensward")]
        Heavensward,

        [Description("Stormblood")]
        Stormblood,

        [Description("Shadowbringers")]
        Shadowbringers,

        [Description("Endwalker")]
        Endwalker
    }

    public static class ExpansionTypeExtensions
    {
        public static string Description(this ExpansionType value)
        {
            return value.GetAttribute<DescriptionAttribute>().Description;
        }
    }
}
