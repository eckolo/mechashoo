using UnityEngine;
using System.Collections;

/// <summary>
/// 炸裂弾クラス
/// </summary>
public class Grenade : Shell
{
    /// <summary>
    /// 炸裂弾オブジェクト
    /// </summary>
    [SerializeField]
    protected Blast blast = null;

    /// <summary>
    /// 自身の削除実行関数
    /// </summary>
    protected override void executeDestroy()
    {
        inject(blast, Vector2.zero);
        base.executeDestroy();
    }
}
