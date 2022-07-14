using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class BlockPropaties
{
    public Vector3 transform;
    public Vector3 rotation;
    public Vector3 scale;
    public int id;
    public bool isRandomize;
    public bool isMonoSpaced;
    public bool isCollision;
}

[System.Serializable]
public class GridPropaties
{
    public List<BlockPropaties> propaties = new List<BlockPropaties>();
}

public class ConvertJson
{
    public static void SaveJson(List<GameObject> obj)
    {
        StreamWriter writer;
        writer = new StreamWriter(Application.dataPath + "/savedata.json", false);
        GridPropaties propaty = new GridPropaties();

        foreach (var dataInfo in obj)
        {
            BlockPropaties data = new BlockPropaties();
            data.transform = dataInfo.transform.position;
            data.rotation = dataInfo.transform.eulerAngles;
            data.scale = dataInfo.transform.localScale;
            data.isRandomize = dataInfo.transform.GetComponent<JsonConvertPropaties>().isRandomize;
            data.isMonoSpaced = dataInfo.transform.GetComponent<JsonConvertPropaties>().isMonoSpaced;
            data.isCollision = dataInfo.transform.GetComponent<JsonConvertPropaties>().isCollision;
            data.id = dataInfo.transform.GetComponent<JsonConvertPropaties>().ID;
            propaty.propaties.Add(data);

        }

        string jsonstr = JsonUtility.ToJson(propaty);

        writer.Write(jsonstr);
        writer.Flush();

        writer.Close();
    }
}
