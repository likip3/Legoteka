using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class InstructionMaker : MonoBehaviour
{
    public List<Transform> Bricks;
    public List<string> Positions;
    public List<GameObject> Panels;
    public List<GameObject> Images;
    public GameObject Check;

    private List<string> Instructions;
    private Dictionary<string, Transform> Models;
    public int currentStep;
    public Transform currentModel;

    void Start()
    {
        Instructions = ParseInstructions();
        Models = ParseModels();
        NextStep();
    }

    void Update()
    {
        
    }

    public void NextStep()
    {
        if (currentModel != null)
        {
            foreach (var image in Images)
            {
                if (image.tag == currentModel.tag)
                    image.SetActive(false);
                Destroy(currentModel.gameObject);
            }
        }
        
        if (currentStep < Instructions.Count)
        {
            string[] args = Instructions[currentStep].Split(';');
            currentModel = Instantiate(Models[args[4]]);
            foreach(var image in Images)
            {
                if (image.tag == currentModel.tag)
                    image.SetActive(true);
            }
            currentModel.transform.position = 
                new Vector3(float.Parse(args[0], new CultureInfo("en-US")), float.Parse(args[1], 
                new CultureInfo("en-US")), float.Parse(args[2], new CultureInfo("en-US")));
            currentModel.Rotate(0, 0, float.Parse(args[3]));
            currentStep++;
        }
        else Finish();
    }

    public List<string> ParseInstructions()
    {
        return Positions;
    }

    public Dictionary<string, Transform> ParseModels()
    {
        var bricksDict = new Dictionary<string, Transform>();

        foreach(var brick in Bricks)
        {
            bricksDict.Add(brick.name, brick);
        }
        return bricksDict;
    }

    public void Finish()
    {
        foreach (var panel in Panels)
            panel.SetActive(false);
        Check.SetActive(true);
    }
}
