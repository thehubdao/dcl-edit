using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;

public static class StaticUtils
{
    public static Guid GuidFromString(string str)
    {
        var bytes = new List<byte>();
        var filler = 0;
        var filled = 0;
        foreach (var c in str)
        {
            filler |= (c % 64) << filled;
            filled += 6;
            if (filled >= 8)
            {
                bytes.Add((byte) filler);
                filler >>= 8;
                filled -= 8;
            }
        }

        if (bytes.Count > 16)
        {
            // remove everything after the 16th byte
            bytes.RemoveRange(16, bytes.Count - 16);
        }

        if (bytes.Count < 16)
        {
            // add new bytes up to 16
            bytes.AddRange(new byte[16 - bytes.Count]);
        }

        var guid = new Guid(bytes.ToArray());

        return guid;
    }

    public static uint FillToAlignment(uint currentCount, uint alignment = 4)
    {
        return (alignment - (currentCount % alignment)) % alignment;
    }
}
