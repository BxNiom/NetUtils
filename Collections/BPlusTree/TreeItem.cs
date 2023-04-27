namespace BxNiom.Collections.BPlusTree;

public abstract class TreeItem<TKey> where TKey : class, IComparable {
    protected TreeItem(TKey key) {
        Key = key;
    }

    public TKey Key { get; }

    public bool IsLess(TKey other) {
        return Key.CompareTo(other) == -1;
    }

    public bool IsGreater(TKey other) {
        return Key.CompareTo(other) == 1;
    }
}