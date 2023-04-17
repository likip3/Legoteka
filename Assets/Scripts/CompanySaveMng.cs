using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;

public class CompanySaveMng : MonoBehaviour
{
	// The dictionaries can be accessed throught a property
	[SerializeField]
	BrickDictSer storyBrickDict;

	public void SaveButton()
    {
		var goArray = FindObjectsOfType(typeof(Brick));

		var collectionToXml = new StoryBrickArrayXML(SceneManager.GetActiveScene().name);

		foreach (Brick brick in goArray)
        {
			collectionToXml.brickArray.Add(new StoryBrickXML(brick.ID, brick.transform.position, brick.transform.rotation));
		}


		if (!Directory.Exists(Application.persistentDataPath + "/Story/"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/Story/");
		}
		var datapath = Application.persistentDataPath + "/Story/" + collectionToXml.fileName;

		Type[] extraTypes = { typeof(StoryBrickXML) };
		XmlSerializer serializer = new XmlSerializer(typeof(StoryBrickArrayXML), extraTypes);

		FileStream fs = new FileStream(datapath, FileMode.Create);
		serializer.Serialize(fs, collectionToXml);
		fs.Close();

	}

	public void LoadButton()
	{
		if (!File.Exists(Application.persistentDataPath + "/Story/" + SceneManager.GetActiveScene().name)) return;
		Type[] extraTypes = { typeof(StoryBrickXML) };
		XmlSerializer serializer = new XmlSerializer(typeof(StoryBrickArrayXML), extraTypes);

		FileStream fs = new FileStream(Application.persistentDataPath + "/Story/" + SceneManager.GetActiveScene().name, FileMode.Open);
		StoryBrickArrayXML collectionFromXml = (StoryBrickArrayXML)serializer.Deserialize(fs);
		fs.Close();

        foreach (var brick in collectionFromXml.brickArray)
        {
			var tempBr = Instantiate(storyBrickDict[brick.ID]);
			tempBr.transform.SetPositionAndRotation(brick.Pos, brick.rotation);
        }
	}
}

[XmlRoot("StoryBrickCollectionRoot")]
[XmlType("StoryBrickCollection")]
public class StoryBrickArrayXML
{
	[XmlElement("FileName")]
	public string fileName;

	[XmlArray("StoryBrickArray")]
	[XmlArrayItem("Brick")]
	public List<StoryBrickXML> brickArray = new();

    public StoryBrickArrayXML(string fileName)
    {
        this.fileName = fileName;
    }

	public StoryBrickArrayXML() { }
}

public class StoryBrickXML
{
	[XmlElement("BrickID")]
	public string ID;
	[XmlElement("Position")]
	public Vector3 Pos;
    [XmlElement("Rotation")]
	public Quaternion rotation;

    public StoryBrickXML(string iD, Vector3 pos, Quaternion rotation)
    {
        ID = iD;
        Pos = pos;
        this.rotation = rotation;
    }
	public StoryBrickXML()
    {

    }
}