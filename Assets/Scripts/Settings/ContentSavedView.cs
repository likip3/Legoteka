using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContentSavedView : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab;
    private void Awake() => UpdateList();


    public void UpdateList() => StartCoroutine(UpdateListCoroutine());
    public IEnumerator UpdateListCoroutine()
    {
        foreach (var GM in GameObject.FindGameObjectsWithTag("UIContentItem"))
        {
            Destroy(GM);
        }
        yield return new WaitForSeconds(.2f);
        var y = 0;
        foreach (var item in Directory.GetFiles(Application.persistentDataPath + "/FreeModeSave/"))
        {
            var temp = Instantiate(itemPrefab, transform);
            temp.GetComponent<SaveFilesItem>().Initialize(item.Split('/').Last());
            temp.GetComponent<RectTransform>().localPosition = new Vector3(477, y);
            y -= 140;
        }
    }
}
