// using System;
// using System.Collections.Generic;
// using System.Drawing;
// using System.Globalization;
// using System.Reflection;
// using DailyDuty.Models.Attributes;
// using DailyDuty.System.Localization;
// using Dalamud.Interface;
// using ImGuiNET;
// using KamiLib.Utilities;
//
// namespace DailyDuty.Views.Components;
//
// public static class ModuleDataView
// {
//     public static void Draw(List<(FieldInfo, DataDisplay)> fields, object sourceObject)
//     {
//         if(fields.Count > 0)
//         {
//             ImGui.Text(Strings.ModuleData);
//             ImGui.Separator();
//             
//             ImGuiHelpers.ScaledIndent(15.0f);
//
//             if (ImGui.BeginTable("##DataTable", 2, ImGuiTableFlags.SizingStretchSame))
//             {
//                 foreach (var (field, attribute) in fields)
//                 {
//                     DrawGenericData(sourceObject, field, attribute);
//                 }
//                 
//                 ImGui.EndTable();
//             }
//
//             ImGuiHelpers.ScaledDummy(10.0f);
//             ImGuiHelpers.ScaledIndent(-15.0f);
//         }
//     }
//     
//     private static void DrawGenericData(object sourceObject, FieldInfo field, DataDisplay attribute)
//     {
//         ImGui.TableNextColumn();
//         ImGui.Text(attribute.Label);
//
//         ImGui.TableNextColumn();
//         switch (Type.GetTypeCode(field.FieldType))
//         {
//             case TypeCode.Boolean:
//                 var boolValue = (bool) field.GetValue(sourceObject)!;
//                 
//                 ImGui.Text(boolValue.ToString());
//                 break;
//
//             case TypeCode.String:
//                 var stringValue = (string) field.GetValue(sourceObject)!;
//
//                 ImGui.Text(stringValue);
//                 break;
//             
//             case TypeCode.Int32:
//                 var intValue = (int) field.GetValue(sourceObject)!;
//                 
//                 ImGui.Text(intValue.ToString());
//                 break;
//             
//             case TypeCode.UInt32:
//                 var uIntValue = (uint) field.GetValue(sourceObject)!;
//                 
//                 ImGui.Text(uIntValue.ToString());
//                 break;
//             
//             case TypeCode.Single:
//                 var floatValue = (float) field.GetValue(sourceObject)!;
//                 
//                 ImGui.Text(floatValue.ToString(CultureInfo.CurrentCulture));
//                 break;
//             
//             case TypeCode.DateTime:
//                 var dateTime = (DateTime) field.GetValue(sourceObject)!;
//                     
//                 ImGui.Text(dateTime.ToLocalTime().ToString(CultureInfo.CurrentCulture));
//                 break;
//             
//             case TypeCode.Object:
//                 if (field.FieldType == typeof(List<int>))
//                 {
//                     var list = (List<int>) field.GetValue(sourceObject)!;
//
//                     if (list.Count > 0)
//                     {
//                         foreach (var value in list)
//                         {
//                             ImGui.Text($"{value:0000}");
//                         }
//                     }
//                     else
//                     {
//                         ImGui.TextColored(KnownColor.Orange.AsVector4(), Strings.NothingToTrack);
//                     }
//                 }
//                 else if (field.FieldType == typeof(TimeSpan))
//                 {
//                     var value = (TimeSpan) field.GetValue(sourceObject)!;
//
//                     if (value > TimeSpan.MinValue)
//                     {
//                         ImGui.Text($"{value.Days:0}.{value.Hours:00}:{value.Minutes:00}:{value.Seconds:00}");
//                     }
//                     else
//                     {
//                         ImGui.Text(Strings.TimeNotAvailable);
//                     }
//                 }
//                 break;
//
//             default:
//                 ImGui.Text("Error: Unable to Read Type");
//                 break;
//         }
//     }
// }