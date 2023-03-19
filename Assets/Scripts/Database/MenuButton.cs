using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;
    public void OpenMenu()
    {
        menu.SetActive(true);
        gameObject.SetActive(false);
    }
}
