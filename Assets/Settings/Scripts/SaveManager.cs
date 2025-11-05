using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager I { get; private set; }
    string SavePath => Path.Combine(Application.persistentDataPath, "save1.json");

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("Save path: " + SavePath);
    }

    public void Save(SaveData data)
    {
        try
        {
            var json = JsonUtility.ToJson(data, false);
            var tmp = SavePath + ".tmp";
            File.WriteAllText(tmp, json);
            if (File.Exists(SavePath)) File.Delete(SavePath);
            File.Move(tmp, SavePath);
            Debug.Log("Saved.");
        }
        catch (System.Exception e) { Debug.LogError("Save failed: " + e); }
    }

    public bool TryLoad(out SaveData data)
    {
        data = null;
        try
        {
            var path = SavePath;
            if (!File.Exists(path)) return false;
            var json = File.ReadAllText(path);
            data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Loaded.");
            return data != null;
        }
        catch (System.Exception e) { Debug.LogError("Load failed: " + e); return false; }
    }

    public void DeleteSave()
    {
        var path = SavePath;
        if (File.Exists(path)) File.Delete(path);
        Debug.Log("Save deleted.");
    }

}
