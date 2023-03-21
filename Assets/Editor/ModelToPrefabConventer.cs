using UnityEditor;
using UnityEngine;

public class ModelToPrefabConventer
{
    [MenuItem("Legotecka/Convert Models to Prefabs")]
    public static void ConvertModels()
    {
        string[] assetPathes = AssetDatabase.FindAssets("", new[] { "Assets/Models/Bricks" });

        for (int i = 0; i < assetPathes.Length; i++)
            assetPathes[i] = AssetDatabase.GUIDToAssetPath(assetPathes[i]);

        foreach (var path in assetPathes)
        {
            if (path.Contains(".meta")) continue;

            if (!path.Contains(".blend"))
            {
                var folderName = path.Split("Bricks/")[1];
                if (System.IO.Directory.Exists("Assets/Resources/Bricks/" + folderName)) continue;
                AssetDatabase.CreateFolder("Assets/Resources/Bricks", folderName);
                continue;
            }

            CreatePrefabFromModel(path);
        }
    }

    private static void CreatePrefabFromModel(string path)
    {
        GameObject model = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
        GameObject prefab = new GameObject();
        prefab.AddComponent<MeshFilter>().sharedMesh = model.GetComponent<MeshFilter>().sharedMesh;
        prefab.AddComponent<MeshRenderer>().sharedMaterial = model.GetComponent<MeshRenderer>().sharedMaterial;

        var savePath = path.Replace("Models", "Resources");
        savePath = savePath.Replace(".blend", ".prefab");

        PrefabUtility.SaveAsPrefabAsset(prefab, savePath);
        Object.DestroyImmediate(prefab);
    }
}
