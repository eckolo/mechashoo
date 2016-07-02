using UnityEngine;
using System.Collections;

/// <summary>
/// 特に銃みたいなタイプの武装全般
/// </summary>
public class Gun : Weapon
{
    /// <summary>
    /// 連射数
    /// </summary>
    [SerializeField]
    protected int fireNum;
    /// <summary>
    /// 弾を撃つ間隔
    /// </summary>
    [SerializeField]
    protected int shotDelay;
    /// <summary>
    /// 基礎反動量
    /// </summary>
    [SerializeField]
    protected Vector2 baseRecoil = new Vector2(0, 0);

    /// <summary>
    /// 発射システム
    /// </summary>
    protected override IEnumerator Motion(int actionNum)
    {
        for (int i = 0; i < fireNum; i++)
        {
            injection(i).velocity = transform.rotation * Vector2.right * getLossyScale(transform).x;

            //反動発生
            // shotDelayフレーム待つ
            yield return startRecoil(baseRecoil, shotDelay);
        }
        yield break;
    }

    /// <summary>
    ///反動関数
    /// </summary>
    protected IEnumerator startRecoil(Vector2 setRecoil, int returnTime)
    {
        var baseVector = correctionVector;
        for (int time = 0; time < returnTime; time++)
        {
            var nowRecoil = setRecoil - new Vector2(
                easing.quadratic.Out(setRecoil.x, time, returnTime - 1),
                easing.quadratic.Out(setRecoil.y, time, returnTime - 1)
                );
            correctionVector = baseVector + nowRecoil;
            yield return null;
        }

        yield break;
    }
}
