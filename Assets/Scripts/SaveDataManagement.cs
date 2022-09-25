using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveDataManagement
{
    private static SaveDataManagement instance;

    public static SaveDataManagement Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SaveDataManagement(Application.persistentDataPath);
            }
            return instance;
        }
    }

    private string saveDirectory;
    public SaveDataManagement(string saveDirectory)
    {
        this.saveDirectory = saveDirectory;
    }

    public void SaveList<T>(string fileName, List<T> data)
    {
        var filePath = Path.Combine(Application.persistentDataPath, fileName);
        var dataAsJSON = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, dataAsJSON);
    }

    public void LoadList<T>(string fileName)
    {
        var filePath = Path.Combine(Application.persistentDataPath, fileName);
        var unParsedData = File.ReadAllText(filePath);
        var list = JsonUtility.FromJson<List<T>>(unParsedData);
    }
}
