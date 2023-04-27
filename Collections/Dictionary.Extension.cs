namespace BxNiom.Collections;

public static class DictionaryEx {
    public static Dictionary<TKey, TValue> AppendDictionaries<TKey, TValue>(this Dictionary<TKey, TValue> left,
                                                                            IEnumerable<Dictionary<TKey, TValue>>
                                                                                dictionaries) where TKey : notnull {
        foreach (var kvp in dictionaries.SelectMany(x => x)) {
            if (!left.ContainsKey(kvp.Key)) {
                left.Add(kvp.Key, kvp.Value);
            }
        }

        return left;
    }

    public static Dictionary<TKey, TValue> AppendDictionary<TKey, TValue>(this Dictionary<TKey, TValue> left,
                                                                          Dictionary<TKey, TValue> right)
        where TKey : notnull {
        foreach (var kvp in right) {
            if (!left.ContainsKey(kvp.Key)) {
                left.Add(kvp.Key, kvp.Value);
            }
        }

        return left;
    }

    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> left,
                                                               Dictionary<TKey, TValue> right) where TKey : notnull {
        return new[] { left, right }.SelectMany(x => x)
            .ToLookup(pair => pair.Key, pair => pair.Value)
            .ToDictionary(g => g.Key, g => g.First());
    }
}