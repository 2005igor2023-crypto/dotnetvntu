using System;

namespace GenericsDelegatesEvents.Events;

/// <summary>
/// Аргументи події видалення елемента з кешу через досягнення ліміту місткості.
/// </summary>
/// <typeparam name="TKey">Тип ключа</typeparam>
/// <typeparam name="TValue">Тип значення</typeparam>
public class ItemRemovedEventArgs<TKey, TValue> : EventArgs
{
    public TKey Key { get; }
    public TValue Value { get; }
    public DateTime RemovedAt { get; }

    public ItemRemovedEventArgs(TKey key, TValue value)
    {
        Key = key;
        Value = value;
        RemovedAt = DateTime.Now;
    }
}
