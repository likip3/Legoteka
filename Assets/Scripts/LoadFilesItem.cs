using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class LoadFilesItem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Button loadButton;
    [SerializeField]
    private Button deleteButton;
    public void Initialize(string name)
    {
        GetComponentInChildren<TMP_Text>().text = name;
        loadButton.onClick.AddListener(delegate { OnLoadClicked(name); });
        deleteButton.onClick.AddListener(delegate { OnDeleteClicked(name); });

        Permission.RequestUserPermissions(new string[2] { Permission.ExternalStorageWrite, Permission.ExternalStorageRead });

    }


    private void OnLoadClicked(string name)
    {
        FreeModeBrickPlacer.LoadBrickState(name, "/FreeModeSave/");
        gameObject.GetComponentInParent<LoadButtonsForFreemode>().ToggleSelf();
    }

    private void OnDeleteClicked(string name)
    {
        File.Delete(Application.persistentDataPath + "/FreeModeSave/" + name);
        gameObject.GetComponentInParent<LoadButtonsForFreemode>().UpdateList();
    }
}
