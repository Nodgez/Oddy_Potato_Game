using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

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
        var dataAsJSON = JsonConvert.SerializeObject(data);
        Debug.Log("Saving llist data as:\n" + dataAsJSON);
        File.WriteAllText(filePath, dataAsJSON);
    }

    public List<string> LoadList(string fileName)
    {
        var filePath = Path.Combine(Application.persistentDataPath, fileName);
        var unParsedData = string.Empty;

        var list = new List<string>();
        if (!File.Exists(filePath))
            return list;
        else
        {
            unParsedData = File.ReadAllText(filePath);
            list = JsonConvert.DeserializeObject<List<string>>(unParsedData);
            return list;
        }
    }
}
