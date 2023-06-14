using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseLoader : MonoBehaviour
{
    public void onButtonInstruction() => SetLoaderStatic.instructionMode = true;
    public void onButtonNoInstruction() => SetLoaderStatic.instructionMode = false;
    public void onStartButton() => UnityEngine.SceneManagement.SceneManager.LoadScene("FreeMode");
    public void onInstructionButton() => UnityEngine.SceneManagement.SceneManager.LoadScene("InstructionViewer");


    [SerializeField] private Text setName;
    [SerializeField] private RawImage preview;
    [SerializeField] private Text instSetName;
    [SerializeField] private RawImage instPreview;
    [SerializeField] private RectTransform instContainer;
    [SerializeField] private GameObject templatePart;
    [SerializeField] private Transform objectHolder;

    private void Awake()
    {
        setName.text = SetLoaderStatic.setName;
        preview.texture = MenuItemsLoader.CreateSetPreview(SaveLoadSystem.DeXml(SetLoaderStatic.setName, "/FreeModeSave/"));

        instSetName.text = SetLoaderStatic.setName;
        instPreview.texture = preview.texture;


    }

    public void onFirstInstPresed()
    {
        objectHolder.SetLocalPositionAndRotation(new Vector3(0, -0.51f, 3.7f), Quaternion.Euler(new Vector3(-130, 0, 45)));
        LoadSetList(SetLoaderStatic.setName);
    }

    private void LoadSetList(string name)
    {
        Dictionary<string, int> counter = new();
        foreach (var brick in UIController.ToBrickDBItemList(SaveLoadSystem.DeXml(name, "/FreeModeSave/").BrickArray))
        {
            if (counter.ContainsKey(brick.ID))
                counter[brick.ID]++;
            else
                counter[brick.ID] = 1;
        }

        foreach (var item in SetLoaderStatic.GetBrickList(UIController.ToBrickDBItemList(SaveLoadSystem.DeXml(name, "/FreeModeSave/").BrickArray)))
        {
            var temp = Instantiate(templatePart, instContainer);
            temp.GetComponent<RawImage>().texture = item.RenderTexture;
            temp.GetComponentInChildren<Text>().text = "X" + counter[item.ID];
        }
    }
}
