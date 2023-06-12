using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SetLoaderStatic
{
    public static bool enabled;
    public static bool instructionMode = true;
    public static string setName;
    public static Color middleColor;
    public static RenderTexture preview;
    public static BrickCollectionXML location;


    public static List<BrickXML> GetBrickList(BrickCollectionXML brickColl)
    {
        var unicBricks = new List<BrickXML>();
        foreach (var brick in brickColl.BrickArray)
        {
            if (!CheckInList(unicBricks, brick))
                unicBricks.Add(brick);
        }
        return unicBricks;
    }

    private static bool CheckInList(List<BrickXML> list, BrickXML el)
    {
        foreach (var brick in list)
        {
            if (brick.brickID == el.brickID)
                return true;
        }
        return false;
    }
}
