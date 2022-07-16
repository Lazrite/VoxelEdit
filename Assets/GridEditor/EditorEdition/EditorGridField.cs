#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

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
    [HideInInspector] public float gridScale;
    [HideInInspector] public GameObject areaGameObject;
    [HideInInspector] public List<GameObject> inGridObjects;

    private GameObject instantiateBuffer;

    // 配列のバッファ
    private int arrayBuffer = 0;

    /// <summary>
    /// グリッドを形成するのに必要なものの作成
    /// </summary>
    public void InstantiateGridField()
    {
        // トランスフォームからの値の編集を禁止
        this.gameObject.GetComponent<EditorGridField>().hideFlags = HideFlags.NotEditable;

        areaGameObject = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorSource/AblePlacementQuad.prefab", typeof(GameObject)) as GameObject;

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

        gridScale = 1.0f;

        int surroundBuffer = 0;

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                //if (isPlaced[(j + 1) + (i * size.y) - 1])
                //{
                //    continue;
                //}

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.Euler(0, -180, 0), gameObject.transform);
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                //if (isPlaced[(j + 1) + (i * size.y) + (size.x * size.y * (size.z - 1)) - 1])
                //{
                //    continue;
                //}

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, j + 0.5f, size.z), Quaternion.identity, gameObject.transform);
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                //if (isPlaced[(((j * size.x * size.y) + 1) + i) - 1])
                //{
                //    continue;
                //}

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(0, i + 0.5f, j + 0.5f), Quaternion.Euler(0, -90, 0), gameObject.transform);
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                //if (isPlaced[((j * size.x * size.y) + 1) + i + (size.y * (size.x - 1)) - 1])
                //{
                //    continue;
                //}

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(size.x, i + 0.5f, j + 0.5f), Quaternion.Euler(0, 90, 0), gameObject.transform);
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                //if (isPlaced[(((j * (size.x * size.y)) + 1) + (i * size.y)) - 1])
                //{
                //    continue;
                //}

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, 0, j + 0.5f), Quaternion.Euler(90, 0, 0), gameObject.transform);
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                //if (isPlaced[(((j * (size.x * size.y)) + 1) + (i * size.y)) + (size.y - 1) - 1])
                //{
                //    continue;
                //}

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, size.y, j + 0.5f), Quaternion.Euler(-90, 0, 0), gameObject.transform);
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                instantiateBuffer.name = "AblePlacementAround";
                EditorUtility.SetDirty(instantiateBuffer);
                surroundBuffer++;
            }
        }

        inGridObjects = new List<GameObject>();
    }

    /// <summary>
    /// グリッドを一掃して綺麗にするメソッド
    /// </summary>
    public void ClearGrid()
    {
        for (int i = transform.childCount; i > 0; --i)
        {
            GameObject.DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
        }

        inGridObjects?.Clear();
    }

}

#endif // UNITY_EDITOR