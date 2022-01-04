using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using UnityEngine;

public enum SerializationMethod
{
    Json,
    Byte,
    Xml
}

public static class SimpleSerialize
{
    public static readonly Dictionary<SerializationMethod, Action<string, object>> Serialize =
        new Dictionary<SerializationMethod, Action<string, object>>
        {
            {SerializationMethod.Json, SerializeAsJson},
            {SerializationMethod.Byte, SerializeAsByte},
            {SerializationMethod.Xml, SerializeAsXml}
        };
    
    public static readonly Dictionary<SerializationMethod, Func<string, object>> Deserialize =
        new Dictionary<SerializationMethod, Func<string, object>>
        {
            {SerializationMethod.Json, SerializeFromJsonTo<Buffer>},
            {SerializationMethod.Byte, SerializeFromByteTo<Buffer>},
            {SerializationMethod.Xml, SerializeFromXmlTo<Buffer>}
        };
    
    public static void SerializeAsByte(string path, object obj)
    {
        var formatter = new BinaryFormatter();
        var stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, obj);
        stream.Close();
    }

    public static T SerializeFromByteTo<T>(string path) where T : class 
    {
        var formatter = new BinaryFormatter();
        var stream = new FileStream(path, FileMode.Open);
        return formatter.Deserialize(stream) as T;
    }

    public static void SerializeAsJson(string path, object obj)
    {
        var serializer = new DataContractJsonSerializer(obj.GetType());
        var stream = new FileStream(path, FileMode.Create);
        serializer.WriteObject(stream, obj);
        stream.Close();
    }
    
    public static T SerializeFromJsonTo<T>(string path) where T : class
    {
        var writer = new DataContractJsonSerializer(typeof(T));
        var stream = new FileStream(path, FileMode.Open);
        return writer.ReadObject(stream) as T;
    }
    
    public static void SerializeAsXml(string path, object obj)
    {
        var writer = new XmlSerializer(obj.GetType());
        var stream = new FileStream(path, FileMode.Create);
        writer.Serialize(stream, obj);
        stream.Close();
    }
    
    public static T SerializeFromXmlTo<T>(string path) where T : class 
    {
        var writer = new XmlSerializer(typeof(T));
        var stream = new FileStream(path, FileMode.Open);
        return writer.Deserialize(stream) as T;
    }
}
