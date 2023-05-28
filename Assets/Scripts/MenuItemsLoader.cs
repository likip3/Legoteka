using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuItemsLoader : MonoBehaviour
{
    [SerializeField]
    private List<MainMenuItem> mainMenuItems;
    private List<MainMenuItem> customMainMenuItems;

    private int curPos;

    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private RectTransform itemContainer;

    public List<MainMenuItem> MainMenuItems => mainMenuItems;
    public List<MainMenuItem> CustomMenuItems => customMainMenuItems;

    private void Awake()
    {
        curPos = 1074;

        LoadItemsFromCol(mainMenuItems);

        customMainMenuItems = new();


        foreach (var item in Directory.GetFiles(Application.persistentDataPath + "/CustomStory/"))
        {
            var brickColl = SaveLoadSystem.DeXml(item.Remove(item.Length - 4).Split('/').Last(), "/CustomStory/");



            customMainMenuItems.Add(new MainMenuItem(brickColl.fileName, CreateSetPreview(brickColl), Color.Lerp(SQLiteTasker.GetColorById(brickColl.BrickArray[0].brickID), SQLiteTasker.GetColorById(brickColl.BrickArray[1].brickID),.4f)));
        }


        LoadItemsFromCol(customMainMenuItems);
    }

    public static RenderTexture CreateSetPreview(BrickCollectionXML brickColl)
    {
        var tempTransform = new GameObject("temp");
        tempTransform.transform.position = new Vector3(4.5f,.4f,30.5f);
        tempTransform.SetActive(false);
        FreeModeBrickPlacer.ToSceneFromBrickCol(brickColl, tempTransform.transform);
        var render = BrickDatabase.CreatePreviewRender(tempTransform, new Color());
        return render;
    }
    private void LoadItemsFromCol(List<MainMenuItem> menuItems) => LoadItemsFromCol(menuItems, false);
    private void LoadItemsFromCol(List<MainMenuItem> menuItems, bool loadCustomData)
    {
        foreach (var item in menuItems)
        {
            var tempInst = Instantiate(itemPrefab, itemContainer);

            tempInst.GetComponent<RectTransform>().localPosition = new Vector3(curPos, -10);
            itemContainer.sizeDelta = new Vector2(itemContainer.rect.width + 856, 0);
            curPos += 856;

            tempInst.GetComponent<Image>().color = item.Background;

            if (loadCustomData)
            {
                tempInst.GetComponent<Button>().onClick.AddListener(delegate {
                    SetLoaderStatic.enabled = true;
                    SetLoaderStatic.setName = item.customSetName;
                    SetLoaderStatic.middleColor = item.Background;

                    SceneManager.LoadScene("CustomModeParamChoice"); 
                });
            }
            else
                tempInst.GetComponent<Button>().onClick.AddListener(delegate { SceneManager.LoadScene(item.SceneName); });


            tempInst.transform.GetChild(0).gameObject.SetActive(false);
            //if (item.InstructionURL is not null && item.InstructionURL.Length == 0)
            //    tempInst.transform.GetChild(0).gameObject.SetActive(false);
            //else
            //    tempInst.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { Application.OpenURL(item.InstructionURL); });

            tempInst.transform.GetChild(2).GetComponent<Text>().text = item.Name;
            if (item.renderTexture == null)
            {
                tempInst.transform.GetChild(3).GetComponent<Image>().sprite = item.Image;
            }
            else
            {
                var tempMat = new Material(tempInst.transform.GetChild(3).GetComponent<Image>().material);
                tempMat.mainTexture = item.renderTexture;
                tempInst.transform.GetChild(3).GetComponent<Image>().material = tempMat;
            }
        }
    }

    [System.Serializable]
    public class MainMenuItem
    {
        [SerializeField] private string name;
        [SerializeField] private Sprite image;
        [SerializeField] public RenderTexture renderTexture;
        [SerializeField] private string sceneName;
        [SerializeField] private Color backgroundColor;
        [SerializeField] private string instructionLink;
        public string customSetName;

        public string Name => name;
        public Sprite Image => image;
        public string SceneName => sceneName;
        public Color Background => backgroundColor;
        public string InstructionURL => instructionLink;

        public MainMenuItem(string name, Sprite image, Color background, string sceneName)
        {
            this.name = name;
            this.image = image;
            backgroundColor = background;
            this.sceneName = sceneName;
            instructionLink = null;
        }

        public MainMenuItem(string name, RenderTexture render, Color background, string sceneName)
        {
            this.name = name;
            this.renderTexture = render;
            backgroundColor = background;
            this.sceneName = sceneName;
            instructionLink = null;
        }

        public MainMenuItem(string name, RenderTexture render, Color background)
        {
            this.name = name;
            this.renderTexture = render;
            backgroundColor = background;
            this.customSetName = name;
            instructionLink = null;
        }
    }
}
