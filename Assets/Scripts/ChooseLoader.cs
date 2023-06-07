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
    [SerializeField] private Image preview;

    private void Awake()
    {
        setName.text = SetLoaderStatic.setName;
        var tempMat = new Material(preview.material);
        tempMat.mainTexture = SetLoaderStatic.preview;
        preview.material = tempMat;


    }

}
