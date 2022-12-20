using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

[Serializable]
public class GenericSaveModel : SimpleSaveModel
{
    public Type type;
    public PropertyInfo[] propertyKeys;
    public object[] propertyValues;
    public FieldInfo[] fieldKeys;
    public object[] fieldValues;
    
    public override void Set(object obj)
    {
        type = obj.GetType();
        SetProperties(type, obj);
        SetFields(type, obj);
    }

    private void SetProperties(Type type, object obj)
    {
        List<PropertyInfo> keys = new List<PropertyInfo>();
        List<object> values = new List<object>();
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo property in properties)
        {
            if (property.PropertyType.IsSerializable && property.GetMethod.IsPublic && property.SetMethod.IsPublic)
            {
                values.Add(property.GetValue(obj, null));
                keys.Add(property);
                //indexer do not work
            }
            //else create generic save model for property
        }

        propertyKeys = keys.ToArray();
        propertyValues = values.ToArray();
    }

    private void SetFields(Type type, object obj)
    {
        List<FieldInfo> keys = new List<FieldInfo>();
        List<object> values = new List<object>();
        FieldInfo[] fields = type.GetFields();
        foreach (FieldInfo field in fields)
        {
            if (field.FieldType.IsSerializable && field.FieldType.IsPublic)
            {
                values.Add(field.GetValue(obj));
                keys.Add(field);
            }
            //else create generic save model for property
        }

        fieldKeys = keys.ToArray();
        fieldValues = values.ToArray();
    }

    public override T Get<T>()
    {
        object obj = Activator.CreateInstance<T>();
        obj = GetProperties(obj);
        obj = GetFields(obj);
        return (T)obj;
    }

    private object GetProperties(object obj)
    {
        for (int i = 0; i < propertyKeys.Length; i++)
        {
            propertyKeys[i].SetValue(obj, propertyValues[i]);
        }

        return obj;
    }

    private object GetFields(object obj)
    {
        for (int i = 0; i < fieldKeys.Length; i++)
        {
            fieldKeys[i].SetValue(obj, fieldValues[i]);
        }

        return obj;
    }
}
