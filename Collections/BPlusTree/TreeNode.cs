namespace BxNiom.Collections.BPlusTree;

internal partial class TreeNode<TKey> where TKey : class, IComparable {
    public TreeNode() {
        Items    = new TreeItemList<TKey>();
        Children = new List<TreeNode<TKey>>();
    }

    public TreeItemList<TKey>     Items    { get; }
    public List<TreeNode<TKey>>   Children { get; }
    public TreeNodeContext<TKey>? Context  { get; set; }

    public void Free() {
        Items.Clear();
        Children.Clear();
        Context = null;
    }

    public TreeNode<TKey> CloneFor(TreeNodeContext<TKey> ctx) {
        if (Context == null) {
            throw new NullReferenceException("no context");
        }

        if (ReferenceEquals(Context, ctx)) {
            return this;
        }

        var node = Context.NewNode();
        node.Items.AddRange(Items);
        node.Children.AddRange(Children);
        return node;
    }

    private TreeNode<TKey> CloneChild(int index) {
        if (Context == null) {
            throw new NullReferenceException("no context");
        }

        var child = Children[index].CloneFor(Context);
        Children[index] = child;
        return child;
    }

    public void Split(int index, out TreeItem<TKey> item, out TreeNode<TKey> node) {
        if (Context == null) {
            throw new NullReferenceException("no context");
        }

        item  =  Items[index];
        index += 1;
        node  =  Context.NewNode();
        node.Items.AddRange(Items.GetRange(index, Items.Count - index));

        Items.Truncate(index - 1);
        if (Children.Count > 0) {
            node.Children.AddRange(Children.GetRange(index, Children.Count - index));
            Children.Truncate(index);
        }
    }

    private bool CheckSplitChild(int index, int maxItems) {
        if (Children[index].Items.Count < maxItems) {
            return false;
        }

        var firstNode = CloneChild(index);
        firstNode.Split(maxItems / 2, out var item, out var secondNode);
        Items.Insert(index, item);
        Children.Insert(index + 1, secondNode);
        return true;
    }

    public TreeItem<TKey>? Insert(TreeItem<TKey> item, int maxItems) {
        if (Items.TryFind(item.Key, out var index)) {
            var foundItem = Items[index];
            Items[index] = item;
            return foundItem;
        }

        if (Children.Count == 0) {
            Items.Insert(index, item);
            return null;
        }

        if (CheckSplitChild(index, maxItems)) {
            var inTreeItem = Items[index];
            if (item.IsGreater(inTreeItem.Key)) {
                index++;
            } else {
                Items[index] = item;
                return inTreeItem;
            }
        }

        return CloneChild(index).Insert(item, maxItems);
    }

    public bool TryGet(TKey key, out TreeItem<TKey>? item) {
        item = null;
        if (Items.TryFind(key, out var index)) {
            item = Items[index];
            return true;
        } else if (Children.Count > 0) {
            return Children[index].TryGet(key, out item);
        }

        return false;
    }

    public TreeItem<TKey>? FirstItem() {
        TreeNode<TKey>? node = this;
        while (node.Children.Count > 0) {
            node = node.Children[0];
        }

        return node.Items.Count == 0 ? null : node.Items[0];
    }

    public TreeItem<TKey>? LastItem() {
        TreeNode<TKey>? node = this;
        while (node.Children.Count > 0) {
            node = node.Children[^1];
        }

        return node.Items.Count == 0 ? null : node.Items[^1];
    }

    public TreeItem<TKey>? Remove(TKey? key, int minItems, RemoveType removeType) {
        var index = 0;
        var found = false;

        switch (removeType) {
            case RemoveType.Min:
                if (Children.Count == 0) {
                    var min = Items[0];
                    Items.RemoveAt(0);
                    return min;
                }

                index = 0;
                break;
            case RemoveType.Max:
                if (Children.Count == 0) {
                    return Items.Pop();
                }

                index = Items.Count;
                break;
            case RemoveType.Item:
                found = Items.TryFind(key, out index);
                if (Children.Count == 0) {
                    if (!found) {
                        return null;
                    }

                    Items.RemoveAt(index, out var item);
                    return item;
                }

                break;
            default:
                throw new ArgumentException("unknown value", nameof(removeType));
        }

        if (Children[index].Items.Count <= minItems) {
            return GrownChildAndRemove(index, key, minItems, removeType);
        }

        var child = CloneChild(index);

        if (found) {
            var item = Items[index];
            Items[index] = child.Remove(default, minItems, RemoveType.Max) ?? throw new InvalidOperationException();
            return item;
        }

        return child.Remove(key, minItems, removeType);
    }

    private TreeItem<TKey>? GrownChildAndRemove(int index, TKey? key, int minItems, RemoveType removeType) {
        TreeNode<TKey> child;
        TreeNode<TKey> stealFrom;
        TreeItem<TKey> stolenItem;
        if (index > 0 && Children[index - 1].Items.Count > minItems) {
            child      = CloneChild(index);
            stealFrom  = CloneChild(index - 1);
            stolenItem = stealFrom.Items.Pop() ?? throw new InvalidOperationException();

            child.Items.Insert(0, Items[index - 1]);
            Items[index - 1] = stolenItem;
            if (stealFrom.Children.Count > 0) {
                child.Children.Insert(0, stealFrom.Children.Pop() ?? throw new InvalidOperationException());
            }
        } else if (index < Items.Count && Children[index + 1].Items.Count > minItems) {
            child      = CloneChild(index);
            stealFrom  = CloneChild(index + 1);
            stolenItem = stealFrom.Items.RemoveFirst() ?? throw new InvalidOperationException();

            child.Items.Add(Items[index]);
            Items[index] = stolenItem;
            if (stealFrom.Children.Count > 0) {
                child.Children.Add(stealFrom.Children.RemoveFirst() ?? throw new InvalidOperationException());
            }
        } else {
            if (index >= Items.Count) {
                index--;
            }

            child = CloneChild(index);
            Items.RemoveAt(index, out var mergeItem);
            Children.RemoveAt(index + 1, out var mergeChild);

            child.Items.Add(mergeItem!);
            child.Items.AddRange(mergeChild!.Items);
            child.Children.AddRange(mergeChild.Children);
            Context!.FreeNode(mergeChild);
        }

        return Remove(key, minItems, removeType);
    }

    public bool Reset(TreeNodeContext<TKey> ctx) {
        if (Children.Any(child => !child.Reset(ctx))) {
            return false;
        }

        return ctx.FreeNode(this) != TreeNodeContext<TKey>.FreeState.PoolFull;
    }

    public bool Iterate(IterateDirection direction,
                        TKey? startKey, TKey? endKey,
                        bool includeStart, ref bool hit,
                        Func<TreeItem<TKey>, bool> callback) {
        return TreeNodeIterator.Iterate(this, direction, startKey, endKey, includeStart, ref hit, callback);
    }

    public void DebugPrint(TextWriter writer, int level) {
        var levelStr = new string(' ', level + 2) + " | +-";
        writer.WriteLine($"{levelStr}(o) {Items}");
        foreach (var child in Children) {
            child.DebugPrint(writer, level + 1);
        }
    }
}