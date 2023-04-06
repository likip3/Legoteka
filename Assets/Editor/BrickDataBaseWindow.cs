using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class BrickDataBaseWindow : EditorWindow
{
    [MenuItem("Legotecka/Brick DataBase Filler")]
    public static void ShowEditor()
    {
        BrickDataBaseWindow wnd = GetWindow<BrickDataBaseWindow>();
        wnd.titleContent = new GUIContent("Brick DataBase Filler");
        wnd.minSize = new Vector2(340, 400);
        wnd.maxSize = new Vector2(800, 900);
    }

    private ListView categoryList;
    private ListView brickList;

    private ListItem selCategory;
    private BrickDBItem selBrick;

    private VisualElement categoryInfo;
    private VisualElement brickInfo;

    private TextField c_nameField;
    private DropdownField c_tagsDropdown;
    private ObjectField c_objectField;

    private TextField b_IdField;
    private ColorField b_ColorField;

    public void CreateGUI()
    {
        //Формирование формы
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UITemplates/BrickDataBaseWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        labelFromUXML.style.height = new Length(100, LengthUnit.Percent);
        rootVisualElement.Add(labelFromUXML);

        //регистрация верхних кнопочек
        rootVisualElement.Q<Button>("save-database").RegisterCallback<MouseUpEvent>(SaveToDatabase_Clicked, TrickleDown.TrickleDown);
        rootVisualElement.Q<Button>("add-category").RegisterCallback<MouseUpEvent>(AddCategory_Clicked, TrickleDown.TrickleDown);
        rootVisualElement.Q<Button>("delete-category").RegisterCallback<MouseUpEvent>(DeleteCategory_Clicked, TrickleDown.TrickleDown);

        //регистрация списка категорий
        categoryList = rootVisualElement.Q<ListView>("category-list");
        categoryList.onSelectionChange += objects => OnCategorySelected(objects);

        //регистрация списка бриков
        brickList = rootVisualElement.Q<ListView>("bricks-list");
        brickList.onSelectionChange += objects => OnBrickSelected(objects);

        //контейнеры заполнения
        categoryInfo = rootVisualElement.Q<VisualElement>("category-info");
        brickInfo = rootVisualElement.Q<VisualElement>("brick-info");

        //поля для параметров категорий
        c_nameField = rootVisualElement.Q<TextField>("category-name");
        c_nameField.RegisterValueChangedCallback(v => NameChanged(v));

        c_objectField = new ObjectField();
        c_objectField.objectType = typeof(GameObject);
        c_objectField.label = "Game Object";
        c_objectField.RegisterValueChangedCallback(v => ObjectChanged(v));
        categoryInfo.Add(c_objectField);

        c_tagsDropdown = rootVisualElement.Q<DropdownField>("tags-dropdown");
        c_tagsDropdown.choices = new List<string>(Enum.GetNames(typeof(BrickTag)));
        c_tagsDropdown.RegisterValueChangedCallback(v => TagsDropdown_Clicked(v));

        rootVisualElement.Q<Button>("add-brick").RegisterCallback<MouseUpEvent>(AddBrick_Clicked, TrickleDown.TrickleDown);

        //поля параметров для бриков
        b_IdField = rootVisualElement.Q<TextField>("brick-id");
        b_IdField.RegisterValueChangedCallback(v => BrickIdChanged(v));

        b_ColorField = new ColorField();
        b_ColorField.label = "Color";
        b_ColorField.RegisterValueChangedCallback(v => BrickColorChanged(v));
        brickInfo.Add(b_ColorField);

        rootVisualElement.Q<Button>("delete-brick").RegisterCallback<MouseUpEvent>(DeleteBrick_Clicked, TrickleDown.TrickleDown);


        UpdateCategoryList();
    }

    private void AddBrick_Clicked(MouseUpEvent evt)
    {
        selCategory.AddItem();
        UpdateBrickList();
    }

    private void DeleteBrick_Clicked(MouseUpEvent evt)
    {
        if (brickList.selectedIndex < 0) return;
        selCategory.DeleteItem(selBrick);
        UpdateBrickList();
        UpdateBrickInfo();
    }

    private void BrickColorChanged(ChangeEvent<Color> v)
    {
        selBrick.Color = v.newValue;
    }

    private void BrickIdChanged(ChangeEvent<string> v)
    {
        selBrick.ID = v.newValue;
        UpdateBrickList();
    }

    private void ObjectChanged(ChangeEvent<UnityEngine.Object> v)
    {
        selCategory.GM = (GameObject)v.newValue;
    }

    private void NameChanged(ChangeEvent<string> v)
    {
        selCategory.Name = v.newValue;
        UpdateCategoryList();
        //categoryList.contentContainer.ElementAt(categoryList.selectedIndex).name = v.newValue; не работает(
    }

    private void TagsDropdown_Clicked(ChangeEvent<string> v)
    {
        selCategory.SetTag(v.newValue);
    }

    private void OnBrickSelected(IEnumerable<object> objects)
    {
        UpdateBrickInfo();
    }

    private void UpdateBrickInfo()
    {
        categoryInfo.style.display = DisplayStyle.None;
        brickInfo.style.display = DisplayStyle.Flex;
        selBrick = (BrickDBItem)brickList.selectedItem;

        if (selBrick is null)
        {
            brickInfo.style.display = DisplayStyle.None;
            return;
        }

        b_IdField.SetValueWithoutNotify(selBrick.ID);
        b_ColorField.SetValueWithoutNotify(selBrick.Color);
    }

    private void OnCategorySelected(IEnumerable<object> objects)
    {
        selCategory = (ListItem)categoryList.selectedItem;
        if (selCategory is null) return;
        UpdateCategoryInfo();
        UpdateBrickList(selCategory);
    }

    private void UpdateCategoryInfo()
    {
        selCategory = (ListItem)categoryList.selectedItem;

        categoryInfo.style.display = DisplayStyle.Flex;
        brickInfo.style.display = DisplayStyle.None;

        c_nameField.SetValueWithoutNotify(selCategory.Name);
        c_objectField.SetValueWithoutNotify(selCategory.GM);
        if (selCategory.Tags.Count > 0)
            c_tagsDropdown.SetValueWithoutNotify(selCategory.Tags.First().ToString());
        else
            c_tagsDropdown.SetValueWithoutNotify(null);
    }

    private void SaveToDatabase_Clicked(MouseUpEvent evt)
    {
        SQLiteTasker.UploadToSQLite();
    }


    private void DeleteCategory_Clicked(MouseUpEvent evt)
    {
        if (categoryList.selectedIndex < 0) return;
        BrickDatabase.DeleteCategoryAt(categoryList.selectedIndex);
        UpdateCategoryList();
        brickList.ClearSelection();

        categoryInfo.style.display = DisplayStyle.None;
        brickInfo.style.display = DisplayStyle.None;

        if (categoryList.selectedItem is null)
        {
            brickList.itemsSource = new List<Brick>();
            return;
        }

        UpdateCategoryInfo();
        UpdateBrickList();
    }

    private void AddCategory_Clicked(MouseUpEvent evt)
    {
        var name = "New Category";
        while (BrickDatabase.IsCategoryNameTaken(name))
            name += 1;
        BrickDatabase.AddCategory(name);
        UpdateCategoryList();
    }

    private void UpdateCategoryList()
    {
        categoryList.makeItem = () => new Label();
        categoryList.bindItem = (item, index) => { (item as Label).text = BrickDatabase.BricksCategories[index].Name; };
        categoryList.itemsSource = BrickDatabase.BricksCategories;
    }
    private void UpdateBrickList()
    {
        UpdateBrickList(selCategory);
    }
    private void UpdateBrickList(ListItem category)
    {
        brickList.makeItem = () => new Label();
        brickList.bindItem = (item, index) => {
            if (index > category.Bricks.Count - 1) return;
            (item as Label).text = category.Bricks[index].ID; };
        brickList.itemsSource = category.Bricks;
    }
}