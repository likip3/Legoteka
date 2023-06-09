using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionWalker : MonoBehaviour
{
    private int stepIndex = 0;
    private Stack<Brick> brickSteps;
    private Brick ghostNow;

    void Start()
    {
        brickSteps = new();
        FreeModeBrickPlacer.LoadInstructionFor(SetLoaderStatic.setName, "/FreeModeSave/");
        FreeModeBrickPlacer.StartInstrucrion(true);
        ghostNow = SpawnGhostForIdx(0);
    }


    public void NextStep()
    {
        if (stepIndex > FreeModeBrickPlacer.InstructionSteps.Count - 1) return;
        stepIndex++;
        var mat = new Material(BrickDatabase.DefaultMaterial);
        (ghostNow.GetComponent<MeshRenderer>().material = mat).color = SQLiteTasker.GetColorById(ghostNow.ID);
        brickSteps.Push(ghostNow);
        if (stepIndex == FreeModeBrickPlacer.InstructionSteps.Count)
        {
            ghostNow = null;
            return;
        }

        ghostNow = SpawnGhostForIdx(stepIndex);
    }

    public void PreviousStep()
    {
        if (stepIndex <= 0) return;
        stepIndex--;
        if(ghostNow is not null) 
            Destroy(ghostNow.gameObject);
        ghostNow = brickSteps.Pop();
        var color = SQLiteTasker.GetColorById(ghostNow.ID);
        color.a = .35f;
        (ghostNow.GetComponent<MeshRenderer>().material = FreeModeBrickPlacer.GhostMaterial).color = color;
    }


    private Brick SpawnGhostForIdx(int index)
    {
        //Destroy(GameObject.FindGameObjectWithTag("ghostBrick"));

        var brick = FreeModeBrickPlacer.InstructionSteps[index];

        var brickGM = Instantiate(SQLiteTasker.BrickDict[brick.ID]);
        brickGM.transform.SetPositionAndRotation(brick.pos, brick.rotation);


        Color color = SQLiteTasker.GetColorById(brick.ID);
        brickGM.color = color;
        color.a = .35f;
        brickGM.ID = brick.ID;
        brickGM.tag = "ghostBrick";
        (brickGM.GetComponent<MeshRenderer>().material = FreeModeBrickPlacer.GhostMaterial).color = color;
        return brickGM;
    }
}
