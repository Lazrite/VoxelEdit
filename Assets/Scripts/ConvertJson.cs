using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// �u���b�N�P�������
/// </summary>
[System.Serializable]
public class BlockPropaties
{
    public Vector3 transform;   // �g�����X�t�H�[��
    public Vector3 rotation;    // ��]
    public Vector3 scale;       // �傫��
    public int id;              // ���f��ID
    public bool isRandomize;    // �����_�}�C�U��
    public bool isMonoSpaced;   // �������ǂ���
    public bool isCollision;    // �����蔻�肪���邩
}

/// <summary>
/// �V���A���C�Y�p�N���X
/// </summary>
[System.Serializable]
public class GridPropaties
{
    public List<BlockPropaties> propaties = new List<BlockPropaties>();
}

/// <summary>
/// JSON�R���o�[�^�[
/// </summary>
public class ConvertJson
{
    public static void SaveJson(List<GameObject> obj)
    {
        StreamWriter writer;
        writer = new StreamWriter(Application.dataPath + "/savedata.json", false);
        GridPropaties propaty = new GridPropaties();

        // �u���b�N�����擾�A�V���A���C�Y�p�N���X�Ɋi�[
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

        // ��������
        writer.Write(jsonstr);
        writer.Flush();

        writer.Close();
    }
}
