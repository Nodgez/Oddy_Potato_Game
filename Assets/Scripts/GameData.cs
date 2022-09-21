#if UNITY_EDITOR
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class GameData : MonoBehaviour
{

    //[MenuItem("DATA/Clear All")]
    //static void ClearAllData()
    //{
    //	PlayerPrefs.DeleteAll();
    //	File.Delete(Application.persistentDataPath + Path.DirectorySeparatorChar + "userdata.dat");
    //}

    [MenuItem("DATA/Clear Prefs")]
    static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Lighting/Force Reflection Probe Render")]
    static void BakeReflectionProbe()
    {
        var selected = Selection.activeObject as GameObject;
        if (selected == null)
            return;

        var probe = selected.GetComponent<ReflectionProbe>();
        probe.RenderProbe();
    }

    [MenuItem("GameObject/Remove Duplicate Numeration")]
    static void RemoveDuplicateNumeration()
    {
        var selected = Selection.objects;
        if (selected == null)
        {
            Debug.Log("No Valid Selection");
            return;
        }

        string pattern = @"\([\s\S]*?\)";

        foreach (var sel in selected)
            sel.name = Regex.Replace(sel.name, pattern, string.Empty);

    }
    //private static string SelectedTag = "Player";

    //[MenuItem("Helpers/Select By Tag")]
    //public static void SelectObjectsWithTag()
    //{
    //	GameObject[] objects = GameObject.FindGameObjectsWithTag(SelectedTag);
    //	Selection.objects = objects;
    //}

    //	[MenuItem("DATA/Unlock all Levels")]
    //	static void UnlockAllLevels()
    //	{
    //		var codes = Enum.GetValues(typeof(RegionCode));
    //		UserSaveData.Awake();
    //		UserSaveData data = UserSaveData.defaultData;
    //		foreach (RegionCode code in codes) {
    //			data.SetInt(code.ToString(), 7);
    //		}
    //		data.WriteSaveDataDefault();
    //	}
}
#endif