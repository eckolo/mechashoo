using UnityEngine;
using System.Collections;

public class Effect : Materials
{
    /// <summary>
    /// エフェクトの基準サイズ
    /// </summary>
    public float baseScale = 1;
    /// <summary>
    /// 角度固定
    /// </summary>
    [SerializeField]
    protected bool isForcedAngle = false;
    /// <summary>
    /// 発生時の効果音
    /// </summary>
    [SerializeField]
    protected AudioClip occurrenceSE = null;
    /// <summary>
    /// 発生時の効果音のピッチ
    /// </summary>
    [SerializeField]
    protected float occurrenceSEPitch = 1;

    float forcedAngle = 0;

    public override void Start()
    {
        base.Start();

        forcedAngle = nowGlobalAngle;
        transform.localScale = transform.localScale * baseScale;

        SoundSE(occurrenceSE, pitch: occurrenceSEPitch);
        Action();
    }

    public override void Update()
    {
        base.Update();
        if(isForcedAngle) nowGlobalAngle = forcedAngle;
    }
}
