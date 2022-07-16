using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// ブロック１つがもつ情報
/// </summary>
[System.Serializable]
public class BlockPropaties
{
    public Vector3 transform;   // トランスフォーム
    public Vector3 rotation;    // 回転
    public Vector3 scale;       // 大きさ
    public int id;              // モデルID
    public bool isRandomize;    // ランダマイザ可否
    public bool isMonoSpaced;   // 等幅かどうか
    public bool isCollision;    // 当たり判定があるか
}

/// <summary>
/// シリアライズ用クラス
/// </summary>
[System.Serializable]
public class GridPropaties
{
    public List<BlockPropaties> propaties = new List<BlockPropaties>();
}

/// <summary>
/// JSONコンバーター
/// </summary>
public class ConvertJson
{
    public static void SaveJson(List<GameObject> obj)
    {
        StreamWriter writer;
        writer = new StreamWriter(Application.dataPath + "/savedata.json", false);
        GridPropaties propaty = new GridPropaties();

        // ブロック情報を取得、シリアライズ用クラスに格納
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

        // 書き込み
        writer.Write(jsonstr);
        writer.Flush();

        writer.Close();
    }
}
