using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NumberConverter
{
    static Dictionary<string, float> numbers = new Dictionary<string, float>{
            {"K", 1000 ^ 1},
            {"M", 1000 ^ 2},
            {"B", 1000 ^ 3},
            {"T", 1000 ^ 4},
            {"Q", 1000 ^ 5},
        };


    public static string FixNumber(this float number)
    {
        float value = 1;
        string key = "";

        foreach (var i in numbers)
        {
            if (i.Value > value && number >= i.Value)
            {
                key = i.Key;

                value = i.Value;
            }
        }

        return
        ((number / value).ToString() + key);
    }

    public static string FixNumber(this int number)
    {
        return FixNumber((float)number);
    }
}