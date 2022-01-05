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
    public const string label = "SimpleSave";
    public static readonly HashSet<string> Keywords = new HashSet<string>(new[] {"SimpleSave", "SS", "Simple Save"});

    public string dataPath = "";
    public SerializationMethod serializationMethod = SerializationMethod.Json;
    public EncryptionMethod encryptionMethod = EncryptionMethod.Aes;
    
    public static SimpleSaveSettings GetOrCreateSettings()
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
        return new SerializedObject(GetOrCreateSettings());
    }
}

static class SimpleSaveSettingsIMGUIRegister
{
    [SettingsProvider]
    public static SettingsProvider CreateSimpleSaveSettingsProvider()
    {
        var provider = new SettingsProvider(SimpleSaveSettings.SettingsPath, SettingsScope.Project)
        {
            label = SimpleSaveSettings.label,
            guiHandler = (searchContext) =>
            {
                var settings = SimpleSaveSettings.GetSerializedSettings();
                EditorGUILayout.PropertyField(settings.FindProperty("dataPath"), new GUIContent("Data path"));
                EditorGUILayout.PropertyField(settings.FindProperty("serializationMethod"), new GUIContent("Serialization Method"));
                settings.ApplyModifiedPropertiesWithoutUndo();
            },
            keywords = SimpleSaveSettings.Keywords
        };

        return provider;

    }
}

static class SimpleSaveSettingsUIElementsRegister
{
    [SettingsProvider]
    public static SettingsProvider CreateSimpleSaveSettingsProvider()
    {
        var provider = new SettingsProvider(SimpleSaveSettings.SettingsPath, SettingsScope.Project)
        {
            label = SimpleSaveSettings.label,
            activateHandler = (searchContext, rootElement) =>
            {
                var settings = SimpleSaveSettings.GetSerializedSettings();
                
                var title = new Label()
                {
                    text = SimpleSaveSettings.label
                };
                rootElement.Add(title);

                var properties = new VisualElement()
                {
                };
                rootElement.Add(properties);

                properties.Add(new PropertyField(settings.FindProperty("dataPath")));
                rootElement.Bind(settings);
            },

            keywords = SimpleSaveSettings.Keywords
        };

        return provider;
    }
}

class SimpleSaveSettingsProvider : SettingsProvider
{
    private SerializedObject _simpleSaveSettings;

    class Styles
    {
        public static readonly GUIContent Script = new GUIContent("Data Path");
    }
    public SimpleSaveSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope) {}

    public static bool IsSettingsAvailable()
    {
        return File.Exists(SimpleSaveSettings.Path);
    }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        _simpleSaveSettings = SimpleSaveSettings.GetSerializedSettings();
    }

    public override void OnGUI(string searchContext)
    {
        EditorGUILayout.PropertyField(_simpleSaveSettings.FindProperty("dataPath"), Styles.Script);
        _simpleSaveSettings.ApplyModifiedPropertiesWithoutUndo();
    }

    [SettingsProvider]
    public static SettingsProvider CreateSimpleSaveSettingsProvider()
    {
        if (IsSettingsAvailable())
        {
            var provider = new SimpleSaveSettingsProvider(SimpleSaveSettings.SettingsPath + "Provider");
            provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
        }

        return null;
    }
}
