using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeModeBrickPlacer : MonoBehaviour
{
    private Camera mainCamera;

    private Transform controllableBrick;
    private Stack<GameObject> brickStack = new();
    private bool isDeleteRay;
    private GameObject highlightedBrick;
    private bool unPress;

    private Box boxToRender;
    private Color colorBox;
    private bool isBoxRender;

    [SerializeField]
    private AudioSource soundSource;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (isBoxRender)
            DrawBox(boxToRender, colorBox, 0);

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && isDeleteRay && highlightedBrick != null)
            unPress = true;
    }

    public void OnSavePresed() => SaveBrickState("Тестовыя абоба");
    public void OnLoadPresed() => LoadBrickState("Тестовыя абоба");


    public static void SaveBrickState(string name)
    {
        BrickCollectionXML brickColl = SceneToBrickCol(name);
        SaveLoadSystem.SaveXml(brickColl, "/FreeModeSave/");
    }

    public static BrickCollectionXML SceneToBrickCol(string name)
    {
        var brickColl = new BrickCollectionXML(name);
        foreach (var brick in GameObject.FindGameObjectsWithTag("BrickOnScene"))
        {
            var brickData = brick.GetComponent<Brick>();
            brickColl.BrickArray.Add(new BrickXML(brickData.ID, brick.transform.position, brick.transform.rotation));
        }

        return brickColl;
    }

    public static void LoadBrickState(string name)
    {
        var brickColl = SaveLoadSystem.DeXml(name, "/FreeModeSave/");
        ToSceneFromBrickCol(brickColl);
    }

    public static void ToSceneFromBrickCol(BrickCollectionXML brickColl)
    {
        foreach (var brick in brickColl.BrickArray)
        {
            var brickGM = Instantiate(SQLiteTasker.BrickDict[brick.brickID]);
            brickGM.transform.SetPositionAndRotation(brick.position, brick.rotation);
            foreach (var col in brickGM.GetComponents<BoxCollider>())
            {
                col.enabled = true;
                col.size += new Vector3(0.001f, 0.001f);
            }

            Color color = SQLiteTasker.GetColorById(brick.brickID);
            brickGM.color = color;
            brickGM.ID = brick.brickID;
            brickGM.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    private void FixedUpdate()
    {
        PlaceCordsForBrick();
        DeleteRayCast();
    }

    private void DeleteRayCast()
    {
        if (!isDeleteRay) return;

        if (highlightedBrick != null && unPress)
        {
            Destroy(highlightedBrick);
            highlightedBrick = null;
            unPress = false;
        }

        if (Input.touchCount <= 0 || CameraControll.movingState)
            return;



        var touch = Input.GetTouch(0);
        var ray = mainCamera.ScreenPointToRay(touch.position);

        if (!Physics.Raycast(ray, out var hit) || hit.collider.gameObject.layer == 7)
        {
            highlightedBrick = null;
            return;
        }

        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);

        if (hit.collider.gameObject == highlightedBrick) return;

        highlightedBrick = hit.collider.gameObject;

        //if (hit.collider.TryGetComponent<Outlinable>(out var outlinable))
        //    outlinable.enabled = true;
        //else
        //{
        //    outlinable = hit.collider.gameObject.AddComponent<Outlinable>();
        //    outlinable.OutlineParameters.FillPass.Shader = Resources.Load<Shader>("Easy performant outline/Shaders/Fills/Interlaced");
        //}


    }

    private void PlaceCordsForBrick()
    {
        if (controllableBrick == null)
            return;

        if (Input.touchCount <= 0 || CameraControll.movingState)
            return;

        isDeleteRay = false;

        var touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Moved)
            return;

        #region DebugRay
        var rayt = mainCamera.ScreenPointToRay(touch.position);
        var groundPlaneTest = new Plane(Vector3.up, Vector3.zero);
        Physics.Raycast(rayt, out var hitt);
        Debug.DrawRay(rayt.origin, rayt.direction * hitt.distance, Color.yellow);
        #endregion

        var ray = mainCamera.ScreenPointToRay(touch.position);

        if (!Physics.Raycast(ray, out var hit))
            return;

        var x = Mathf.RoundToInt(hit.point.x);
        var z = Mathf.RoundToInt(hit.point.z);
        var y = 0f;

        if (hit.collider.gameObject.layer != 7)
        {
            var oldLayer = hit.collider.gameObject.layer;
            hit.collider.gameObject.layer = 9;

            Debug.DrawRay(hit.point + new Vector3(0, 0.01f), Vector3.down * hit.transform.InverseTransformPoint(hit.point).z + new Vector3(0, 0.01f), Color.magenta, .1f);

            if (Physics.Raycast(hit.point + new Vector3(0, 0.01f), Vector3.down, out var downPoint, hit.transform.InverseTransformPoint(hit.point).z + 0.01f)
                && downPoint.collider.gameObject.layer != 7) //ось z т.к. модели из блендера, а "вверх" в блендере это ось z. И мы используем локальные координаты.
            {
                y = downPoint.point.y - 0.2f;
            }
            else
            {
                y = hit.collider.transform.position.y;
            }
            hit.collider.gameObject.layer = oldLayer;
        }

        foreach (var boxCollider in controllableBrick.GetComponents<BoxCollider>())
        {
            var globalPosShift = controllableBrick.position - controllableBrick.TransformPoint(boxCollider.center);
            StartRenderBox(new Vector3(x, y, z) - globalPosShift,
                    boxCollider.size / 2 - new Vector3(0.01f, 0.01f, 0.201f),
                    controllableBrick.rotation,
                    Color.red);

            if (Physics.CheckBox(new Vector3(x, y, z) - globalPosShift,
                boxCollider.size / 2 - new Vector3(0.01f, 0.01f, 0.201f),
                controllableBrick.rotation)) 
                return;
        }


        controllableBrick.transform.position = new Vector3(x, y, z);
    }

    public void StartPlacingBrick(Brick buildingPrefab)
    {
        if (controllableBrick == null)
            controllableBrick = Instantiate(buildingPrefab).transform;
        else
        {
            Destroy(controllableBrick.gameObject);
            controllableBrick = Instantiate(buildingPrefab).transform;
        }
        controllableBrick.gameObject.layer = 9;
        CameraControll.movingState = false;
    }

    public void StartPlacingBrick(Brick buildingPrefab, Material material)
    {
        if (controllableBrick == null)
            controllableBrick = Instantiate(buildingPrefab).transform;
        else
        {
            Destroy(controllableBrick.gameObject);
            controllableBrick = Instantiate(buildingPrefab).transform;
        }
        controllableBrick.GetComponent<MeshRenderer>().material = material;
        controllableBrick.gameObject.layer = 9;
        CameraControll.movingState = false;
    }


    public void PlaceControllableBrick()
    {
        brickStack.Push(controllableBrick.gameObject);
        foreach (var col in controllableBrick.GetComponents<BoxCollider>())
        {
            col.enabled = true;
            col.size += new Vector3(0.001f, 0.001f);
        }
        controllableBrick.gameObject.layer = 0;
        soundSource.pitch = Random.Range(.8f,2.5f);
        soundSource.transform.position = controllableBrick.position;
        soundSource.Play();
        controllableBrick = null;
    }

    public void RotateBrick()
    {
        if (controllableBrick != null)
            controllableBrick.transform.Rotate(new Vector3(0, 0, 90f));
    }

    public void Undo()
    {
        if (brickStack.Count != 0)
        { 
            var gm = brickStack.Pop().gameObject;
            if (gm == null)
            {
                Undo();
                return;
            }
            Destroy(gm);
        }
    }

    public void UnSelectBrick()
    {
        CameraControll.movingState = true;
        if (controllableBrick == null) return;
        Destroy(controllableBrick.gameObject);
        controllableBrick = null;
    }

    public void DeleteRayOn()
    {
        UnSelectBrick();
        CameraControll.movingState = false;
        isDeleteRay = true;
    }

    public void DeleteRayToggle()
    {
        if (isDeleteRay)
            DeleteRayOff();
        else DeleteRayOn();
    }

    public void DeleteRayOff()
    {
        CameraControll.movingState = true;
        isDeleteRay = false;
    }

    #region DebugDrawBox
    public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
    {
        DrawBox(new Box(origin, halfExtents, orientation), color, 1.3f);
    }
    public void StartRenderBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
    {
        boxToRender = new Box(origin, halfExtents, orientation);
        colorBox = color;
        isBoxRender = true;

    }

    public void StopRenderBox()
    {
        isBoxRender = false;
    }

    public static void DrawBox(Box box, Color color, float duration)
    {
        Debug.DrawLine(box.frontTopLeft, box.frontTopRight, color);
        Debug.DrawLine(box.frontTopRight, box.frontBottomRight, color);
        Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
        Debug.DrawLine(box.frontBottomLeft, box.frontTopLeft, color);

        Debug.DrawLine(box.backTopLeft, box.backTopRight, color, duration);
        Debug.DrawLine(box.backTopRight, box.backBottomRight, color, duration);
        Debug.DrawLine(box.backBottomRight, box.backBottomLeft, color, duration);
        Debug.DrawLine(box.backBottomLeft, box.backTopLeft, color, duration);

        Debug.DrawLine(box.frontTopLeft, box.backTopLeft, color, duration);
        Debug.DrawLine(box.frontTopRight, box.backTopRight, color, duration);
        Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color, duration);
        Debug.DrawLine(box.frontBottomLeft, box.backBottomLeft, color, duration);
    }

    public struct Box
    {
        public Vector3 localFrontTopLeft { get; private set; }
        public Vector3 localFrontTopRight { get; private set; }
        public Vector3 localFrontBottomLeft { get; private set; }
        public Vector3 localFrontBottomRight { get; private set; }
        public Vector3 localBackTopLeft { get { return -localFrontBottomRight; } }
        public Vector3 localBackTopRight { get { return -localFrontBottomLeft; } }
        public Vector3 localBackBottomLeft { get { return -localFrontTopRight; } }
        public Vector3 localBackBottomRight { get { return -localFrontTopLeft; } }

        public Vector3 frontTopLeft { get { return localFrontTopLeft + origin; } }
        public Vector3 frontTopRight { get { return localFrontTopRight + origin; } }
        public Vector3 frontBottomLeft { get { return localFrontBottomLeft + origin; } }
        public Vector3 frontBottomRight { get { return localFrontBottomRight + origin; } }
        public Vector3 backTopLeft { get { return localBackTopLeft + origin; } }
        public Vector3 backTopRight { get { return localBackTopRight + origin; } }
        public Vector3 backBottomLeft { get { return localBackBottomLeft + origin; } }
        public Vector3 backBottomRight { get { return localBackBottomRight + origin; } }

        public Vector3 origin { get; private set; }

        public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
        {
            Rotate(orientation);
        }
        public Box(Vector3 origin, Vector3 halfExtents)
        {
            this.localFrontTopLeft = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontTopRight = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontBottomLeft = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
            this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

            this.origin = origin;
        }

        public void Rotate(Quaternion orientation)
        {
            localFrontTopLeft = RotatePointAroundPivot(localFrontTopLeft, Vector3.zero, orientation);
            localFrontTopRight = RotatePointAroundPivot(localFrontTopRight, Vector3.zero, orientation);
            localFrontBottomLeft = RotatePointAroundPivot(localFrontBottomLeft, Vector3.zero, orientation);
            localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
        }
    }

    static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        Vector3 direction = point - pivot;
        return pivot + rotation * direction;
    }
    #endregion

}

