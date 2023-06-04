using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PanelToggle : MonoBehaviour
{
    public List<GameObject> Panels;

    public void TogglePanel()
    {
        foreach (var panel in Panels)
            panel.gameObject.SetActive(!panel.gameObject.activeSelf);
    }
}