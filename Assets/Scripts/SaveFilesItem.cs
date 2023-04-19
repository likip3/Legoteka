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

    }

    private void OnDeleteClicked(string name)
    {
        File.Delete(Application.persistentDataPath + "/FreeModeSave/" + name + ".lgt");
        gameObject.GetComponentInParent<ContentSavedView>().UpdateList();
    }
}
