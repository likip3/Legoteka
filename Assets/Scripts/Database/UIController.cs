using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class UIController : MonoBehaviour
{
    [SerializeField]
    private VisualTreeAsset templateButtonUXML;

    [SerializeField]
    private VisualTreeAsset templateButtonExpandedUXML;

    [SerializeField]
    private GameObject openButton;

    [SerializeField]
    private GameObject backButton;

    [SerializeField]
    private FreeModeBrickPlacer rootGrid;

    [SerializeField]
    private List<ListItem> bricks;


    private Label selectedItemName;
    private Label selectedItemTags;
    private VisualElement detalPreview;
    private VisualElement itemsContainer;
    private Button collapseButton;
    private Button closeButton;
    private VisualElement dragMenu;
    private ScrollView scrollViewMain;
    private List<VisualElement> extendersToCollapse = new List<VisualElement>();
    private bool isDraging;
    private bool isLoadedSet;

    private void Awake()
    {
        isLoadedSet = SetLoaderStatic.enabled;
    }

    void OnEnable()
    {
        bricks = BrickDatabase.BricksCategories;

        var root = GetComponent<UIDocument>().rootVisualElement;

        selectedItemName = root.Q<Label>("item-name-label");
        selectedItemTags = root.Q<Label>("item-name-tags");
        itemsContainer = root.Q<VisualElement>("scroll-content");
        detalPreview = root.Q<VisualElement>("detal-preview");
        collapseButton = root.Q<Button>("collapse-button");
        closeButton = root.Q<Button>("close-button");
        dragMenu = root.Q<VisualElement>("drag-menu");
        scrollViewMain = root.Q<ScrollView>("scroll-view-main");
        collapseButton.clicked += CollapseAll;
        closeButton.clicked += CloseMenu;
        if(isLoadedSet)
            JustCreateButtons(ToBrickDBItemList(SetLoaderStatic.GetBrickList(SaveLoadSystem.DeXml(SetLoaderStatic.setName, "/FreeModeSave/"))));
        else
            CreateExtandButtons(bricks);
    }


    private void Update()
    {
        if (isDraging)
        {
            if (Input.touchCount == 0)
            {
                isDraging = false;
                return;
            }
            var newPos = Math.Max(Math.Min(Input.GetTouch(0).position.x, Screen.width - Screen.width / 70), 600);
            dragMenu.style.width = new Length(newPos, LengthUnit.Pixel);

        }
    }


    public void SetDrag(bool drag)
    {
        isDraging = drag;
    }


    private void CollapseAll()
    {
        foreach (var e in extendersToCollapse)
            Collapse(e);
        extendersToCollapse.Clear();
    }

    private void CloseMenu()
    {
        backButton.SetActive(true);
        openButton.SetActive(true);
        gameObject.SetActive(false);
    }

    private void ButtonVisualise(string name, List<BrickTag> tags, RenderTexture renderTexture)
    {
        selectedItemName.text = name;
        selectedItemTags.text = tags.ToString();

        detalPreview.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(renderTexture));
    }

    private void ObjectBrickSpawn(GameObject GM, BrickDBItem brickDBItem)
    {
        Brick buildingPrefab = GM.GetComponent<Brick>();
        buildingPrefab.ID = brickDBItem.ID;
        buildingPrefab.color = brickDBItem.Color;
        rootGrid.StartPlacingBrick(buildingPrefab, brickDBItem.Material);
    }

    private void ButtonSwitch(ListItem item, VisualElement extender)
    {
        var tempScroll = scrollViewMain.scrollOffset;
        StartCoroutine(ScrollerAwaiter(tempScroll));

        ButtonVisualise(item.Name, item.Tags, item.RenderTexture);
        if (extender.style.width.value.value == 100)
        {
            extendersToCollapse.Remove(extender);
            Collapse(extender);
            return;
        }

        Extand(item, extender);
    }

    private System.Collections.IEnumerator ScrollerAwaiter(Vector2 scroll)
    {
        yield return new WaitForEndOfFrame();
        scrollViewMain.scrollOffset = scroll;
    }

    private void Extand(ListItem item, VisualElement extender)
    {
        extender.style.width = new Length(100, LengthUnit.Percent);
        extender.style.height = new StyleLength(StyleKeyword.Auto);
        extendersToCollapse.Add(extender);
        foreach (var det in item.Bricks)
        {
            var templateButton = templateButtonUXML.Instantiate().Q<Button>();
            templateButton.clicked += delegate
            {
                //ObjectSpawn(item.GM, det.Material);
                ObjectBrickSpawn(item.GM, det);
                //ButtonVisualise(det.ID.ToString(), item.Tags, det.RenderTexture);
            };
            templateButton.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(det.RenderTexture));
            extender.Add(templateButton);
        }
    }

    private static void Collapse(VisualElement extender)
    {
        extender.style.width = new Length(200, LengthUnit.Pixel);
        extender.style.height = new Length(200, LengthUnit.Pixel);
        var childCount = extender.childCount;
        for (var i = 1; i < childCount; i++)
        {
            extender.RemoveAt(1);
        }
    }

    private void CreateExtandButtons(List<ListItem> items)
    {
        foreach (var item in items)
        {
            var extBack = templateButtonExpandedUXML.Instantiate()[0];
            var templateButton = templateButtonUXML.Instantiate().Q<Button>();
            templateButton.clicked += delegate { ButtonSwitch(item, extBack); };
            templateButton.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(item.RenderTexture));
            extBack.Add(templateButton);

            itemsContainer.Add(extBack);
        }
    }

    private void JustCreateButtons(List<BrickDBItem> items)
    {
        foreach (var det in items)
        {
            var templateButton = templateButtonUXML.Instantiate().Q<Button>();
            templateButton.clicked += delegate
            {
                //ObjectSpawn(item.GM, det.Material);
                ObjectBrickSpawn(det.GameObject, det);
            };
            templateButton.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(det.RenderTexture));
            itemsContainer.Add(templateButton);
        }
    }

    public static List<BrickDBItem> ToBrickDBItemList(List<BrickXML> col)
    {
        List<BrickDBItem> listF = new();
        foreach (var item in col)
        {
            listF.Add(new BrickDBItem(item.brickID,SQLiteTasker.GetColorById(item.brickID), SQLiteTasker.BrickDict[item.brickID].gameObject));
        }
        return listF;
    }

}