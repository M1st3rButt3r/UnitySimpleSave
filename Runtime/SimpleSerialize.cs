using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;

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
            {SerializationMethod.Json, DeserializeFromJsonTo<Buffer>},
            {SerializationMethod.Byte, DeserializeFromByteTo<Buffer>},
            {SerializationMethod.Xml, DeserializeFromXmlTo<Buffer>}
        };
    
    public static void SerializeAsByte(string path, object obj)
    {
        var stream = new FileStream(path, FileMode.Create);
        using var cstream = SimpleEncryption.Writer[SimpleSaveSettings.GetOrCreateSettings().encryptionMethod].Invoke(stream);
        var formatter = new BinaryFormatter();
        formatter.Serialize(cstream, obj);
    }

    public static T DeserializeFromByteTo<T>(string path) where T : class 
    {
        var stream = new FileStream(path, FileMode.Open);
        using var cstream = SimpleEncryption.Reader[SimpleSaveSettings.GetOrCreateSettings().encryptionMethod].Invoke(stream);
        var formatter = new BinaryFormatter();
        return formatter.Deserialize(cstream) as T;
    }

    public static void SerializeAsJson(string path, object obj)
    {
        var stream = new FileStream(path, FileMode.Create);
        using var cstream = SimpleEncryption.Writer[SimpleSaveSettings.GetOrCreateSettings().encryptionMethod].Invoke(stream);
        var serializer = new DataContractJsonSerializer(obj.GetType());
        serializer.WriteObject(cstream, obj);
    }
    
    public static T DeserializeFromJsonTo<T>(string path) where T : class
    {
        var stream = new FileStream(path, FileMode.Open);
        using var cstream = SimpleEncryption.Reader[SimpleSaveSettings.GetOrCreateSettings().encryptionMethod].Invoke(stream);
        var serializer = new DataContractJsonSerializer(typeof(T));
        return serializer.ReadObject(cstream) as T;
    }
    
    public static void SerializeAsXml(string path, object obj)
    {
        using var stream = new FileStream(path, FileMode.Create);
        using var cstream = SimpleEncryption.Writer[SimpleSaveSettings.GetOrCreateSettings().encryptionMethod].Invoke(stream);
        var serializer = new XmlSerializer(obj.GetType());
        serializer.Serialize(cstream, obj);
    }
    
    public static T DeserializeFromXmlTo<T>(string path) where T : class
    {
        var stream = new FileStream(path, FileMode.Open);
        using var cstream = SimpleEncryption.Reader[SimpleSaveSettings.GetOrCreateSettings().encryptionMethod].Invoke(stream);
        var serializer = new XmlSerializer(typeof(T));
        return serializer.Deserialize(cstream) as T;
    }
}