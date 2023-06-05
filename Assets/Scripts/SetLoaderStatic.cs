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


    public static List<BrickXML> GetBrickList(BrickCollectionXML brickColl) => brickColl.BrickArray.Distinct().ToList();


}
