using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Buffer
{
    public object[] Values = {};
    public string[] Keys = {};


    public void Set(string key, object value)
    {
        if (!value.GetType().IsSerializable)
        {
            Debug.Log($"[Simple Save] Type '{value.GetType()}' is not serializable!");
            return;
        }
        List<string> newKeys = Keys.ToList();
        newKeys.Add(key);
        Keys = newKeys.ToArray();
        
        List<object> newValues = Values.ToList();
        newValues.Add(value);
        Values = newValues.ToArray();
    }

    public T Get<T>(string key)
    {
        try
        {
            int i = Keys.ToList().IndexOf(key);
            return (T) Values[i];
        }
        catch
        {
            Debug.Log($"[Simple Save] Key '{key}' does not exist as {typeof(T)}!");
            return default;
        }
    }

    public bool HasKey(string key)
    {
        return Keys.Contains(key);
    }
}
