using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UiControl : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                SceneManager.LoadScene("ExitMenu");
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }

    public void SceneSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void FreeModeScene()
    {
        SceneManager.LoadScene("FreeMode");
    }

    public void CrocodileWithHelper()
    {
        SceneManager.LoadScene("CrocodileWithHelper");
    }

    public void SceneCrocodileNoHelper()
    {
        SceneManager.LoadScene("CrocodileNoHelper");
    }

    public void SceneGiraffeNoHelper()
    {
        SceneManager.LoadScene("GiraffeNoHelper");
    }

    public void SceneHorseWithHelper()
    {
        SceneManager.LoadScene("HorseWithHelper");
    }

    public void SceneGiraffeWithHelper()
    {
        SceneManager.LoadScene("GiraffeWithHelper");
    }

    public void SceneChooseGiraffe()
    {
        SceneManager.LoadScene("ChooseGiraffe");
    }

    public void SceneChooseCrocodile()
    {
        SceneManager.LoadScene("ChooseCrocodile");
    }

    public void SceneExitMenu()
    {
        SceneManager.LoadScene("ExitMenu");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void CloseApp()
    {
        Application.Quit();
    }

    public void InstructionGiraffe()
    {
        Application.OpenURL("https://drive.google.com/file/d/1SGDF0PAxUhWIYtsG83w55ni_mP4lbCty/view?usp=sharing");
    }

    public void InstructionCrocodile()
    {
        Application.OpenURL("https://drive.google.com/file/d/1Z8lqTqnY2yJRu4-iSARx-kXwWcRuJ4yg/view?usp=sharing");
    }

    public void InstructionHorse()
    {
        Application.OpenURL("https://drive.google.com/file/d/1-TukMFR6-tnhCTYNOr8u9TuBdhVO8nd_/view?usp=sharing");
    }

    
}
