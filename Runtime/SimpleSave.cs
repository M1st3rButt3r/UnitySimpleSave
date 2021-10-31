using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SimpleSave
{
    private static string _currentPath;
    private static Dictionary<string, object> buffer = new Dictionary<string, object>();

    public static void Save(string filename)
    {
        _currentPath = Application.persistentDataPath + "/" + filename;
        RemoveNonSerializableTypes();
        var formatter = new BinaryFormatter();
        var stream = new FileStream(_currentPath, FileMode.Create);
        formatter.Serialize(stream, buffer);
        stream.Close();
    }

    private static void RemoveNonSerializableTypes()
    {
        var toRemove = new List<string>();
        foreach (var data in buffer)
        {
            if (data.Value.GetType().IsSerializable) return;
            toRemove.Add(data.Key);
            Debug.LogWarning($"[Simple Save] Data with the key '{data.Key}' is not serializable");
        }
        toRemove.ForEach(key => { buffer.Remove(key); });
    }
}