using System.Text.RegularExpressions;

namespace BxNiom.Text;

public static class RegexExtensions {
    public static bool TryGetGroupValue(this Match match, string group, out string value) {
        if (match.Groups.ContainsKey(group)) {
            value = match.Groups[group].Value;
            return true;
        }

        value = "";
        return false;
    }

    public static string GetGroupValue(this Match match, string group, string defaultValue = "") {
        return match.Groups.ContainsKey(group) ? match.Groups[group].Value : defaultValue;
    }

    public static bool TryGetGroupValueAsShort(this Match match, string group, out short value) {
        if (match.Groups.ContainsKey(group)) {
            value = match.GetGroupValueAsShort(group);
            return true;
        }

        value = default;
        return false;
    }

    public static short GetGroupValueAsShort(this Match match, string group, short defaultValue = 0) {
        return match.Groups.ContainsKey(group) && short.TryParse(match.Groups[group].Value, out var i)
            ? i
            : defaultValue;
    }

    public static bool TryGetGroupValueAsInt(this Match match, string group, out int value) {
        if (match.Groups.ContainsKey(group)) {
            value = match.GetGroupValueAsInt(group);
            return true;
        }

        value = default;
        return false;
    }

    public static int GetGroupValueAsInt(this Match match, string group, int defaultValue = 0) {
        return match.Groups.ContainsKey(group) && int.TryParse(match.Groups[group].Value, out var i)
            ? i
            : defaultValue;
    }

    public static bool TryGetGroupValueAsLong(this Match match, string group, out long value) {
        if (match.Groups.ContainsKey(group)) {
            value = match.GetGroupValueAsLong(group);
            return true;
        }

        value = default;
        return false;
    }

    public static long GetGroupValueAsLong(this Match match, string group, long defaultValue = 0) {
        return match.Groups.ContainsKey(group) && long.TryParse(match.Groups[group].Value, out var i)
            ? i
            : defaultValue;
    }

    public static bool TryGetGroupValueAsSingle(this Match match, string group, out float value) {
        if (match.Groups.ContainsKey(group)) {
            value = match.GetGroupValueAsSingle(group);
            return true;
        }

        value = default;
        return false;
    }

    public static float GetGroupValueAsSingle(this Match match, string group, float defaultValue = 0) {
        return match.Groups.ContainsKey(group) && float.TryParse(match.Groups[group].Value, out var i)
            ? i
            : defaultValue;
    }

    public static bool TryGetGroupValueAsDouble(this Match match, string group, out double value) {
        if (match.Groups.ContainsKey(group)) {
            value = match.GetGroupValueAsDouble(group);
            return true;
        }

        value = default;
        return false;
    }

    public static double GetGroupValueAsDouble(this Match match, string group, double defaultValue = 0) {
        return match.Groups.ContainsKey(group) && double.TryParse(match.Groups[group].Value, out var i)
            ? i
            : defaultValue;
    }

    public static bool TryGetGroupValueAsDateTime(this Match match, string group, out DateTime value) {
        if (match.Groups.ContainsKey(group)) {
            value = match.GetGroupValueAsDateTime(group);
            return true;
        }

        value = default;
        return false;
    }

    public static DateTime GetGroupValueAsDateTime(this Match match, string group) {
        return match.Groups.ContainsKey(group) && DateTime.TryParse(match.Groups[group].Value, out var i)
            ? i
            : DateTime.Now;
    }
}