using System.Collections.Concurrent;

namespace BxNiom.Collections.BPlusTree;

internal class TreeNodePool<TKey> where TKey : class, IComparable {
    private readonly int                             _poolSize;
    private readonly ConcurrentQueue<TreeNode<TKey>> _queue;

    public TreeNodePool(int poolSize) {
        _poolSize = poolSize;
        _queue    = new ConcurrentQueue<TreeNode<TKey>>();
    }

    public TreeNode<TKey> NewNode() {
        if (!_queue.TryDequeue(out var node)) {
            return new TreeNode<TKey>();
        }

        return node;
    }

    public bool Enqueue(TreeNode<TKey> node) {
        if (_queue.Count >= _poolSize) {
            return false;
        }

        _queue.Enqueue(node);
        return true;
    }
}

/*
   class Factory
   {
      // Maximum objects allowed!
      private static int _PoolMaxSize = 3;
      // My Collection Pool
      private static readonly Queue objPool = new
         Queue(_PoolMaxSize);
      public Student GetStudent()
      {
         Student oStudent;
         // Check from the collection pool. If exists, return
         // object; else, create new
         if (Student.ObjectCounter >= _PoolMaxSize &&
            objPool.Count > 0)
         {
            // Retrieve from pool
            oStudent = RetrieveFromPool();
         }
         else
         {
            oStudent = GetNewStudent();
         }
         return oStudent;
      }
      private Student GetNewStudent()
      {
         // Creates a new Student
         Student oStu = new Student();
         objPool.Enqueue(oStu);
         return oStu;
      }
      protected Student RetrieveFromPool()
      {
         Student oStu;
         // Check if there are any objects in my collection
         if (objPool.Count > 0)
         {
            oStu = (Student)objPool.Dequeue();
            Student.ObjectCounter--;
         }
         else
         {
            // Return a new object
            oStu = new Student();
         }
         return oStu;
      }
   }
   */