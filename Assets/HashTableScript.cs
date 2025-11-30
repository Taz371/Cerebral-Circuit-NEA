using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HashTableScript<T1,T2>
{
    LinkedListScript<Tuple<T1, T2>>[] keyValuePairs;

    private int length;
    public int count;

    public HashTableScript()
    {
        length = 12;
        count = 0;

        keyValuePairs = new LinkedListScript<Tuple<T1, T2>>[length];

        for (int i = 0; i < length; i++)
        {
            keyValuePairs[i] = new LinkedListScript<Tuple<T1, T2>>();
        }
    }

    public int HashValue(T1 key)
    {
        int hash = key.GetHashCode();
        int calculatedIndex = Math.Abs(hash) % length;
        return calculatedIndex;
    }

    public void Put(T1 key, T2 value)
    {
        int index = HashValue(key);
        var keyValuePair = keyValuePairs[index];

        for (int i = 0;i < keyValuePair.count; i++)
        {
            var keyValue = keyValuePair[i];
            if (EqualityComparer<T1>.Default.Equals(keyValue.Item1, key))
            {
                keyValuePair[i] = Tuple.Create(key, value);
                return;
            }
        }

        keyValuePair.AddLast(Tuple.Create(key, value));
        count++;
    }

    public T2 get(T1 key)
    {
        int index = HashValue(key);
        var keyValuePair = keyValuePairs[index];

        for (int i = 0; i < keyValuePair.count; i++)
        {
            var keyValue = keyValuePair[i];
            if (EqualityComparer<T1>.Default.Equals(keyValue.Item1, key))
            {
                return keyValue.Item2;
            }
        }

        return default(T2);
    }

    public bool ContainsKey(T1 key)
    {
        int index = HashValue(key);
        var keyValuePair = keyValuePairs[index];

        for (int i = 0; i < keyValuePair.count; i++)
        {
            var keyValue = keyValuePair[i];
            if (EqualityComparer<T1>.Default.Equals(keyValue.Item1, key))
            {
                return true;
            }
        }

        return false;
    }
}
