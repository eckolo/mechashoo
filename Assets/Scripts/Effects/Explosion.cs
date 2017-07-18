using UnityEngine;
using System.Collections;

/// <summary>
/// 爆破エフェクトクラス
/// </summary>
public class Explosion : Effect
{
    /// <summary>
    /// 消滅までのフレーム数
    /// </summary>
    [SerializeField]
    protected int destroyLimit = 120;
    /// <summary>
    /// 最大サイズ
    /// </summary>
    [SerializeField]
    protected float maxSize = 2;
    /// <summary>
    /// 炸裂時SE
    /// </summary>
    [SerializeField]
    public AudioClip explodeSE = null;
    /// <summary>
    /// 事前エフェクト
    /// </summary>
    [SerializeField]
    protected Effect accumulateEffect = null;

    protected override IEnumerator Motion(int actionNum)
    {
        Vector3 baseScale = transform.localScale;
        var subEffect = accumulateEffect != null ? OutbreakEffect(accumulateEffect, baseScale) : null;
        if(subEffect != null)
        {
            subEffect.nowParent = transform;
            subEffect.position = Vector2.zero;
        }
        nowAlpha = 0;
        yield return Wait(() => subEffect == null);

        SoundSE(explodeSE);

        for(int time = 0; time < destroyLimit; time++)
        {
            transform.localScale = baseScale * Easing.exponential.Out(maxSize, time, destroyLimit - 1);

            nowAlpha = Easing.quadratic.SubIn(time, destroyLimit - 1);

            yield return Wait(1);
        }
        Destroy(gameObject);
        yield break;
    }
}
