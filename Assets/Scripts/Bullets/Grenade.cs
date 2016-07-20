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

    protected override void selfDestroyAction()
    {
        injection(blast, Vector2.zero);
        base.selfDestroyAction();
    }
}
