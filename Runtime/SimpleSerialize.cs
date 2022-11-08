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
    public static readonly Dictionary<SerializationMethod, Action<Stream, object>> Serializer =
        new Dictionary<SerializationMethod, Action<Stream, object>>
        {
            {SerializationMethod.Json, SerializeAsJson},
            {SerializationMethod.Byte, SerializeAsByte},
            {SerializationMethod.Xml, SerializeAsXml}
        };
    
    public static readonly Dictionary<SerializationMethod, Func<Stream, object>> Deserializer =
        new Dictionary<SerializationMethod, Func<Stream, object>>
        {
            {SerializationMethod.Json, DeserializeFromJsonTo<Buffer>},
            {SerializationMethod.Byte, DeserializeFromByteTo<Buffer>},
            {SerializationMethod.Xml, DeserializeFromXmlTo<Buffer>}
        };

    public static void Serialize(this Stream stream, object obj, SerializationMethod serializationMethod)
    {
        Serializer[serializationMethod].Invoke(stream, obj);
    }

    public static T Deserialize<T>(this Stream stream, SerializationMethod serializationMethod) where T : class
    {
        switch (serializationMethod)
        {
            case SerializationMethod.Byte:
                return DeserializeFromByteTo<T>(stream);
            default:
                return default;
        }
    }
    
    public static void SerializeAsByte(this Stream stream, object obj)
    {
        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, obj);
    }

    public static T DeserializeFromByteTo<T>(this Stream stream) where T : class 
    {
        var formatter = new BinaryFormatter();
        return formatter.Deserialize(stream) as T;
    }

    public static void SerializeAsJson(this Stream stream, object obj)
    {
        var serializer = new DataContractJsonSerializer(obj.GetType());
        serializer.WriteObject(stream, obj);
    }
    
    public static T DeserializeFromJsonTo<T>(this Stream stream) where T : class
    {
        var serializer = new DataContractJsonSerializer(typeof(T));
        return serializer.ReadObject(stream) as T;
    }
    
    public static void SerializeAsXml(this Stream stream, object obj)
    {
        var serializer = new XmlSerializer(obj.GetType());
        serializer.Serialize(stream, obj);
    }
    
    public static T DeserializeFromXmlTo<T>(this Stream stream) where T : class
    {
        var serializer = new XmlSerializer(typeof(T));
        return serializer.Deserialize(stream) as T;
    }
}