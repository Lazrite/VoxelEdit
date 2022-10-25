using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// JSONに出力する際に送る情報
/// </summary>
public class JsonConvertPropaties : MonoBehaviour
{
    [SerializeField] public bool isSpawnPoint;      // エンティティのスポーンポイントかどうか
    [SerializeField] public bool isRandomize;       // ランダマイザ(回転をランダムにかけるか？)
    [SerializeField] public bool isCollision;       // 当たり判定が存在するか？
    [SerializeField] public bool isMonoSpaced;      // 等幅(縦、横、奥行の全てが全て同じ大きさか)
    [SerializeField] public bool isintrustUserMesh; // ユーザー定義メッシュモデル描画を任せるか
    [SerializeField] public int ID;                 // モデルID(コンバート先で)
}
