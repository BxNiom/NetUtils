namespace BxNiom.Collections.BPlusTree;

internal class TreeNodeContext<TKey> where TKey : class, IComparable {
    public enum FreeState {
        Stored,
        NotOwned,
        PoolFull
    }

    private readonly TreeNodePool<TKey> _pool;

    public TreeNodeContext(int poolSize) : this(new TreeNodePool<TKey>(poolSize)) { }

    public TreeNodeContext(TreeNodePool<TKey> pool) {
        _pool = pool;
    }

    public TreeNode<TKey> NewNode() {
        var node = _pool.NewNode();
        node.Context = this;
        return node;
    }

    public FreeState FreeNode(TreeNode<TKey> node) {
        if (!ReferenceEquals(node.Context, this)) {
            return FreeState.NotOwned;
        }

        node.Free();
        return _pool.Enqueue(node) ? FreeState.Stored : FreeState.PoolFull;
    }
}