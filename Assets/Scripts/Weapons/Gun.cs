using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// 特に銃みたいなタイプの武装全般
/// </summary>
public class Gun : Weapon
{
    /// <summary>
    /// 弾を撃つ間隔
    /// </summary>
    [SerializeField]
    private int shotDelay;
    protected int shotDelayFinal => Mathf.CeilToInt(shotDelay * delayTweak);
    /// <summary>
    /// 基礎反動量
    /// </summary>
    [SerializeField]
    protected float recoilRate = 1;
    /// <summary>
    /// 弾ブレ度合い
    /// </summary>
    [SerializeField]
    protected int noAccuracy = 0;
    /// <summary>
    /// 連射数特殊指定
    /// </summary>
    [SerializeField]
    protected int burst = 1;
    protected int getBurst(Injection injection) => injection.burst > 0 ? injection.burst : burst;

    /// <summary>
    ///発射音
    /// </summary>
    public AudioClip shotSE = null;

    /// <summary>
    /// 発射システム
    /// </summary>
    protected override IEnumerator motion(int actionNum)
    {
        yield return charging();
        var nowInjections = onTypeInjections;
        if(!nowInjections.Any()) yield break;

        var maxBurst = nowInjections.Max(injection => getBurst(injection));
        for(int fire = 1; fire <= maxBurst; fire++)
        {
            foreach(var injection in nowInjections)
            {
                if(fire <= getBurst(injection)) inject(injection);
            }
            // shotDelayフレーム待つ
            yield return wait(shotDelayFinal);
        }
        yield break;
    }

    protected override Bullet inject(Injection injection, float fuelCorrection = 1, float angleCorrection = 0)
    {
        var shake = Mathf.Abs(Easing.quadratic.In(noAccuracy, 1, 0));
        var bullet = base.inject(injection, fuelCorrection, angleCorrection);
        if(bullet == null) return null;

        soundSE(shotSE, 0.8f);
        if(shake > 0) bullet.transform.rotation *= Quaternion.AngleAxis(Random.Range(-shake, shake), Vector3.forward);

        var rootShip = nowRoot.GetComponent<Ship>();
        var missile = bullet.GetComponent<Missile>();
        if(rootShip != null && missile != null) missile.target = rootShip.nowNearSiteTarget;

        //反動発生
        setRecoil(injection, recoilRate);
        return bullet;
    }

    /// <summary>
    /// 発射前のチャージモーション
    /// </summary>
    protected IEnumerator charging()
    {
        var effects = new List<Effect>();

        foreach(var injection in onTypeInjections)
        {
            var setEffect = injection.charge ?? chargeEffect;
            if(setEffect == null) continue;
            var effect = Instantiate(setEffect);
            effect.nowParent = transform;
            effect.position = injection.hole.scaling(lossyScale.abs());
            effect.setAngle(injection.angle);
            effects.Add(effect);
        }

        yield return wait(() => !effects.Any(effect => effect != null));

        yield break;
    }
}
