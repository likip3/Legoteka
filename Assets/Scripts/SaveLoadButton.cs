using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadButton : MonoBehaviour
{
    public void SaveScene() => SaveLoadSystem.SaveXml(FreeModeBrickPlacer.SceneToBrickCol(SceneManager.GetActiveScene().name), "/Story/");
    public void LoadScene() => FreeModeBrickPlacer.ToSceneFromBrickCol(SaveLoadSystem.DeXml(SceneManager.GetActiveScene().name, "/Story/"));
}
