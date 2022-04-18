#if UNITY_EDITOR

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

/// <summary>
/// どの方向が端に面しているかを表す
/// </summary>
[Flags]
public enum EdgeOrientation
{
    /// <summary>
    /// 端でない
    /// </summary>
    None = 0,
    /// <summary>
    /// X正方向
    /// </summary>
    PositiveX = 1,
    /// <summary>
    /// Y正方向
    /// </summary>
    PositiveY = 2,
    /// <summary>
    /// Z正方向
    /// </summary>
    PositiveZ = 4,
    /// <summary>
    /// X負方向
    /// </summary>
    NegativeX = 8,
    /// <summary>
    /// Y負方向
    /// </summary>
    NegativeY = 16,
    /// <summary>
    /// Z負方向
    /// </summary>
    NegativeZ = 32,
}

/// <summary>
/// グリッドの本体クラス。グリッドを構成する情報は大体ココにある
/// </summary>
public class EditorGridField : MonoBehaviour
{
    private Vector3[] verts;    //ポリゴンの頂点を入れる
    private int[] triangles;    //三角形を描く際に、頂点の描画順を指定する

    [SerializeField] public Material material;
    [HideInInspector] public Vector3Int size;
    [HideInInspector] public float lineSize;
    [HideInInspector] public GameObject areaGameObject;

    private GameObject instantiateBuffer;

    [SerializeField] public Vector3[,,] gridPosFromIndexMultiple;
    [SerializeField] public Vector3[] gridPosFromIndex;
    [SerializeField] public bool[] isPlaced;
    [SerializeField] public GameObject[] placedObjects;
    [SerializeField] public (GameObject[] obj, float[] index) ablePLacementSurround;

    // 配列のバッファ
    private int arrayBuffer = 0;

    /// <summary>
    /// グリッドを形成するのに必要なものの作成
    /// </summary>
    public void InstantiateGridField()
    {
        // トランスフォームからの値の編集を禁止
        this.gameObject.GetComponent<EditorGridField>().hideFlags = HideFlags.NotEditable;

        isPlaced = new bool[size.x * size.y * size.z];
        gridPosFromIndexMultiple = new Vector3[size.x, size.y, size.z];
        gridPosFromIndex = new Vector3[size.x * size.y * size.z];
        placedObjects = new GameObject[size.x * size.y * size.z];
        ablePLacementSurround.obj = new GameObject[(size.x * size.y) * 2 + (size.x * size.z) * 2 + (size.y * size.z) * 2];
        ablePLacementSurround.index = new float[(size.x * size.y) * 2 + (size.x * size.z) * 2 + (size.y * size.z) * 2];
        areaGameObject = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorSource/AblePlacementQuad.prefab", typeof(GameObject)) as GameObject;

        for (int i = 0; i < size.z; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                for (int k = 0; k < size.y; k++)
                {
                    gridPosFromIndexMultiple[j, k, i] = new Vector3(this.transform.position.x + 0.5f + j, this.transform.position.y + 0.5f + k, this.transform.position.z + 0.5f + i);
                }
            }
        }

        for (int i = 0; i < size.z; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                for (int k = 0; k < size.y; k++)
                {
                    gridPosFromIndex[(i * size.x * size.y) + (j * size.y) + k] = gridPosFromIndexMultiple[j, k, i];
                }
            }
        }

        //新しいMeshを作成
        Mesh mesh = new Mesh();

        //頂点の番号をsize分確保、縦横の線が一本ずつなくなるので+2を入れる、一本の線は頂点6つで表示させるので*6
        triangles = new int[12 * 6];
        //頂点の座標をsize分確保
        verts = new Vector3[12 * 6];

        //頂点番号を割り当て
        for (int i = 0; i < triangles.Length; i++)
        {
            triangles[i] = i;
        }

        //何回for分が回ったかをカウントさせる
        int x = 0, y = 0;

        // 横線

        verts[arrayBuffer] = new Vector3(x, 0, 0);
        verts[arrayBuffer + 1] = new Vector3(x, size.y, 0);
        verts[arrayBuffer + 2] = new Vector3(lineSize + x, size.y, 0);
        verts[arrayBuffer + 3] = new Vector3(lineSize + x, size.y, 0);
        verts[arrayBuffer + 4] = new Vector3(lineSize + x, 0, 0);
        verts[arrayBuffer + 5] = new Vector3(x, 0, 0);

        verts[arrayBuffer + 6] = new Vector3(size.x, 0, 0);
        verts[arrayBuffer + 7] = new Vector3(size.x, size.y, 0);
        verts[arrayBuffer + 8] = new Vector3(lineSize + size.x, size.y, 0);
        verts[arrayBuffer + 9] = new Vector3(lineSize + size.x, size.y, 0);
        verts[arrayBuffer + 10] = new Vector3(lineSize + size.x, 0, 0);
        verts[arrayBuffer + 11] = new Vector3(size.x, 0, 0);

        verts[arrayBuffer + 12] = new Vector3(x, 0, size.z);
        verts[arrayBuffer + 13] = new Vector3(x, size.y, size.z);
        verts[arrayBuffer + 14] = new Vector3(lineSize + x, size.y, size.z);
        verts[arrayBuffer + 15] = new Vector3(lineSize + x, size.y, size.z);
        verts[arrayBuffer + 16] = new Vector3(lineSize + x, 0, size.z);
        verts[arrayBuffer + 17] = new Vector3(x, 0, size.z);

        verts[arrayBuffer + 18] = new Vector3(size.x, 0, size.z);
        verts[arrayBuffer + 19] = new Vector3(size.x, size.y, size.z);
        verts[arrayBuffer + 20] = new Vector3(lineSize + size.x, size.y, size.z);
        verts[arrayBuffer + 21] = new Vector3(lineSize + size.x, size.y, size.z);
        verts[arrayBuffer + 22] = new Vector3(lineSize + size.x, 0, size.z);
        verts[arrayBuffer + 23] = new Vector3(size.x, 0, size.z);

        arrayBuffer += 24;

        //縦線

        verts[arrayBuffer] = new Vector3(0, y, 0);
        verts[arrayBuffer + 1] = new Vector3(size.x + lineSize, y, 0);
        verts[arrayBuffer + 2] = new Vector3(0, y - lineSize, 0);
        verts[arrayBuffer + 3] = new Vector3(size.x + lineSize, y, 0);
        verts[arrayBuffer + 4] = new Vector3(size.x + lineSize, y - lineSize, 0);
        verts[arrayBuffer + 5] = new Vector3(0, y - lineSize, 0);

        verts[arrayBuffer + 6] = new Vector3(0, size.y, 0);
        verts[arrayBuffer + 7] = new Vector3(size.x + lineSize, size.y, 0);
        verts[arrayBuffer + 8] = new Vector3(0, size.y - lineSize, 0);
        verts[arrayBuffer + 9] = new Vector3(size.x + lineSize, size.y, 0);
        verts[arrayBuffer + 10] = new Vector3(size.x + lineSize, size.y - lineSize, 0);
        verts[arrayBuffer + 11] = new Vector3(0, size.y - lineSize, 0);

        verts[arrayBuffer + 12] = new Vector3(0, y, size.z);
        verts[arrayBuffer + 13] = new Vector3(size.x + lineSize, y, size.z);
        verts[arrayBuffer + 14] = new Vector3(0, y - lineSize, size.z);
        verts[arrayBuffer + 15] = new Vector3(size.x + lineSize, y, size.z);
        verts[arrayBuffer + 16] = new Vector3(size.x + lineSize, y - lineSize, size.z);
        verts[arrayBuffer + 17] = new Vector3(0, y - lineSize, size.z);

        verts[arrayBuffer + 18] = new Vector3(0, size.y, size.z);
        verts[arrayBuffer + 19] = new Vector3(size.x + lineSize, size.y, size.z);
        verts[arrayBuffer + 20] = new Vector3(0, size.y - lineSize, size.z);
        verts[arrayBuffer + 21] = new Vector3(size.x + lineSize, size.y, size.z);
        verts[arrayBuffer + 22] = new Vector3(size.x + lineSize, size.y - lineSize, size.z);
        verts[arrayBuffer + 23] = new Vector3(0, size.y - lineSize, size.z);

        arrayBuffer += 24;

        verts[arrayBuffer] = new Vector3(x, y, 0);
        verts[arrayBuffer + 1] = new Vector3(x, y, size.z);
        verts[arrayBuffer + 2] = new Vector3(x, y - lineSize, 0);
        verts[arrayBuffer + 3] = new Vector3(x, y, size.z);
        verts[arrayBuffer + 4] = new Vector3(x, y - lineSize, size.z);
        verts[arrayBuffer + 5] = new Vector3(x, y - lineSize, 0);

        verts[arrayBuffer + 6] = new Vector3(size.x, size.y, 0);
        verts[arrayBuffer + 7] = new Vector3(size.x, size.y, size.z);
        verts[arrayBuffer + 8] = new Vector3(size.x, size.y - lineSize, 0);
        verts[arrayBuffer + 9] = new Vector3(size.x, size.y, size.z);
        verts[arrayBuffer + 10] = new Vector3(size.x, size.y - lineSize, size.z);
        verts[arrayBuffer + 11] = new Vector3(size.x, size.y - lineSize, 0);

        verts[arrayBuffer + 12] = new Vector3(x, size.y, 0);
        verts[arrayBuffer + 13] = new Vector3(x, size.y, size.z);
        verts[arrayBuffer + 14] = new Vector3(x, size.y - lineSize, 0);
        verts[arrayBuffer + 15] = new Vector3(x, size.y, size.z);
        verts[arrayBuffer + 16] = new Vector3(x, size.y - lineSize, size.z);
        verts[arrayBuffer + 17] = new Vector3(x, size.y - lineSize, 0);

        verts[arrayBuffer + 18] = new Vector3(size.x, y, 0);
        verts[arrayBuffer + 19] = new Vector3(size.x, y, size.z);
        verts[arrayBuffer + 20] = new Vector3(size.x, y - lineSize, 0);
        verts[arrayBuffer + 21] = new Vector3(size.x, y, size.z);
        verts[arrayBuffer + 22] = new Vector3(size.x, y - lineSize, size.z);
        verts[arrayBuffer + 23] = new Vector3(size.x, y - lineSize, 0);

        arrayBuffer += 24;

        //作った頂点番号、座標データを作成したmeshに追加
        mesh.vertices = verts;
        mesh.triangles = triangles;
        arrayBuffer = 0;

        //再計算()
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //再計算後に完成したMeshを追加
        GetComponent<MeshFilter>().mesh = mesh;
        //設定したMaterialを反映
        GetComponent<MeshRenderer>().material = material;


        int surroundBuffer = 0;

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (isPlaced[(j + 1) + (i * size.y) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.Euler(0, -180, 0), gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = (j + 1) + (i * size.y);
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = (j + 1) + (i * size.y);
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (isPlaced[(j + 1) + (i * size.y) + (size.x * size.y * (size.z - 1)) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, j + 0.5f, size.z), Quaternion.identity, gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = (j + 1) + (i * size.y) + (size.x * size.y * (size.z - 1));
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = (j + 1) + (i * size.y) + (size.x * size.y * (size.z - 1));
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (isPlaced[(((j * size.x * size.y) + 1) + i) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(0, i + 0.5f, j + 0.5f), Quaternion.Euler(0, -90, 0), gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = ((j * size.x * size.y) + 1) + i;
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = ((j * size.x * size.y) + 1) + i;
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (isPlaced[((j * size.x * size.y) + 1) + i + (size.y * (size.x - 1)) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(size.x, i + 0.5f, j + 0.5f), Quaternion.Euler(0, 90, 0), gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = ((j * size.x * size.y) + 1) + i + (size.y * (size.x - 1));
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = ((j * size.x * size.y) + 1) + i + (size.y * (size.x - 1));
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (isPlaced[(((j * (size.x * size.y)) + 1) + (i * size.y)) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, 0, j + 0.5f), Quaternion.Euler(90, 0, 0), gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = (((j * (size.x * size.y)) + 1) + (i * size.y));
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = (((j * (size.x * size.y)) + 1) + (i * size.y));
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (isPlaced[(((j * (size.x * size.y)) + 1) + (i * size.y)) + (size.y - 1) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, size.y, j + 0.5f), Quaternion.Euler(-90, 0, 0), gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = (((j * (size.x * size.y)) + 1) + (i * size.y)) + (size.y - 1);
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = (((j * (size.x * size.y)) + 1) + (i * size.y)) + (size.y - 1);
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }
    }

    /// <summary>
    /// グリッドを一掃して綺麗にするメソッド
    /// </summary>
    public void ClearGrid()
    {
        for (int i = transform.childCount; i > 0; --i)
        {
            GameObject.DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
            Debug.Log(gameObject.transform.childCount);
        }
    }

    /// <summary>
    /// グリッドが動いたときに内部のグリッド座標を再計算するメソッド
    /// </summary>
    public void ReCalculationGridPos()
    {
        gridPosFromIndexMultiple = new Vector3[size.x, size.y, size.z];
        gridPosFromIndex = new Vector3[size.x * size.y * size.z];

        for (int i = 0; i < size.z; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                for (int k = 0; k < size.y; k++)
                {
                    gridPosFromIndexMultiple[j, k, i] = new Vector3(this.transform.position.x + 0.5f + j, this.transform.position.y + 0.5f + k, this.transform.position.z + 0.5f + i);
                }
            }
        }

        for (int i = 0; i < size.z; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                for (int k = 0; k < size.y; k++)
                {
                    gridPosFromIndex[(i * size.x * size.y) + (j * size.y) + k] = gridPosFromIndexMultiple[j, k, i];
                }
            }
        }
    }

    public Vector3Int ReturnGridSquarePoint(int index)
    {
        gridPosFromIndex = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGridField>()
            .gridPosFromIndex;
        gridPosFromIndexMultiple = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGridField>()
            .gridPosFromIndexMultiple;

        if (index - 1 < 0 || index > size.x * size.y * size.z)
        {
            Debug.Log("不正なインデックスです");
            return new Vector3Int(-1, -1, -1);
        }

        for (int i = 0; i < size.z; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                for (int k = 0; k < size.y; k++)
                {
                    if (gridPosFromIndex[index - 1] == gridPosFromIndexMultiple[j, k, i])
                    {
                        return new Vector3Int(j, k, i);
                    }
                }
            }
        }

        Debug.Log("不正なインデックスです");
        return new Vector3Int(-1, -1, -1);
    }

    /// <summary>
    /// レイキャストの情報を基にグリッドの端かどうかを判定します
    /// このメソッドはグリッド領域外(グリッド端のプレハブの外側)から接触された場合に使用される
    /// </summary>
    /// <param name="hit"> レイキャスト情報 </param>
    /// <returns> 端である場合true, インデックス領域外、端でない場合false </returns>
    public bool IsCheckGridEdgeFromHit(RaycastHit hit)
    {
        size = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGridField>().size;

        var gridPoint = ReturnGridSquarePoint(ConjecturePlacementObjIndex(hit));

        if (gridPoint == new Vector3(-1, -1, -1))
        {
            return false;
        }

        if (gridPoint.x == 0 && hit.normal == Vector3.right)
        {
            return true;
        }
        else if (gridPoint.x == size.x - 1 && hit.normal == Vector3.left)
        {
            return true;
        }
        else if (gridPoint.y == 0 && hit.normal == Vector3.up)
        {
            return true;
        }
        else if (gridPoint.y == size.y - 1 && hit.normal == Vector3.down)
        {
            return true;
        }
        else if (gridPoint.z == 0 && hit.normal == Vector3.forward)
        {
            return true;
        }
        else if (gridPoint.z == size.z - 1 && hit.normal == Vector3.back)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 対象インデックスがどの方向の端と接しているかを判定するメソッド
    /// </summary>
    /// <param name="index"> グリッドのインデックス </param>
    /// <returns> 面している方向 </returns>
    public EdgeOrientation isCheckGridEdgeFromIndex(int index)
    {
        size = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGridField>().size;

        var gridPoint = ReturnGridSquarePoint(index);

        if (gridPoint == new Vector3(-1, -1, -1))
        {
            return EdgeOrientation.None;
        }

        var edgeFlg = (EdgeOrientation.None, EdgeOrientation.None, EdgeOrientation.None);

        if (gridPoint.x == 0)
        {
            edgeFlg.Item1 = EdgeOrientation.NegativeX;
        }
        else if (gridPoint.x == size.x - 1)
        {
            edgeFlg.Item1 = EdgeOrientation.PositiveX;
        }
        if (gridPoint.y == 0)
        {
            edgeFlg.Item2 = EdgeOrientation.NegativeY;
        }
        else if (gridPoint.y == size.y - 1)
        {
            edgeFlg.Item2 = EdgeOrientation.PositiveY;
        }
        if (gridPoint.z == 0)
        {
            edgeFlg.Item3 = EdgeOrientation.NegativeZ;
        }
        else if (gridPoint.z == size.z - 1)
        {
            edgeFlg.Item3 = EdgeOrientation.PositiveZ;
        }

        return edgeFlg.Item1 | edgeFlg.Item2 | edgeFlg.Item3;
    }

    public int ConjecturePlacementObjIndex(RaycastHit hitObj)
    {
        size = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGridField>().size;

        var hitAngles = hitObj.normal;
        var hitIndex = hitObj.collider.GetComponent<GridRelatedInfo>().gridIndex;

        if (hitAngles == Vector3.back)
        {
            return hitIndex - (size.x * size.y);
        }
        else if (hitAngles == Vector3.forward)
        {
            return hitIndex + (size.x * size.y);
        }
        else if (hitAngles == Vector3.right)
        {
            return hitIndex + size.y;
        }
        else if (hitAngles == Vector3.left)
        {
            return hitIndex - size.y;
        }
        else if (hitAngles == Vector3.down)
        {
            return hitIndex - 1;
        }
        else if (hitAngles == Vector3.up)
        {
            return hitIndex + 1;
        }

        return -1;
    }

    public void PreLoadGridInfo()
    {
        gridPosFromIndexMultiple = new Vector3[size.x, size.y, size.z];
        ablePLacementSurround.obj = new GameObject[(size.x * size.y) * 2 + (size.x * size.z) * 2 + (size.y * size.z) * 2];
        ablePLacementSurround.index = new float[(size.x * size.y) * 2 + (size.x * size.z) * 2 + (size.y * size.z) * 2];

        for (int i = 0; i < size.z; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                for (int k = 0; k < size.y; k++)
                {
                    gridPosFromIndexMultiple[j, k, i] = new Vector3(this.transform.position.x + 0.5f + j, this.transform.position.y + 0.5f + k, this.transform.position.z + 0.5f + i);
                }
            }
        }

        int surroundBuffer = 0;

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (isPlaced[(j + 1) + (i * size.y) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.Euler(0, -180, 0), gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = (j + 1) + (i * size.y);
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = (j + 1) + (i * size.y);
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (isPlaced[(j + 1) + (i * size.y) + (size.x * size.y * (size.z - 1)) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, j + 0.5f, size.z), Quaternion.identity, gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = (j + 1) + (i * size.y) + (size.x * size.y * (size.z - 1));
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = (j + 1) + (i * size.y) + (size.x * size.y * (size.z - 1));
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (isPlaced[(((j * size.x * size.y) + 1) + i) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(0, i + 0.5f, j + 0.5f), Quaternion.Euler(0, -90, 0), gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = ((j * size.x * size.y) + 1) + i;
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = ((j * size.x * size.y) + 1) + i;
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (isPlaced[((j * size.x * size.y) + 1) + i + (size.y * (size.x - 1)) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(size.x, i + 0.5f, j + 0.5f), Quaternion.Euler(0, 90, 0), gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = ((j * size.x * size.y) + 1) + i + (size.y * (size.x - 1));
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = ((j * size.x * size.y) + 1) + i + (size.y * (size.x - 1));
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (isPlaced[(((j * (size.x * size.y)) + 1) + (i * size.y)) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, 0, j + 0.5f), Quaternion.Euler(90, 0, 0), gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = (((j * (size.x * size.y)) + 1) + (i * size.y));
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = (((j * (size.x * size.y)) + 1) + (i * size.y));
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (isPlaced[(((j * (size.x * size.y)) + 1) + (i * size.y)) + (size.y - 1) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, size.y, j + 0.5f), Quaternion.Euler(-90, 0, 0), gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = (((j * (size.x * size.y)) + 1) + (i * size.y)) + (size.y - 1);
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = (((j * (size.x * size.y)) + 1) + (i * size.y)) + (size.y - 1);
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }
    }
}

#endif // UNITY_EDITOR