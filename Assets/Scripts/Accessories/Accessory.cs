using UnityEngine;
using System.Collections;

public class Accessory : Parts
{
    /// <summary>
    /// 発生エフェクトの基準サイズ
    /// </summary>
    [SerializeField]
    protected float baseEffectScale = 1;
    /// <summary>
    /// 基礎角度
    /// </summary>
    public float baseAngle { get; set; } = 0;

    public override void Start()
    {
        base.Start();
        Action();
    }
    /// <summary>
    /// 付属パーツ系の初期動作
    /// </summary>
    public virtual void AccessoryStartMotion() => AccessoryMotion(Vector2.zero);
    /// <summary>
    ///付属パーツ系の基本動作
    /// </summary>
    public virtual void AccessoryMotion(Vector2 setVector, float correctionAngle = 0)
    {
        return;
    }
}
