using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuItemsLoader : MonoBehaviour
{
    [SerializeField]
    private List<MainMenuItem> mainMenuItems;
    private List<MainMenuItem> customMainMenuItems;

    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private RectTransform itemContainer;

    public List<MainMenuItem> MainMenuItems => mainMenuItems;
    public List<MainMenuItem> CustomMenuItems => customMainMenuItems;

    private void Awake()
    {
        var curPos = 1074;
        foreach (var item in mainMenuItems)
        {
            var tempInst = Instantiate(itemPrefab, itemContainer);

            tempInst.GetComponent<RectTransform>().localPosition = new Vector3(curPos, -10);
            itemContainer.sizeDelta = new Vector2(itemContainer.rect.width + 856, 0);
            curPos += 856;

            tempInst.GetComponent<Image>().color = item.Background;
            tempInst.GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene(item.SceneName); });

            if (item.InstructionURL is not null && item.InstructionURL.Length ==0)
                tempInst.transform.GetChild(0).gameObject.SetActive(false);
            else
                tempInst.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { Application.OpenURL(item.InstructionURL); });

            tempInst.transform.GetChild(2).GetComponent<Text>().text = item.Name;
            tempInst.transform.GetChild(3).GetComponent<Image>().sprite = item.Image;
        }
    }


    [System.Serializable]
    public class MainMenuItem
    {
        [SerializeField] private string name;
        [SerializeField] private Sprite image;
        [SerializeField] private string sceneName;
        [SerializeField] private Color backgroundColor;
        [SerializeField] private string instructionLink;

        public string Name => name;
        public Sprite Image => image;
        public string SceneName => sceneName;
        public Color Background => backgroundColor;
        public string InstructionURL => instructionLink;

        public MainMenuItem(string name, Sprite image, string sceneName, string instructionLink)
        {
            this.name = name;
            this.image = image;
            this.sceneName = sceneName;
            this.instructionLink = instructionLink;
        }

        public MainMenuItem(string name, Sprite image, string sceneName)
        {
            this.name = name;
            this.image = image;
            this.sceneName = sceneName;
            instructionLink = null;
        }
    }
}
