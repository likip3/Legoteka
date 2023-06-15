using System;
using System.Collections.Generic;
using UnityEngine;


public enum BrickTag
{
    Square = 0,
    Сircle = 1,
    Sphere = 2,
    Long = 3,
    Flat = 4,
}

public static class BrickDatabase
{
    const int renderResolution = 200;

    static BrickDatabase()
    {
        SQLiteTasker.LoadFromSQLite();
    }

    private static List<ListItem> bricksCategories = new List<ListItem>();

    private static Material defaultMaterial = Resources.Load<Material>("DefaultMaterial");

    public static List<ListItem> BricksCategories => bricksCategories;
    public static Material DefaultMaterial
    {
        get
        {
            return defaultMaterial;
        }
        set
        {
            defaultMaterial = value;
        }
    }


    public static bool IsCategoryNameTaken(string name)
    {
        foreach (var item in bricksCategories)
        {
            if (item.Name == name)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsBrickIDTaken(string id)
    {
        foreach (var item in bricksCategories)
        {
            foreach (var brick in item.Bricks)
            {
                if (brick.ID == id)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static ListItem AddCategory(string name)
    {
        var tempCat = new ListItem(name);
        bricksCategories.Add(tempCat);
        return tempCat;
    }

    public static ListItem AddCategory(string name, GameObject gameObject, List<BrickTag> brickTags)
    {
        var tempCat = new ListItem(name, gameObject, brickTags);
        bricksCategories.Add(tempCat);
        return tempCat;
    }

    public static void DeleteCategoryAt(int index)
    {
        bricksCategories.RemoveAt(index);

    }
    public static RenderTexture CreatePreviewRender(GameObject gameObject, Color color) => CreatePreviewRender(gameObject, color, Vector3.zero, Quaternion.identity);

    public static RenderTexture CreatePreviewRender(GameObject gameObject, Color color, Vector3 pos, Quaternion rotarion)
    {
        var renderTexture = new RenderTexture(renderResolution, renderResolution, 16);
        var material = new Material(defaultMaterial);
        material.color = color;
        var renderTask = new RenderTask(renderTexture, gameObject, material, pos, rotarion);
        ObjectPreviewRenderer.current.RenderPreview(renderTask);
        return renderTexture;
    }

    public static RenderTexture NewCreatePreviewRender(GameObject gameObject, Color color)
    {
        var renderTexture = new RenderTexture(580, 550, 16);
        var material = new Material(defaultMaterial);
        material.color = color;
        var renderTask = new RenderTask(renderTexture, gameObject, new Vector3(), Quaternion.Euler(0, 45, 0));
        ObjectPreviewRenderer.current.RenderPreview(renderTask);
        return renderTexture;
    }



}


public class ListItem
{
    private System.Random rnd = new System.Random();

    public ListItem(string name)
    {
        brickName = name;
        foreach (var color in defaultColorsList)
        {
            bricksLEGO.Add(new BrickDBItem(rnd.Next(100, 99999).ToString(), color, gameObject));
        }

    }
    public ListItem(string name, GameObject gameObject, List<BrickTag> brickTags)
    {
        brickName = name;
        this.gameObject = gameObject;
        this.brickTags = brickTags;
    }

    [SerializeField] private string brickName;
    [SerializeField] private Sprite brickPreview;
    [SerializeField] private GameObject gameObject;
    [SerializeField] private List<BrickTag> brickTags = new List<BrickTag>();
    [SerializeField] private List<BrickDBItem> bricksLEGO = new List<BrickDBItem>();
    [SerializeField] private RenderTexture renderTexture;

    private Color[] defaultColorsList = new Color[] {
    new Color(1,0,0),
    new Color(0,1,0),
    new Color(0,0,1),
    new Color(1,1,0),
    new Color(1,1,1),
    new Color(0,1,1),
    };

    public void AddItem()
    {
        var id = rnd.Next(100, 99999);
        while (BrickDatabase.IsBrickIDTaken(id.ToString()))
            id += 1;

        AddItem(id.ToString());
    }

    public void AddItem(string id)
    {
        AddItem(id, new Color(1, 1, 1));
    }

    public void AddItem(string id, Color color)
    {
        bricksLEGO.Add(new BrickDBItem(id, color, gameObject));
    }

    public void AddItem(Color color)
    {
        var id = rnd.Next(100, 99999);
        while (BrickDatabase.IsBrickIDTaken(id.ToString()))
            id += 1;
        bricksLEGO.Add(new BrickDBItem(id.ToString(), color, gameObject));
    }

    public void DeleteItem(BrickDBItem item)
    {
        bricksLEGO.Remove(item);
    }

    public string Name
    {
        get
        {
            return brickName;
        }
        set
        {
            brickName = value;
        }
    }

    public List<BrickTag> Tags => brickTags;
    public GameObject GM
    {
        get
        {
            return gameObject;
        }
        set
        {
            gameObject = value;
        }
    }
    public void SetTag(string tag)
    {
        //пока так
        //нужно будет с MultiSelectDropdown разобраться
        brickTags.Clear();
        brickTags.Add((BrickTag)Enum.Parse(typeof(BrickTag), tag));
    }

    [Obsolete("Сейчас не работает. Используйте RenderTexture")]
    public Sprite Sprite
    {
        get
        {
            if (brickPreview is not null) return brickPreview;

            brickPreview = CreatePreviewSprite(gameObject, BrickDatabase.DefaultMaterial);

            return brickPreview;
        }
    }

    private Sprite CreatePreviewSprite(GameObject gameObject, Material material)
    {
        var renderTexture = new RenderTexture(64, 64, 16);


        gameObject.GetComponent<MeshRenderer>().material = material;
        var renderTaks = new RenderTask(renderTexture,gameObject);
        ObjectPreviewRenderer.current.RenderPreview(renderTaks);

        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

    }

    public RenderTexture RenderTexture
    {
        get
        {
            if (renderTexture is not null) return renderTexture;

            renderTexture = BrickDatabase.CreatePreviewRender(gameObject, new Color(1,1,1));

            return renderTexture;
        }
    }

    public List<BrickDBItem> Bricks => bricksLEGO;
}





public class BrickDBItem
{
    [SerializeField] private string brickID;
    [Obsolete("Не робит")][SerializeField] private Sprite brickPreview;
    [SerializeField] private Color color;
    [SerializeField] private RenderTexture renderTexture;
    private GameObject gameObject;

    public BrickDBItem(string id, Color color, GameObject gameObject)
    {
        brickID = id;
        this.color = color;
        if (gameObject is null) return;
        this.gameObject = gameObject;
        var brick = this.gameObject.GetComponent<Brick>();
        brick.color = color;
        brick.ID = id;
    }



    public string ID
    {
        get
        {
            return brickID;
        }
        set
        {
            brickID = value;
        }
    }

    public Material Material
    {
        get
        {
            var material = new Material(BrickDatabase.DefaultMaterial);
            material.color = color;
            return material;
        }
    }

    [Obsolete("Сейчас не работает. Используйте RenderTexture")]
    public Sprite Sprite
    {
        get
        {
            return brickPreview;
        }
    }

    public RenderTexture RenderTexture
    {
        get
        {
            if (renderTexture is not null) return renderTexture;

            renderTexture = BrickDatabase.CreatePreviewRender(gameObject, color);

            return renderTexture;
        }
    }

    public Color Color { get => color; set => color = value; }
    public GameObject GameObject { get => gameObject; set => gameObject = value; }
}
