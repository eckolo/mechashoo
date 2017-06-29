using UnityEngine;
using System.Collections;

public class Effect : Materials
{
    /// <summary>
    /// エフェクトの基準サイズ
    /// </summary>
    public float baseScale = 1;
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

    public override void Start()
    {
        base.Start();

        transform.localScale = transform.localScale * baseScale;

        SoundSE(occurrenceSE, pitch: occurrenceSEPitch);
        Action();
    }
}
