using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SetLoaderStatic
{
    public static bool enabled;
    public static bool instructionMode;
    public static string setName;
    public static Color middleColor;
    public static RenderTexture preview;


    public static List<BrickXML> GetBrickList(BrickCollectionXML brickColl) => brickColl.BrickArray.Distinct().ToList();


}
