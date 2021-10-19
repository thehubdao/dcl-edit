using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

static class TSUtil
{
    public static string ToTS(this float f)
    {
        NumberFormatInfo nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        nfi.CurrencyGroupSeparator = "";
        return f.ToString(nfi);
    }
    public static string ToTS(this Vector3 v, bool addNewKeyword = true)
    {
        return (addNewKeyword ? "new " : "") + $"Vector3({v.x.ToTS()}, {v.y.ToTS()}, {v.z.ToTS()})";
    }
    public static string ToTS(this Quaternion q, bool addNewKeyword = true)
    {
        return (addNewKeyword ? "new " : "") + $"Quaternion({q.x.ToTS()}, {q.y.ToTS()}, {q.z.ToTS()}, {q.w.ToTS()})";
    }

    public static Vector3 OnGroundPlane(this Vector3 position)
    {
        return new Vector3(position.x,0, position.z);
    }

    public const float TileSpacing = 16;
    public static Vector2Int ToLandIndex(this Vector3 position)
    {
        return new Vector2Int(
            (int) Mathf.Floor(position.x / TileSpacing), 
            (int) Mathf.Floor(position.z / TileSpacing));
    }
    public static Vector3 ToWorldCoordinates(this Vector2Int position)
    {
        return new Vector3(position.x * TileSpacing + TileSpacing / 2, 0, position.y * TileSpacing + TileSpacing / 2);
    }
}