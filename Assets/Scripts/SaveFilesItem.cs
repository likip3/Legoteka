using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveFilesItem : MonoBehaviour
{
    [SerializeField]
    private Button saveButton;
    [SerializeField]
    private Button deleteButton;
    public void Initialize(string name)
    {
        GetComponentInChildren<TMP_Text>().text = name;
        saveButton.onClick.AddListener(delegate { OnSaveClicked(name); });
        deleteButton.onClick.AddListener(delegate { OnDeleteClicked(name); });
    }


    private void OnSaveClicked(string name)
    {
#if UNITY_EDITOR
        File.Copy(Application.persistentDataPath + "/FreeModeSave/" + name, "C:/Users/%USERNAME%/Downloads");
#elif UNITY_ANDROID
        File.Copy(Application.persistentDataPath + "/FreeModeSave/" + name, "/storage/emulated/0/Download");
#endif
    }

    private void OnDeleteClicked(string name)
    {
        File.Delete(Application.persistentDataPath + "/FreeModeSave/" + name);
        gameObject.GetComponentInParent<ContentSavedView>().UpdateList();
    }
}
