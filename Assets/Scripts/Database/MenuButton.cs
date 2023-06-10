using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;
    public GameObject back;
    public void OpenMenu()
    {
        menu.SetActive(true);
        back.SetActive(false);
        gameObject.SetActive(false);
    }
}
