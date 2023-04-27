namespace BxNiom.Collections.BPlusTree;

internal class TreeItemList<TKey> : List<TreeItem<TKey>> where TKey : class, IComparable {
    private static TreeItemComparer<TKey> Comparer { get; } = new();

    private int InternalSearch(TKey value) {
        var n1 = 0;
        var n2 = Count - 1;
        while (n1 <= n2) {
            var i1 = n1 + ((n2 - n1) >> 1);
            var n3 = Comparer.Compare(this[i1].Key, value);
            if (n3 == 0) {
                return i1;
            }

            if (n3 < 0) {
                n1 = i1 + 1;
            } else {
                n2 = i1 - 1;
            }
        }

        return ~n1;
    }

    public bool TryFind(TKey? key, out int index) {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        index = InternalSearch(key);
        var found = index >= 0;
        if (!found) {
            index = ~index;
        }

        if (index > 0 && this[index - 1].IsGreater(key)) {
            index--;
            return true;
        }

        return found;
    }

    public override string ToString() {
        return string.Join(", ", from i in this select i.Key);
    }
}