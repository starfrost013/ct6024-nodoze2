using System.Collections.Generic;

// https://softwareengineering.stackexchange.com/questions/319264/dictionary-of-dictionaries-design-in-c
// Nice syntactical sugar!
internal class NAryDictionary<TKey, TValue> :
    Dictionary<TKey, TValue>
{
}

internal class NAryDictionary<TKey1, TKey2, TValue> :
    Dictionary<TKey1, NAryDictionary<TKey2, TValue>>
{
}

internal class NAryDictionary<TKey1, TKey2, TKey3, TValue> :
    Dictionary<TKey1, NAryDictionary<TKey2, TKey3, TValue>>
{
}