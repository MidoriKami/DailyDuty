using System;
using System.Reflection;
using DailyDuty.Interfaces;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.Attributes;

public class ConfigList : DrawableAttribute
{
    public ConfigList() : base(null) { }
    
    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null)
    {
        var configList = GetValue<IConfigDrawable>(obj, field);

        if (saveAction is not null)
        {
            configList.Draw(saveAction);
        }
    }
}