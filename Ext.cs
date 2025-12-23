using System;

namespace XmasMod2025;

public static class Ext
{
    public static string FormatNumber(this double num)
    {
        if (num < 1000) return num.ToString("#.###");
        var zeros = (int)Math.Log10(num);
        if (zeros >= 50)
        {
            var newNum = Math.Round(num / (10 ^ zeros), 2);
            return newNum.ToString("#.##") + $"e{zeros}";
        }

        var index = (zeros - 3) / 3; // 1000 = 0, 10,000 = 0.33 (0), 100,000 = 0.67 (0), 1,000,000 = 1 (1)... 
        if (index < 0) return num.ToString("###.##");


        string[] suffixes = ["K", "M", "B", "T", "Qd", "Qn", "Sx", "Sp", "Oc", "No", "De", "UDe", "DDe"];

        return (num / (int)Math.Pow(10, zeros / 3 * 3)).ToString("#.##") +
               suffixes[index]; // 10,134,560,000,000 > 10.13T
    }
}