using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeModeBrickPlacer : MonoBehaviour
{
    private Camera mainCamera;

    private Transform controllableBrick;
    private int height;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.touchCount > 0 && !CameraControll.movingState)
        {
            var touch = Input.GetTouch(0);


            #region DebugRay
            var rayt = mainCamera.ScreenPointToRay(touch.position);
            var groundPlaneTest = new Plane(Vector3.up, Vector3.zero);
            groundPlaneTest.Raycast(rayt, out float distanceTest);
            Debug.DrawRay(rayt.origin, rayt.direction * distanceTest, Color.yellow);
            #endregion


            if (controllableBrick != null)
            {
                var groundPlane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = mainCamera.ScreenPointToRay(touch.position);

                Vector3 offset = mainCamera.transform.up * 0.1f; //?
                ray.origin += offset;

                if (groundPlane.Raycast(ray, out float distance))
                {
                    Vector3 worldPosition = ray.GetPoint(distance);
                    int x = Mathf.RoundToInt(worldPosition.x);
                    int z = Mathf.RoundToInt(worldPosition.z);

                    if (touch.phase == TouchPhase.Moved)
                    {
                        controllableBrick.transform.position = new Vector3(x, height, z);
                    }
                }
            }
        }
    }


    public void SelectBrick(Brick buildingPrefab)
    {
        if (controllableBrick == null)
            controllableBrick = Instantiate(buildingPrefab).transform;
        else
        {
            Destroy(controllableBrick.gameObject);
            controllableBrick = Instantiate(buildingPrefab).transform;
        }
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
    }
}
