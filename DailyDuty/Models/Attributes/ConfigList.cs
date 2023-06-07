using System;
using System.Reflection;
using DailyDuty.Interfaces;
using KamiLib.AutomaticUserInterface;

namespace DailyDuty.Models.Attributes;

public class ConfigList : DrawableAttribute
{
    public ConfigList(string category, int group) : base(null, category, group) { }
    
    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null)
    {
        var configList = GetValue<IConfigDrawable>(obj, field);

        if (saveAction is not null)
        {
            configList.Draw(saveAction);
        }
    }
}