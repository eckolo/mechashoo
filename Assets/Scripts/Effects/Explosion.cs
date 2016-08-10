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
    protected int maxSize = 2;

    protected override IEnumerator Motion(int actionNum)
    {
        Vector3 baseScale = transform.localScale;
        for (int time = 0; time < destroyLimit; time++)
        {
            transform.localScale = baseScale * easing.exponential.Out(maxSize, time, destroyLimit - 1);

            setAlpha(getAlpha() * (easing.quadratic.SubOut(time, destroyLimit - 1)));

            yield return null;
        }
        Destroy(gameObject);
        yield break;
    }
}
