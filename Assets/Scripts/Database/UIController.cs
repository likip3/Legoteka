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
    private BricksGridNoInstruction rootGrid;

    [SerializeField]
    private List<ListItem> bricks;


    private Label selectedItemName;
    private Label selectedItemTags;
    private VisualElement detalPreview;
    private VisualElement itemsContainer;
    private Button collapseButton;
    private Button closeButton;
    private VisualElement dragMenu;
    private List<VisualElement> extendersToCollapse = new List<VisualElement>();
    private bool isDraging;

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
        collapseButton.clicked += CollapseAll;
        closeButton.clicked += CloseMenu;

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
        this.isDraging = drag;
    }


    private void CollapseAll()
    {
        foreach (var e in extendersToCollapse)
            Collapse(e);
        extendersToCollapse.Clear();
    }

    private void CloseMenu()
    {
        openButton.SetActive(true);
        gameObject.SetActive(false);
    }

    private void ButtonVisualise(string name, List<BrickTag> tags, RenderTexture renderTexture)
    {
        selectedItemName.text = name;
        selectedItemTags.text = tags.ToString();

        detalPreview.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(renderTexture));
    }

    private void ObjectBrickSpawn(GameObject GM, Material material)
    {
        rootGrid.StartPlacingBrick(GM.GetComponent<Brick>(), material);
    }

    private void ButtonSwitch(ListItem item, VisualElement extender)
    {
        ButtonVisualise(item.Name, item.Tags, item.RenderTexture);
        if (extender.style.width.value.value == 100)
        {
            extendersToCollapse.Remove(extender);
            Collapse(extender);
            return;
        }

        Extand(item, extender);
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
                ObjectBrickSpawn(item.GM, det.Material);
                ButtonVisualise(det.ID.ToString(), item.Tags, det.RenderTexture);
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

}