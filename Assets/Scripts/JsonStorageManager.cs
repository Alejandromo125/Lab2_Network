using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonStorageManager : MonoBehaviour
{
    
    public void SaveJsonFile(string fileName, string json, string saveDirectory)
    {
        string dir = Application.persistentDataPath + saveDirectory;

        if(!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        File.WriteAllText(dir + fileName, json);

        Debug.Log("New json file saved: " + dir + fileName);
    }

    public string LoadJsonFile(string fileName, string saveDirectory)
    {
        string fullPath = Application.persistentDataPath + saveDirectory + fileName;
        string json = "...";

        if (File.Exists(fullPath))
        {
            json = File.ReadAllText(fullPath);
        }
        else
        {
            Debug.Log("Save Json file does not exist");
        }

        return json;
    }
}
