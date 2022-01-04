using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SimpleSave
{
    private static string DataPath
    {
        get => Path.Combine(Application.persistentDataPath,  SimpleSaveSettings.GetOrCreateSettings().dataPath);
    }
    private static string _currentPath = "";
    private static Dictionary<string, object> _buffer = new Dictionary<string, object>();

    public static void Set(string key, object value)
    {
        if (!value.GetType().IsSerializable)
        {
            Debug.Log($"[Simple Save] Type '{value.GetType()}' is not serializable!");
            return;
        }
        _buffer.Add(key, value);
    }

    public static bool HasKey(string key)
    {
        return _buffer.ContainsKey(key);
    }

    public static T Get<T>(string key)
    {
        try
        {
            return (T) _buffer[key];
        }
        catch
        {
            Debug.Log($"[Simple Save] Key '{key}' does not exist as {typeof(T)}!");
            return default;
        }
    }
    
    public static void Save(string filename = "")
    {
        if (filename != "") _currentPath = Application.persistentDataPath + "/" + filename;
        else if (_currentPath == "") return;
        var formatter = new BinaryFormatter();
        var stream = new FileStream(_currentPath, FileMode.Create);
        formatter.Serialize(stream, _buffer);
        stream.Close();
    }

    public static void Delete(string filename)
    {
        string path = Application.persistentDataPath + "/" + filename;
        if (!File.Exists(path))
        {
            Debug.LogError($"[Simple Save] File '{path}' does not exist!");
            return;
        }
        File.Delete(path);
    }
    
    public static Dictionary<string, string> GetSaves()
    {
        if (!Directory.Exists(Application.persistentDataPath)) return new Dictionary<string, string>();
        
        string[] files = Directory.GetFiles(Application.persistentDataPath);
        var saves = new Dictionary<string, string>();
        foreach (var file in files)
        {
            string name = Path.GetFileNameWithoutExtension(@file);
            saves.Add(name, file);
        }

        return saves;
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
        _buffer = formatter.Deserialize(stream) as Dictionary<string, object>;
    }
    
    public static void Clear()
    {
        _currentPath = "";
        _buffer = new Dictionary<string, object>();
    }
}