using System;
using System.Diagnostics.CodeAnalysis;

namespace XmasMod2025;

public struct IntMinMax : IEquatable<IntMinMax>
{
    public int Min;
    public int Max;

    public IntMinMax(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is IntMinMax other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Min, Max);
    }
    
    public static bool operator == (IntMinMax? a, object? b) => a.HasValue && a.Value.Equals(b);
    public static bool operator != (IntMinMax? a, object? b) => a.HasValue && a.Value.Equals(b);

    public static IntMinMax operator +(IntMinMax a, int b) => new(a.Min + b, a.Max + b);
    public static IntMinMax operator -(IntMinMax a, int b) => new(a.Min - b, a.Max - b);
    public static IntMinMax operator *(IntMinMax a, int b) => new(a.Min * b, a.Max * b);
    public static IntMinMax operator /(IntMinMax a, int b) => new(a.Min / b, a.Max / b);
    public static IntMinMax operator +(IntMinMax a, IntMinMax b) => new(a.Min + b.Min, a.Max + b.Max);
    public static IntMinMax operator -(IntMinMax a, IntMinMax b) => new(a.Min - b.Min, a.Max - b.Max);
    public static IntMinMax operator *(IntMinMax a, IntMinMax b) => new(a.Min * b.Min, a.Max * b.Max);
    public static IntMinMax operator /(IntMinMax a, IntMinMax b) => new(a.Min / b.Min, a.Max / b.Max);
    public static IntMinMax operator +(IntMinMax a, (int, int) b) => new(a.Min + b.Item1, a.Max + b.Item2);
    public static IntMinMax operator -(IntMinMax a, (int, int) b) => new(a.Min - b.Item1, a.Max - b.Item2);
    public static IntMinMax operator *(IntMinMax a, (int, int) b) => new(a.Min * b.Item1, a.Max * b.Item2);
    public static IntMinMax operator /(IntMinMax a, (int, int) b) => new(a.Min / b.Item1, a.Max / b.Item2);
    public bool Equals(IntMinMax other)
    {
        return Min == other.Min && Max == other.Max;
    }
}