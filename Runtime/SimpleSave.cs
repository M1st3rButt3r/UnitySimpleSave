using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SimpleSave
{
    private static string _currentPath = "";
    public static Dictionary<string, object> buffer = new Dictionary<string, object>();

    public static void Save(string filename = "")
    {
        if (filename != "") _currentPath = Application.persistentDataPath + "/" + filename;
        else if (_currentPath == "") return;
        RemoveNonSerializableTypes();
        var formatter = new BinaryFormatter();
        var stream = new FileStream(_currentPath, FileMode.Create);
        formatter.Serialize(stream, buffer);
        stream.Close();
    }

    public static void Load(string filename)
    {
        string path = Application.persistentDataPath + "/" + filename;
        if (!File.Exists(path))
        {
            Debug.LogError($"[Simple Save] File '{path}' does not exist!");
            return;
        }
        _currentPath = path;
        var formatter = new BinaryFormatter();
        var stream = new FileStream(_currentPath, FileMode.Open);
        buffer = formatter.Deserialize(stream) as Dictionary<string, object>;
    }

    private static void RemoveNonSerializableTypes()
    {
        var toRemove = new List<string>();
        foreach (var data in buffer)
        {
            if (data.Value.GetType().IsSerializable) return;
            toRemove.Add(data.Key);
            Debug.LogWarning($"[Simple Save] Data with the key '{data.Key}' is not serializable!");
        }
        toRemove.ForEach(key => { buffer.Remove(key); });
    }
}