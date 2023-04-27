using System.Collections;

namespace BxNiom.Collections.BPlusTree;

internal class TreeItemComparer<T> : IComparer<T> where T : IComparable {
    public int Compare(T? x, T? y) {
        if (x == null) {
            return 1;
        }

        if (y == null) {
            return -1;
        }

        return Comparer.Default.Compare(x, y);
    }
}