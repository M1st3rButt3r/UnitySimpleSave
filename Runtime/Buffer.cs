using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Buffer
{
    public SimpleSaveModel[] Values = {};
    public string[] Keys = {};

    public Dictionary<Type, SimpleSaveModel> SaveModels = new Dictionary<Type, SimpleSaveModel>
    {
    };

    public void Set(string key, object value)
    {
        SimpleSaveModel saveModel;
        if (!SaveModels.ContainsKey(value.GetType()))
        {
            saveModel = new GenericSaveModel();
            saveModel.Set(value);
        }
        else
        {
            
            saveModel = (SimpleSaveModel)SaveModels[value.GetType()].Clone();
            saveModel.Set(value);
        }


        List<string> newKeys = Keys.ToList();
        newKeys.Add(key);
        Keys = newKeys.ToArray();
        
        List<SimpleSaveModel> newValues = Values.ToList();
        newValues.Add(saveModel);
        Values = newValues.ToArray();
    }

    public T Get<T>(string key)
    {
        int i = Keys.ToList().IndexOf(key);
        return Values[i].Get<T>();
    }

    public bool HasKey(string key)
    {
        return Keys.Contains(key);
    }
}
