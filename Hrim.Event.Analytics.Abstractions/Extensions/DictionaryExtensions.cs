#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions.Extensions;

public static class DictionaryExtensions
{
    /// <summary> Checks that two dictionaries either both null or have the same keys and the same values </summary>
    public static bool EqualTo(this IDictionary<string, string>? left, IDictionary<string, string>? right) {
        if (left is null && right is null)
            return true;
        if (left is not null && left.Count > 0 && right is null ||
            right is not null && right.Count > 0 && left is null)
            return false;
        if (left is not null  && left.Count  == 0 && right is null ||
            right is not null && right.Count == 0 && left is null)
            return true;
        if (left!.Count != right!.Count)
            return false;
        foreach (var leftPair in left) {
            if (!right.ContainsKey(leftPair.Key))
                return false;
            if (right[leftPair.Key] != leftPair.Value)
                return false;
        }
        return true;
    }

    /// <summary> Checks that two dictionaries not equal </summary>
    public static bool NotEqualTo(this IDictionary<string, string>? left, IDictionary<string, string>? right)
        => !EqualTo(left, right);
}