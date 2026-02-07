namespace NuNuGet;

/// <summary>
/// Extension methods for <see cref="IDictionary{TKey, TValue}"/>.
/// </summary>
internal static class DictionaryExtensions
{
    /// <summary>
    /// Adds an item to a list in a dictionary, creating the list if it doesn't already exist.
    /// </summary>
    /// <remarks>
    /// This makes it easier to use an <see cref="IDictionary{TKey, TValue}"/> as a multi-valued dictionary.
    /// </remarks>
    /// <typeparam name="TKey">The type of key that the dictionary uses.</typeparam>
    /// <typeparam name="TValue">The type of value that the dictionary holds. This must implement <see cref="IList{T}"/>, and be constructable.</typeparam>
    /// <typeparam name="TValueItem">The type of item in the list.</typeparam>
    /// <param name="dictionary">The dictionary to add the item to.</param>
    /// <param name="key">The key of the list to add the item to.</param>
    /// <param name="item">The item to add to the list.</param>
    public static void AddItem<TKey, TValue, TValueItem>(this IDictionary<TKey, TValue> dictionary, TKey key, TValueItem item)
        where TValue : IList<TValueItem>, new()
    {
        if (dictionary.TryGetValue(key, out var list))
        {
            list.Add(item);
        }
        else
        {
            dictionary[key] = [item];
        }
    }
}
