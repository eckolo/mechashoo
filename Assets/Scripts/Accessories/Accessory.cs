using UnityEngine;
using System.Collections;

public class Accessory : Parts
{
    /// <summary>
    /// 発生エフェクトの基準サイズ
    /// </summary>
    [SerializeField]
    protected float baseEffectScale = 1;

    protected override void baseStart()
    {
        base.baseStart();
        Action();
    }
    /// <summary>
    ///付属パーツ系の基本動作
    /// </summary>
    public virtual void accessoryMotion(Vector2 setVector, float correctionAngle = 0)
    {
        return;
    }
}
