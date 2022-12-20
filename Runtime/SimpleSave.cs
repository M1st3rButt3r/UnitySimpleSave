using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

public static class SimpleSave
{
    private static string DataPath
    {
        get => Path.Combine(Application.persistentDataPath,  SimpleSaveSettings.Get().dataPath);
    }
    private static string _currentPath = "";
    private static Buffer _buffer = new Buffer();

    public static void Set(string key, object value)
    {
        _buffer.Set(key, value);
    }

    public static bool HasKey(string key)
    {
        return _buffer.HasKey(key);
    }

    public static T Get<T>(string key)
    {
        return _buffer.Get<T>(key);
    }
    
    public static void Save(string filename)
    {
        if (filename == "") return;
        _currentPath = Application.persistentDataPath + "/" + filename;
        Stream stream = new FileStream(_currentPath, FileMode.Create);
        Stream cryptoStream = stream.Encrypt(SimpleSaveSettings.Get().encryptionMethod);
        Stream compStream = cryptoStream.Compress(SimpleSaveSettings.Get().compressionMethod);
        compStream.Serialize(_buffer, SimpleSaveSettings.Get().serializationMethod);
        compStream.Close();
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
        Stream stream = new FileStream(_currentPath, FileMode.Open);
        _buffer = stream.Decompress(SimpleSaveSettings.Get().compressionMethod).Decrypt(SimpleSaveSettings.Get().encryptionMethod).Deserialize<Buffer>(SimpleSaveSettings.Get().serializationMethod);
    }
    
    public static void Clear()
    {
        _currentPath = "";
        _buffer = new Buffer();
    }
}