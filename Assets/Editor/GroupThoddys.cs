using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System.IO;

public class GroupThoddys : EditorWindow
{
    private AddressableAssetSettings settings;
    private string baseAssetPath;
    private int fileCount = 1000;
    private int bundleSize = 20;
    public int offset = 0;

    [MenuItem("Window/Thoddy Addressables")]
    static void Init()
    {
        var window = GetWindow<GroupThoddys>();
        window.Show();
    }

    private static void AddressableAssetSettings_OnModificationGlobal(AddressableAssetSettings setting, AddressableAssetSettings.ModificationEvent triggeredEvent, object obj)
    {
        Debug.Log(triggeredEvent.ToString());
    }

    private void OnGUI()
    {
        settings = EditorGUILayout.ObjectField(settings, typeof(AddressableAssetSettings)) as AddressableAssetSettings;
        baseAssetPath = EditorGUILayout.TextField(baseAssetPath);
        fileCount = EditorGUILayout.IntField(new GUIContent("File Count: "), fileCount);
        bundleSize = EditorGUILayout.IntField(new GUIContent("Bundle Size: "), bundleSize);
        offset = EditorGUILayout.IntField(new GUIContent("Folder offset: "), offset);

        if (GUILayout.Button("Create asset bundle group"))
        {
            AddressableAssetSettings.OnModificationGlobal += AddressableAssetSettings_OnModificationGlobal;

            if (!Directory.Exists(baseAssetPath))
            {
                Debug.LogError("Could not find: " + baseAssetPath);
                return;
            }

            for (int i = 0; i < fileCount; i += bundleSize)
            {
                var groupID = offset + i;

                var group = settings.FindGroup(groupID.ToString());
                if (group == null)
                    group = settings.CreateGroup("New Group " + groupID.ToString(), false, false, true, settings.DefaultGroup.Schemas);


                var entriesAdded = new List<AddressableAssetEntry>();
                for (int j = 0; j < bundleSize; j++)
                {
                    var assetPath = Path.Combine(baseAssetPath, (offset + i + j + 1).ToString() + ".png");
                    var guid = AssetDatabase.AssetPathToGUID(assetPath);

                    var e = settings.CreateOrMoveEntry(guid, group, false, false);
                    if (e == null)
                    {
                        Debug.LogError("Missing oddy asset: " + (offset + i + j + 1).ToString());
                        continue;
                    }
                    e.address = assetPath;
                    entriesAdded.Add(e);
                }

                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true);
                Debug.Log("Creating asset bundle group: " + i.ToString());
            }

            AddressableAssetSettings.OnModificationGlobal -= AddressableAssetSettings_OnModificationGlobal;
        }
    }

    private void OnDestroy()
    {
        AddressableAssetSettings.OnModificationGlobal -= AddressableAssetSettings_OnModificationGlobal;
    }
}
