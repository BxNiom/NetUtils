namespace BxNiom.Collections;

public class Bag<T> {
    private T?[] _data;

    public Bag(int capacity = 64) {
        _data = new T[capacity];
    }

    public int Size     { get; private set; }
    public int Capacity => _data.Length;

    public bool Empty => Size == 0;

    public T? this[int index] {
        get => _data[index];
        set {
            if (index >= _data.Length) {
                Grow(index * 2);
            }

            Size         = System.Math.Max(Size, index + 1);
            _data[index] = value;
        }
    }

    public T? Remove(int index) {
        var item = _data[index];
        _data[index] = _data[--Size];
        _data[Size]  = default;
        return item;
    }

    public T? PopLast() {
        if (Size > 0) {
            var item = _data[--Size];
            _data[Size] = default;
            return item;
        }

        return default;
    }

    public bool Remove(T item) {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        for (var i = 0; i < Size; i++) {
            var item2 = _data[i];

            if (item.Equals(item2)) {
                _data[i]    = _data[--Size]; // overwrite item to remove with last element
                _data[Size] = default;       // null last element, so gc can do its work
                return true;
            }
        }

        return false;
    }

    public bool Contains(T item) {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        for (var i = 0; Size > i; i++) {
            if (item.Equals(_data[i])) {
                return true;
            }
        }

        return false;
    }

    public bool IsIndexWithinBounds(int index) {
        return index < Capacity;
    }

    public void Add(T item) {
        if (Size == _data.Length) {
            Grow();
        }

        _data[Size++] = item;
    }

    public void Clear() {
        for (var i = 0; i < Size; i++) {
            _data[i] = default;
        }

        Size = 0;
    }

    private void Grow() {
        var newCapacity = _data.Length * 3 / 2 + 1;
        Grow(newCapacity);
    }

    private void Grow(int newCapacity) {
        var oldData = _data;
        _data = new T[newCapacity];
        Array.Copy(_data, oldData, oldData.Length);
    }
}