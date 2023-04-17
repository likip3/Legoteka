using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System;

public static class SaveLoadSystem
{
    public static void SaveXml(BrickCollectionXML collectionToXml) => SaveXml(collectionToXml, "/");

    public static void SaveXml(BrickCollectionXML collectionToXml, string subFolder)
	{
		if (!Directory.Exists(Application.persistentDataPath + subFolder))
        {
			Directory.CreateDirectory(Application.persistentDataPath + subFolder);
		}
		var datapath = Application.persistentDataPath + subFolder + collectionToXml.fileName;

		Type[] extraTypes = { typeof(BrickXML) };
		XmlSerializer serializer = new XmlSerializer(typeof(BrickCollectionXML), extraTypes);

		FileStream fs = new FileStream(datapath, FileMode.Create);
		serializer.Serialize(fs, collectionToXml);
		fs.Close();

	}

	public static BrickCollectionXML DeXml(string name) => DeXml(name, "/");
	public static BrickCollectionXML DeXml(string name, string subFolder)
	{
		if (!File.Exists(Application.persistentDataPath + subFolder + name)) return null;
		Type[] extraTypes = { typeof(BrickXML) };
		XmlSerializer serializer = new XmlSerializer(typeof(BrickCollectionXML), extraTypes);

		FileStream fs = new FileStream(Application.persistentDataPath + subFolder + name, FileMode.Open);
		BrickCollectionXML collectionFromXml = (BrickCollectionXML)serializer.Deserialize(fs);
		fs.Close();

		return collectionFromXml;
	}
}

[XmlRoot("BrickCollectionRoot")]
[XmlType("BrickCollection")]
public class BrickCollectionXML
{
	[XmlElement("FileName")]
	public string fileName;

	[XmlElement("Time")]
	public DateTime time;
    private static List<BrickXML> brickXMLs = new List<BrickXML>();
    [XmlArray("BrickArray")]
	[XmlArrayItem("Brick")]
	public List<BrickXML> BrickArray = brickXMLs;

    public BrickCollectionXML(string fileName)
    {
        this.fileName = fileName;
        this.time = DateTime.Now;
    }

    public BrickCollectionXML() { }
}

public class BrickXML
{
    public BrickXML(string brickID, Vector3 position, Quaternion rotation)
    {
        this.brickID = brickID;
        this.position = position;
        this.rotation = rotation;
    }

	public BrickXML() { }

    [XmlElement("BrickID")]
	public string brickID { get; set; }

	[XmlElement("Position")]
	public Vector3 position { get; set; }

	[XmlElement("Rotation")]
	public Quaternion rotation { get; set; }

}