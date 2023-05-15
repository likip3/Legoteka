using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BricksGrid : MonoBehaviour
{
    public Vector2Int GridSize = new Vector2Int(10, 10);
    private Brick[,] grid;
    private Brick flyingBrick;
    private Camera mainCamera;
    private Touch touch;
    public InstructionMaker instruction;
    private int step;
    public GameObject uiObjectWrong;
    public GameObject uiObjectRight;
    public AudioSource source;
    public AudioClip wrong;
    public AudioClip audioClip;

    private void Start()
    {
        uiObjectWrong.SetActive(false);
        uiObjectRight.SetActive(false);
    }

    private void Awake()
    {
        grid = new Brick[GridSize.x, GridSize.y];
        mainCamera = Camera.main;
    }

    public void StartPlacingBrick(Brick buildingPrefab)
    {
        if (flyingBrick == null && instruction.currentStep > step)
        {
            flyingBrick = Instantiate(buildingPrefab);
            step++;
        }

        else if(flyingBrick != null && instruction.currentStep == step)
        {
            Destroy(flyingBrick.gameObject);
            flyingBrick = Instantiate(buildingPrefab);
        }

        else if (flyingBrick == null && instruction.currentStep == step)
        {
            flyingBrick = Instantiate(buildingPrefab);
        }
        flyingBrick.gameObject.transform.rotation = instruction.currentModel.rotation;
        flyingBrick.transform.position = new Vector3(0, 0, 6);
    }

    private void Update()
    {
        if(Input.touchCount > 0 && !CameraControll.movingState)
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
                        flyingBrick.transform.position = new Vector3(x, instruction.currentModel.position.y, z);
                    }
                    
                    if (flyingBrick.transform.position == instruction.currentModel.transform.position && flyingBrick.tag.Contains(instruction.currentModel.tag))
                    {
                        source.PlayOneShot(audioClip);
                        uiObjectRight.SetActive(true);
                        PlaceFlyingBrick(x, z);
                        instruction.NextStep();
                        StartCoroutine("Wait");
                    }

                    else if(flyingBrick.transform.position == instruction.currentModel.transform.position && !flyingBrick.tag.Contains(instruction.currentModel.tag))
                    {
                        uiObjectWrong.SetActive(true);
                        source.PlayOneShot(wrong);
                        Destroy(flyingBrick.gameObject);
                        StartCoroutine("Wait");                      
                    }
                        
                }
                
            }
        }
        CheckBrick();
        
    }

    private void PlaceFlyingBrick(int placeX, int placeY)
    {
        //for (int x = 0; x < flyingBrick.Size.x; x++)
        //{
        //    for (int y = 0; y < flyingBrick.Size.y; y++)
        //    {
        //        grid[placeX + x, placeY + y] = flyingBrick;
        //    }
        //}
        flyingBrick = null;
    }

    public void CheckBrick()
    {
        if (flyingBrick == null && touch.phase == TouchPhase.Moved)
            Events.InvokeIfNull();
        else
            Events.InvokeIfNotNull();
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.5f);
        uiObjectRight.SetActive(false);
        uiObjectWrong.SetActive(false);
    }
}
