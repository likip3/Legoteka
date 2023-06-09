using UnityEngine;
using UnityEngine.UI;

public class CanvasToggle : MonoBehaviour
{
    public Canvas canvas;
    public static bool isInSettings;

    public void Start()
    {
        if (isInSettings)
        {
            ToggleCanvas();
            isInSettings = false;
        }
    }


    public void ToggleCanvas()
    {
        canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
    }
}