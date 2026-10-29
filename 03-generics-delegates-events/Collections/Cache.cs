using System;
using System.Collections.Generic;
using GenericsDelegatesEvents.Events;

namespace GenericsDelegatesEvents.Collections;

/// <summary>
/// Узагальнений кеш з обмеженою місткістю та витісненням найстаріших записів.
/// </summary>
/// <typeparam name="TKey">Тип ключа (не може бути null)</typeparam>
/// <typeparam name="TValue">Тип збереженого значення</typeparam>
public class Cache<TKey, TValue> where TKey : notnull
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, TValue> _storage;
    private readonly List<TKey> _keysOrder;

    /// <summary>
    /// Подія витіснення найстарішого елемента при переповненні кешу.
    /// </summary>
    public event EventHandler<ItemRemovedEventArgs<TKey, TValue>>? ItemRemoved;

    public int Capacity => _capacity;
    public int Count => _storage.Count;

    public Cache(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Місткість кешу має бути більшою за нуль.");
        }

        _capacity = capacity;
        _storage = new Dictionary<TKey, TValue>(capacity);
        _keysOrder = new List<TKey>(capacity);
    }

    /// <summary>
    /// Отримує значення за ключем, або завантажує за допомогою делегата load та додає до кешу.
    /// Повертає значення та статус (чи було взято з кешу).
    /// </summary>
    public TValue GetOrAdd(TKey key, Func<TKey, TValue> load, out bool fromCache)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(load);

        if (_storage.TryGetValue(key, out TValue? cachedValue))
        {
            fromCache = true;
            return cachedValue;
        }

        fromCache = false;
        TValue loadedValue = load(key);

        // Якщо кеш заповнено, видаляємо найстаріший елемент перед додаванням нового
        if (_storage.Count >= _capacity)
        {
            EvictOldest();
        }

        _storage[key] = loadedValue;
        _keysOrder.Add(key);

        return loadedValue;
    }

    /// <summary>
    /// Перевантажений варіант GetOrAdd для стандартної сигнатури.
    /// </summary>
    public TValue GetOrAdd(TKey key, Func<TKey, TValue> load)
    {
        return GetOrAdd(key, load, out _);
    }

    /// <summary>
    /// Виконує вказану дію для кожного елемента кешу у порядку їх додавання.
    /// </summary>
    public void ForEach(Action<TKey, TValue> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        foreach (TKey key in _keysOrder)
        {
            if (_storage.TryGetValue(key, out TValue? value))
            {
                action(key, value);
            }
        }
    }

    /// <summary>
    /// Перевіряє наявність ключа в кеші.
    /// </summary>
    public bool ContainsKey(TKey key)
    {
        ArgumentNullException.ThrowIfNull(key);
        return _storage.ContainsKey(key);
    }

    /// <summary>
    /// Очищення кешу.
    /// </summary>
    public void Clear()
    {
        _storage.Clear();
        _keysOrder.Clear();
    }

    /// <summary>
    /// Видаляє найстаріший запис з кешу та викликає подію ItemRemoved.
    /// </summary>
    private void EvictOldest()
    {
        if (_keysOrder.Count == 0)
        {
            return;
        }

        TKey oldestKey = _keysOrder[0];
        _keysOrder.RemoveAt(0);

        if (_storage.Remove(oldestKey, out TValue? removedValue))
        {
            OnItemRemoved(new ItemRemovedEventArgs<TKey, TValue>(oldestKey, removedValue));
        }
    }

    /// <summary>
    /// Безпечний виклик події ItemRemoved за стандартним шаблоном .NET.
    /// </summary>
    protected virtual void OnItemRemoved(ItemRemovedEventArgs<TKey, TValue> e)
    {
        ItemRemoved?.Invoke(this, e);
    }
}
