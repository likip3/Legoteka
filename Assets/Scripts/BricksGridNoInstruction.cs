using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BricksGridNoInstruction : MonoBehaviour
{
    public Vector2Int GridSize = new Vector2Int(10, 10);
    private Brick[,] grid;
    private Brick flyingBrick;
    private Camera mainCamera;
    private float y = 0;
    private Touch touch;
    private int step;
    private List<int> cList;
    private Stack<Brick> posStack;
    public AudioSource source;
    public AudioClip audioClip;

    private void Awake()
    {
        grid = new Brick[GridSize.x, GridSize.y];
        mainCamera = Camera.main;
        cList = new List<int>();
        posStack = new Stack<Brick>();
    }

    public void StartPlacingBrick(Brick buildingPrefab)
    {
        if(flyingBrick == null)
            flyingBrick = Instantiate(buildingPrefab);
        else if(flyingBrick != null && flyingBrick == buildingPrefab)
        {
            Destroy(flyingBrick.gameObject);
            flyingBrick = Instantiate(buildingPrefab);
        }
    }

    public void StartPlacingBrick(Brick buildingPrefab, Material material)
    {
        if (flyingBrick == null)
        {
            flyingBrick = Instantiate(buildingPrefab);
            flyingBrick.GetComponent<MeshRenderer>().material = material;
        }
        else if (flyingBrick != null && flyingBrick == buildingPrefab)
        {
            Destroy(flyingBrick.gameObject);
            flyingBrick = Instantiate(buildingPrefab);
            flyingBrick.GetComponent<MeshRenderer>().material = material;
            }
    }

    private void Update()
    {
        if (Input.touchCount > 0 && !CameraControll.movingState)
        {
            touch = Input.GetTouch(0);
            
            if (flyingBrick != null)
            {
                var groundPlane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = mainCamera.ScreenPointToRay(touch.position);
                Vector3 offset = mainCamera.transform.up * 0.1f;
                ray.origin += offset;
                if (groundPlane.Raycast(ray, out float position))
                {

                    Vector3 worldPosition = ray.GetPoint(position);
                    int x = Mathf.RoundToInt(worldPosition.x);
                    int z = Mathf.RoundToInt(worldPosition.z);

                    if (touch.phase == TouchPhase.Moved)
                    {
                        flyingBrick.transform.position = new Vector3(x, y, z);
                    }

                    cList.Add(x);
                    cList.Add(z);
                }

            }
        }

        CheckBrick();
    }

    public void PlaceFlyingBrick()
    {
        var placeX = cList[0];
        var placeY = cList[1];
        for (int x = 0; x < flyingBrick.Size.x; x++)
        {
            for (int y = 0; y < flyingBrick.Size.y; y++)
            {
                grid[placeX + x, placeY + y] = flyingBrick;
            }
        }
        source.PlayOneShot(audioClip);
        posStack.Push(flyingBrick);
        y = 0;
        flyingBrick = null;
    }

    public void CheckBrick()
    {
        if (flyingBrick == null && touch.phase == TouchPhase.Moved)
            Events.InvokeIfNull();     
        else
            Events.InvokeIfNotNull();
    }

    public void IncreazeZ()
    {

        if (flyingBrick != null)
        {
            y += 1.2f;
            flyingBrick.transform.position += new Vector3(0, 1.2f, 0);
        }

    }

    public void IncreazeZSmall()
    {

        if (flyingBrick != null)
        {
            y += 0.4f;
            flyingBrick.transform.position += new Vector3(0, 0.4f, 0);
        }

    }

    public void DecreaseZ()
    {
        if (flyingBrick != null && y != 0)
        {
            if (y - 1.2f < 0)
            {
                y = 0;
                flyingBrick.transform.position = new Vector3(flyingBrick.transform.position.x, 0, flyingBrick.transform.position.z);
            }
            else
            {
                y -= 1.2f;
                flyingBrick.transform.position -= new Vector3(0, 1.2f, 0);
            }
        }

    }

    public void DecreaseZSmall()
    {
        if (flyingBrick != null && y != 0)
        {
            y -= 0.4f;
            flyingBrick.transform.position -= new Vector3(0, 0.4f, 0);
        }

    }

    public void RotateBrick()
    {
        if(flyingBrick != null)
            flyingBrick.transform.Rotate(new Vector3(0, 0, 90f));
    }

    public void ReverseStep()
    {
        if(posStack.Count != 0 && flyingBrick == null)
            Destroy(posStack.Pop().gameObject);
    }
}
