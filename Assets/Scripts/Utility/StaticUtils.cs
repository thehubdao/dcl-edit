using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class StaticUtils
{

    public static void Forall<T>(this IEnumerable<T> Ts,Action<T> action)
    {
        foreach (var t in Ts)
        {
            action.Invoke(t);
        }
    }

    public static IEnumerable<T> AsSingleInstanceInEnumerable<T>(this T value)
    {
        yield return value;
    }

    public static T Next<T>(this T src) where T : Enum
    {
        if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

        T[] arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(arr, src) + 1;
        return (arr.Length==j) ? arr[0] : arr[j];            
    }

    public static string ToHumanName(this string s)
    {
        var retVal = "";

        for (var i = 0; i < s.Length; i++)
        {
            if (i == 0)
            {
                retVal += s[i];
                continue;
            }

            var currentChar = s[i];
            var lastChar = s[i-1];

            if (Char.IsUpper(currentChar) && Char.IsLower(lastChar))
            {
                retVal += " " + currentChar;
            }
            else if(currentChar == '_')
            {
                retVal += " ";
            }
            else
            {
                retVal += currentChar;
            }
        }

        return retVal;
    }
}
