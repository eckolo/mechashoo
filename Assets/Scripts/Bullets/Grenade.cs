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

    public override void selfDestroy(bool system = false)
    {
        if (!system) injection(blast, Vector2.zero);
        base.selfDestroy();
    }
}
