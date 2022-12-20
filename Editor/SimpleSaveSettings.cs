using System;using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class SimpleSaveSettings : ScriptableObject
{
    public const string Path = "Assets/ProjectSettings/SimpleSaveSettings.asset";
    public const string SettingsPath = "Project/SimpleSaveSettings";
    public const string Label = "SimpleSave";
    public static readonly HashSet<string> Keywords = new HashSet<string>(new[] {"SimpleSave", "SS", "Simple Save"});

    public string dataPath = "";
    public SerializationMethod serializationMethod = SerializationMethod.Json;
    public EncryptionMethod encryptionMethod = EncryptionMethod.Aes;
    public CompressionMethod compressionMethod = CompressionMethod.GZip;
    public string password = "";
    
    public static SimpleSaveSettings Get()
    {
        var settings = AssetDatabase.LoadAssetAtPath<SimpleSaveSettings>(Path);

        if (settings == null)
        {
            settings = CreateInstance<SimpleSaveSettings>();
            AssetDatabase.CreateAsset(settings, Path);
            AssetDatabase.SaveAssets();
        }

        return settings;
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(Get());
    }
}

static class SimpleSaveSettingsIMGUIRegister
{
    [SettingsProvider]
    public static SettingsProvider CreateSimpleSaveSettingsProvider()
    {
        var provider = new SettingsProvider(SimpleSaveSettings.SettingsPath, SettingsScope.Project)
        {
            label = SimpleSaveSettings.Label,
            guiHandler = (searchContext) =>
            {
                var settings = SimpleSaveSettings.GetSerializedSettings();
                EditorGUILayout.PropertyField(settings.FindProperty("dataPath"), new GUIContent("Data path"));
                EditorGUILayout.PropertyField(settings.FindProperty("serializationMethod"), new GUIContent("Serialization Method"));
                EditorGUILayout.PropertyField(settings.FindProperty("encryptionMethod"), new GUIContent("Encryption Method"));
                if (settings.FindProperty("encryptionMethod").enumValueIndex == Decimal.One)
                {
                    EditorGUILayout.PropertyField(settings.FindProperty("password"), new GUIContent("Password"));
                }
                settings.ApplyModifiedPropertiesWithoutUndo();
            },
            keywords = SimpleSaveSettings.Keywords
        };

        return provider;

    }
}
