using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class Subscribable<T>
{
    private T value;
    public event Action OnValueChanged;

    public Subscribable(T value) => this.value = value;

    public Subscribable()
    {

    }

    public T Value
    {
        get { return value; }
        set
        {
            this.value = value;
            OnValueChanged?.Invoke();
        }
    }
}

public class SubscribableQueue<T> : IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>
{
    private Queue<T> queue = new();
    public event Action OnQueueChanged;

    public void Enqueue(T value)
    {
        queue.Enqueue(value);
        OnQueueChanged?.Invoke();
    }
    public T Dequeue()
    {
        T item = queue.Dequeue();
        OnQueueChanged?.Invoke();
        return item;
    }
    public void Clear()
    {
        queue.Clear();
        OnQueueChanged?.Invoke();
    }

    public void TrimExcess()
    {
        queue.TrimExcess();
        OnQueueChanged?.Invoke();
    }

    public bool TryDequeue(out T result)
    {
        bool success = queue.TryDequeue(out result);
        OnQueueChanged?.Invoke();
        return success;
    }

    public bool TryPeek(out T result) => queue.TryPeek(out result);
    public T Peek() => queue.Peek();
    public bool Contains(T value) => queue.Contains(value);
    public int Count => queue.Count;
    public bool IsSynchronized => ((ICollection)queue).IsSynchronized;
    public object SyncRoot => ((ICollection)queue).SyncRoot;
    public void CopyTo(Array array, int index) => ((ICollection)queue).CopyTo(array, index);
    public IEnumerator<T> GetEnumerator() => queue.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => queue.GetEnumerator();
}

public class SubscribableStack<T> : IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>
{
    private Stack<T> stack = new();
    public event Action OnStackChanged;

    public int Count => stack.Count;
    public bool IsSynchronized => ((ICollection)stack).IsSynchronized;
    public object SyncRoot => ((ICollection)stack).SyncRoot;
    public void CopyTo(Array array, int index) => ((ICollection)stack).CopyTo(array, index);
    public T Peek() => stack.Peek();

    public void Clear()
    {
        stack.Clear();
        OnStackChanged?.Invoke();
    }
    public void Push(T item)
    {
        stack.Push(item);
        OnStackChanged?.Invoke();
    }

    public T Pop()
    {
        T item = stack.Pop();
        OnStackChanged?.Invoke();
        return item;
    }

    public IEnumerator<T> GetEnumerator() => stack.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => stack.GetEnumerator();
}

public class SubscribableList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>
{
    private List<T> list = new();
    public event Action OnListChanged;

    public T this[int index]
    {
        get => list[index];
        set
        {
            list[index] = value;
            OnListChanged?.Invoke();
        }
    }

    public int Count => list.Count;

    public bool IsReadOnly => ((ICollection<T>)list).IsReadOnly;

    public bool IsFixedSize => ((IList)list).IsFixedSize;

    public bool IsSynchronized => ((ICollection)list).IsSynchronized;

    public object SyncRoot => ((ICollection)list).SyncRoot;

    object IList.this[int index]
    {
        get => (IList)list[index];
        set
        {
            ((IList)list)[index] = value;
            OnListChanged?.Invoke();
        }
    }

    public void Add(T item)
    {
        list.Add(item);
        OnListChanged?.Invoke();
    }

    public void Clear()
    {
        list.Clear();
        OnListChanged?.Invoke();
    }

    public void RemoveAll(Predicate<T> match)
    {
        list.RemoveAll(match);
        OnListChanged?.Invoke();
    }

    public bool Contains(T item) => list.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

    public int IndexOf(T item) => list.IndexOf(item);

    public void Insert(int index, T item)
    {
        list.Insert(index, item);
        OnListChanged?.Invoke();
    }

    public bool Remove(T item)
    {
        bool success = list.Remove(item);
        OnListChanged?.Invoke();
        return success;
    }

    public void RemoveAt(int index)
    {
        list.RemoveAt(index);
        OnListChanged?.Invoke();
    }

    IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();

    public int Add(object value)
    {
        int index = ((IList)list).Add(value);
        OnListChanged?.Invoke();
        return index;
    }

    public bool Contains(object value) => ((IList)list).Contains(value);

    public int IndexOf(object value) => ((IList)list).IndexOf(value);

    public void Insert(int index, object value)
    {
        ((IList)list).Insert(index, value);
        OnListChanged?.Invoke();
    }

    public void Remove(object value)
    {
        ((IList)list).Remove(value);
        OnListChanged?.Invoke();
    }

    public void CopyTo(Array array, int index) => ((IList)list).CopyTo(array, index);
}

public class SubscribableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, ISerializable, IDeserializationCallback
{
    private Dictionary<TKey, TValue> dict = new();
    public event Action OnDictionaryChanged;

    public TValue this[TKey key]
    {
        get => dict[key];
        set
        {
            dict[key] = value;
            OnDictionaryChanged?.Invoke();
        }
    }
    public object this[object key]
    {
        get => ((IDictionary)dict)[key];
        set
        {
            ((IDictionary)dict)[key] = value;
            OnDictionaryChanged?.Invoke();
        }
    }

    public ICollection<TKey> Keys => dict.Keys;

    public ICollection<TValue> Values => dict.Values;

    public int Count => dict.Count;

    public bool IsReadOnly => ((ICollection<TKey>)dict).IsReadOnly;

    public bool IsFixedSize => ((IDictionary)dict).IsFixedSize;

    public bool IsSynchronized => ((ICollection)dict).IsSynchronized;

    public object SyncRoot => ((ICollection)dict).SyncRoot;

    ICollection IDictionary.Keys => dict.Keys;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => dict.Keys;

    ICollection IDictionary.Values => dict.Values;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => dict.Values;

    public void Add(TKey key, TValue value)
    {
        dict.Add(key, value);
        OnDictionaryChanged?.Invoke();
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)dict).Add(item);
        OnDictionaryChanged?.Invoke();
    }

    public void Add(object key, object value)
    {
        ((IDictionary)dict).Add(key, value);
        OnDictionaryChanged?.Invoke();
    }

    public void Clear()
    {
        dict.Clear();
        OnDictionaryChanged?.Invoke();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)dict).Contains(item);

    public bool Contains(object key) => ((IDictionary)dict).Contains(key);

    public bool ContainsKey(TKey key) => dict.ContainsKey(key);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)dict).CopyTo(array, arrayIndex);

    public void CopyTo(Array array, int index) => ((ICollection)dict).CopyTo(array, index);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dict.GetEnumerator();

    public void GetObjectData(SerializationInfo info, StreamingContext context) => dict.GetObjectData(info, context);

    public void OnDeserialization(object sender)
    {
        dict.OnDeserialization(sender);
        OnDictionaryChanged?.Invoke();
    }

    public bool Remove(TKey key)
    {
        bool success = dict.Remove(key);
        OnDictionaryChanged?.Invoke();
        return success;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        bool success = ((ICollection<KeyValuePair<TKey, TValue>>)dict).Remove(item);
        OnDictionaryChanged?.Invoke();
        return success;
    }

    public void Remove(object key)
    {
        ((IDictionary)dict).Remove(key);
        OnDictionaryChanged?.Invoke();
    }

    public bool TryGetValue(TKey key, out TValue value) => dict.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumerator();

    IDictionaryEnumerator IDictionary.GetEnumerator() => dict.GetEnumerator();
}