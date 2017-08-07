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
    private int shotDelay = 1;
    protected int shotDelaySum => Mathf.CeilToInt(shotDelay * delayTweak * injectShotDelay);
    protected float injectShotDelay => onTypeInjections.Max(injection => injection.shotDelayTweak);
    /// <summary>
    /// 基礎反動量
    /// </summary>
    [SerializeField]
    protected float recoilRate = 1;
    /// <summary>
    /// 連射数特殊指定
    /// </summary>
    [SerializeField]
    protected int burst = 1;
    protected int GetBurst(Injection injection) => injection.burst > 0 ? injection.burst : burst;

    /// <summary>
    /// 発射音
    /// </summary>
    public AudioClip shotSE = null;

    protected override IEnumerator BeginMotion(int actionNum)
    {
        if(!onTypeInjections.Any()) yield break;
        yield return Charging();
        yield break;
    }

    /// <summary>
    /// 発射システム
    /// </summary>
    protected override IEnumerator Motion(int actionNum)
    {
        if(!onTypeInjections.Any()) yield break;

        var maxBurst = onTypeInjections.Max(injection => GetBurst(injection));
        for(int fire = 1; fire <= maxBurst; fire++)
        {
            List<Bullet> unionBullets = new List<Bullet>();
            foreach(var injection in onTypeInjections)
            {
                if(fire <= GetBurst(injection))
                {
                    var injectedBullet = Inject(injection);
                    if(injection.union) unionBullets.Add(injectedBullet);
                }
            }
            // shotDelayフレーム待つ
            yield return Wait(shotDelaySum);
            yield return Wait(() => !unionBullets.Any(bullet => bullet != null));
        }
        yield break;
    }

    protected override Bullet Inject(Injection injection, float fuelCorrection = 1, float angleCorrection = 0)
    {
        var bullet = base.Inject(injection, fuelCorrection, angleCorrection);
        if(bullet == null) return null;

        SoundSE(shotSE, 0.8f);

        var rootShip = nowRoot?.GetComponent<Ship>();
        var missile = bullet.GetComponent<Missile>();
        if(rootShip != null && missile != null) missile.target = rootShip.nowNearSiteTarget;

        //反動発生
        SetRecoil(injection, recoilRate);
        return bullet;
    }

    /// <summary>
    /// 発射前のチャージモーション
    /// </summary>
    protected IEnumerator Charging()
    {
        var effects = new List<Effect>();

        foreach(var injection in onTypeInjections)
        {
            var setEffect = injection.charge ?? chargeEffect;
            if(setEffect == null) continue;
            var effect = Instantiate(setEffect);
            effect.nowParent = transform;
            effect.position = injection.hole.Scaling(lossyScale.Abs());
            effect.SetAngle(injection.angle + injection.bulletAngle);
            effects.Add(effect);
        }

        yield return Wait(() => !effects.Any(effect => effect != null));

        yield break;
    }
}
