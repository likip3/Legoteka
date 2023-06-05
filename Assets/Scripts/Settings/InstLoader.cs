using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InstLoader : MonoBehaviour
{
    [SerializeField] private ContentSavedView savedList;
    public void LoadFromDownloads()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/FreeModeSave/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/FreeModeSave/");
        }


        foreach (var item in Directory.GetFiles("/storage/emulated/0/Download/"))
        {
            if (!item.Contains(".lgt")) continue;
            File.Copy(item, Application.persistentDataPath + "/FreeModeSave/");
        }
        savedList.UpdateList();
    }
}
