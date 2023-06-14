using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorMenu : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer platform;
    [SerializeField]
    private GameObject colorButtonPreset;
    [SerializeField]
    private RectTransform container;
    [SerializeField]
    private GameObject colorPicker;
    [SerializeField]
    private GameObject customButton;
    [SerializeField]
    private GameObject background;

    public List<Color> colorList;
    public void OpenMenu()
    {
        gameObject.SetActive(true);
        UpdateColorList();
        CameraControll.movingState = false;
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
        colorPicker.SetActive(false);
        CameraControll.movingState = true;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        //if (!colorPicker.activeInHierarchy) return;

    }

    public void UpdateColorList()
    {
        var deffPos = 310;
        foreach (var color in colorList)
        {
            var temp = Instantiate(colorButtonPreset, container);
            temp.GetComponent<Button>().onClick.AddListener(delegate {
                SetPlatformColor(color);
            });
            temp.GetComponent<RectTransform>().localPosition = new Vector2(deffPos, 0);
            temp.GetComponent<Image>().color = color;
            deffPos += 230;
            container.sizeDelta = new Vector2(container.rect.width + 230, container.rect.height);
        }
    }

    public void onCustomColorPicker()
    {
        colorPicker.SetActive(true);
        platform.material.color = customButton.GetComponent<Image>().color;

    }

    public void SetPlatformColor(Color color)
    {
        platform.material.color = color;
    }

    public void ChangeCustomButtonColor(Color color)
    {
        customButton.GetComponent<Image>().color = color;
    }
}
