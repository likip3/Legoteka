using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraControll : MonoBehaviour
{
    public Transform cameraParent;
    public Transform target;
    private Vector3 startPos;
    private Vector3 delta;
    private float zoomDistance;
    private bool touched;

    public static bool movingState = false;
    public float zoomMin = 5;
    public float zoomMax = 20;
    public float minAngle = -20f;
    public float maxAngle = 70f;
    private float rotateSensitivity = 0.1f;
    private float tiltSensitivity = 0.1f;
    private float zoomSpeed = 1f;
    private float x = 0.0f;
    private float y = 0.0f;
    private float angle = 0f;
    
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        zoomDistance = Vector3.Distance(transform.position, target.position);
    }

    private void OnEnable()
    {
        Events.IsNull += OnEnableCamera;
        Events.IsNotNull += OnOffCamera;
    }

    public void Update()
    {
        if (movingState) TrackTouches();
    }

    public void TrackTouches()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            startPos = Input.mousePosition;
            touched = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            touched = false;
        }
        if (Input.touchCount == 2)
        {
            ZoomCamera();
            touched = false;
        }
        else
        {
            if (touched) MoveCamera();
        }
    }

    public void MoveCamera()
    {
        delta = startPos - Input.mousePosition;
        startPos = Input.mousePosition;

        if (Mathf.Abs(delta.x) > 1.0f)
        {
            float horizontal = -delta.x * rotateSensitivity;
            cameraParent.transform.RotateAround(target.position, Vector3.up, horizontal);
        }

        if (Mathf.Abs(delta.y) > 1.0f)
        {
            float vertical = delta.y * tiltSensitivity;
            angle += vertical;
            angle = Mathf.Clamp(angle, minAngle, maxAngle);
            if (angle == maxAngle || angle == minAngle) vertical = 0f;
            cameraParent.transform.RotateAround(target.position, cameraParent.transform.right, vertical);
        }
    }

    public void ZoomCamera()
    {
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
        float deltaMagnitudeDiff = (prevTouchDeltaMag - touchDeltaMag) * 0.01f;

        zoomDistance += deltaMagnitudeDiff * zoomSpeed;
        zoomDistance = Mathf.Clamp(zoomDistance, zoomMin, zoomMax);
        transform.position = target.position - transform.forward * zoomDistance;
    }

    public void OnEnableCamera()
    {
        movingState = true;
    }

    public void OnOffCamera()
    {
        movingState = false;
        startPos = Input.mousePosition;
    }

    private void OnDisable()
    {
        Events.IsNull -= OnEnableCamera;
        Events.IsNotNull -= OnOffCamera;
    }
}