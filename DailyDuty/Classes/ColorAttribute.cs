using System;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;

namespace DailyDuty.Classes;

public class ColorAttribute(KnownColor internalColor) : Attribute {
	private KnownColor InternalColor { get; } = internalColor;
	public Vector4 Color => InternalColor.Vector();
}