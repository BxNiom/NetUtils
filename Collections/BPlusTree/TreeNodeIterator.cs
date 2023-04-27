namespace BxNiom.Collections.BPlusTree;

internal static class TreeNodeIterator {
    public static bool Iterate<TKey>(TreeNode<TKey> node,
                                     IterateDirection direction,
                                     TKey? startKey, TKey? endKey,
                                     bool includeStart, ref bool hit,
                                     Func<TreeItem<TKey>, bool> callback) where TKey : class, IComparable {
        var index = 0;

        switch (direction) {
            case IterateDirection.Ascend:
                if (startKey != null) {
                    node.Items.TryFind(startKey, out index);
                }

                for (var i = index; i < node.Items.Count; i++) {
                    if (node.Children.Count > 0 &&
                        !node.Children[i].Iterate(direction, startKey, endKey, includeStart, ref hit, callback)) {
                        return false;
                    }

                    if (!includeStart && !hit && startKey != null && startKey.CompareTo(node.Items[i].Key) != -1) {
                        hit = true;
                        continue;
                    }

                    hit = true;
                    if (endKey != null && endKey.CompareTo(node.Items[i].Key) != 1) {
                        return false;
                    }

                    if (!callback(node.Items[i])) {
                        return false;
                    }
                }

                if (node.Children.Count > 0 &&
                    !node.Children[^1].Iterate(direction, startKey, endKey, includeStart, ref hit, callback)) {
                    return false;
                }

                break;
            case IterateDirection.Descend:
                break;
            default:
                break;
        }

        return true;
    }
}