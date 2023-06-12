using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LoadButtonsForFreemode : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private Transform container;

    private void OnEnable() => UpdateList();


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
            temp.GetComponent<LoadFilesItem>().Initialize(item.Remove(item.Length - 4).Split('/').Last());
            temp.GetComponent<RectTransform>().localPosition = new Vector3(477, y);
            y -= 140;
        }
    }


    public void ToggleSelf()
    {
        if(gameObject.activeInHierarchy)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
}
