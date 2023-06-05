using System;
using System.Reflection;
using KamiLib.AutomaticUserInterface;
using KamiLib.Interfaces;

namespace DailyDuty.Models.Attributes;

public class DataList : DrawableAttribute
{
    public DataList() : base(null) { }

    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null)
    {
        var configList = GetValue<IDrawable>(obj, field);

        configList.Draw();
    }
}