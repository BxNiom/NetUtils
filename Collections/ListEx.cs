namespace BxNiom.Collections;

public static class ListEx {
    public static void Truncate<T>(this List<T> list, int index) {
        var count = list.Count - index;
        if (count > 0) {
            list.RemoveRange(index, count);
        }
    }

    public static void Push<T>(this List<T> list, T item) {
        list.Insert(0, item);
    }

    public static T? Pop<T>(this List<T> list) {
        if (list.Count == 0) {
            return default;
        }

        var idx    = list.Count - 1;
        var result = list[idx];
        list.RemoveAt(idx);
        return result;
    }

    public static T? RemoveFirst<T>(this List<T> list) {
        if (list.Count == 0) {
            return default;
        }

        var item = list[0];
        list.RemoveAt(0);
        return item;
    }

    public static void RemoveAt<T>(this List<T> list, int index, out T? item) {
        if (list.Count < index) {
            item = default;
        } else {
            item = list[index];
            list.RemoveAt(index);
        }
    }
}