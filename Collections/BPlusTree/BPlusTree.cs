namespace BxNiom.Collections.BPlusTree;

public class BPlusTree<TKey, TItem> where TKey : class, IComparable where TItem : TreeItem<TKey> {
    private readonly int                   _maxItems;
    private readonly int                   _minItems;
    private readonly TreeNodeContext<TKey> _nodeContext;

    private TreeNode<TKey>? _rootNode;

    public BPlusTree(int degree, int nodePoolSize = 32) {
        if (degree <= 1) {
            throw new ArgumentException(nameof(degree));
        }

        Degree = degree;
        Length = 0;

        _rootNode    = null;
        _nodeContext = new TreeNodeContext<TKey>(nodePoolSize);

        _minItems = Degree - 1;
        _maxItems = Degree * 2 - 1;
    }

    public int Degree { get; }
    public int Length { get; private set; }

    public TItem? ReplaceOrInsert(TItem item) {
        if (item == null) {
            throw new ArgumentNullException(nameof(item));
        }

        if (_rootNode == null) {
            _rootNode = _nodeContext.NewNode();
            _rootNode.Items.Add(item);
            Length++;
            return null;
        } else {
            _rootNode = _rootNode.CloneFor(_nodeContext);
            if (_rootNode.Items.Count >= _maxItems) {
                _rootNode.Split(_maxItems / 2, out var splitItem, out var splitNode);
                var oldRoot = _rootNode;
                _rootNode = _nodeContext.NewNode();
                _rootNode.Items.Add(splitItem);
                _rootNode.Children.Add(oldRoot);
                _rootNode.Children.Add(splitNode);
            }
        }

        var result = _rootNode.Insert(item, _maxItems);
        if (result == null) {
            Length++;
        }

        return (TItem?)result;
    }

    public bool TryFindItem(TKey key, out TItem? item) {
        item = null;
        if (_rootNode != null && _rootNode.TryGet(key, out var treeItem)) {
            item = (TItem?)treeItem;
            return true;
        }

        return false;
    }

    public bool Contains(TKey key) {
        return TryFindItem(key, out _);
    }

    public TItem? Remove(TItem item) {
        return Remove(item.Key);
    }

    public TItem? Remove(TKey key) {
        return Remove(key, RemoveType.Item);
    }

    public TItem? RemoveMin() {
        return Remove(default, RemoveType.Min);
    }

    public TItem? RemoveMax() {
        return Remove(default, RemoveType.Min);
    }

    public void IterateAscendRange(TKey greaterOrEqual, TKey lessThan, Func<TreeItem<TKey>, bool> callback) {
        if (_rootNode == null) {
            return;
        }

        var hit = false;
        TreeNodeIterator.Iterate(_rootNode, IterateDirection.Ascend, greaterOrEqual, lessThan, true, ref hit, callback);
    }

    public void IterateAscendLessThan(TKey pivot, Func<TreeItem<TKey>, bool> callback) {
        if (_rootNode == null) {
            return;
        }

        var hit = false;
        TreeNodeIterator.Iterate(_rootNode, IterateDirection.Ascend, default, pivot, true, ref hit, callback);
    }

    private TItem? Remove(TKey? key, RemoveType removeType) {
        if (_rootNode == null || _rootNode.Items.Count == 0) {
            return null;
        }

        _rootNode = _rootNode.CloneFor(_nodeContext);
        var result = _rootNode.Remove(key, _minItems, removeType);
        if (_rootNode.Items.Count == 0 && _rootNode.Children.Count > 0) {
            var oldRoot = _rootNode;
            _rootNode = _rootNode.Children[0];
            _nodeContext.FreeNode(oldRoot);
        }

        if (result != null) {
            Length--;
        }

        return (TItem?)result;
    }

    public void DebugPrint(TextWriter writer) {
        if (_rootNode == null) {
            return;
        }

        _rootNode.DebugPrint(writer, 0);
    }
}