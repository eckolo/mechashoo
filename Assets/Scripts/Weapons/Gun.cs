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
    protected int shotDelay;
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
    ///発射音
    /// </summary>
    public AudioClip shotSE = null;

    protected override void updateMotion()
    {
        base.updateMotion();
        updateRecoil();
    }

    /// <summary>
    /// 発射システム
    /// </summary>
    protected override IEnumerator motion(int actionNum)
    {
        yield return charging();
        if(!onTypeInjections.Any()) yield break;

        foreach(var injection in onTypeInjections)
        {
            var shake = Mathf.Abs(Easing.quadratic.In(noAccuracy, 1, 0));
            var bullet = inject(injection);

            if(bullet != null)
            {
                soundSE(shotSE, 0.8f);
                if(shake > 0) bullet.transform.rotation *= Quaternion.AngleAxis(Random.Range(-shake, shake), Vector3.forward);

                //反動発生
                setRecoil(injection, recoilRate);
            }
        }

        // shotDelayフレーム待つ
        yield return wait(shotDelay);

        yield break;
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

    /// <summary>
    ///反動関数
    /// </summary>
    protected void setRecoil(Injection injection, float recoilRate = 1)
    {
        var injectBullet = getBullet(injection);
        var recoilPower = recoilRate * injection.initialVelocity;
        var setedRecoil = (injection.angle + 180).recalculation(recoilPower) * injectBullet.weight;

        var ship = nowParent.GetComponent<Ship>();
        if(ship != null)
        {
            var direction = getWidthRealRotation(getLossyRotation() * (lossyScale.y.toSign() * injection.angle).toRotation()) * Vector2.left;
            ship.exertPower(direction, setedRecoil.scaling(baseMas).magnitude);
        }
        else
        {
            var recoil = Vector2.right * Mathf.Log(Mathf.Abs(setedRecoil.x) + 1) * setedRecoil.x.toSign() + Vector2.up * Mathf.Log(Mathf.Abs(setedRecoil.y) + 1) * setedRecoil.y.toSign();
            setRecoil(recoil);
        }
    }
    protected void setRecoil(Vector2 hangForce)
    {
        Vector2 degree = hangForce - recoilSpeed;
        var length = degree.magnitude;
        float variation = length != 0 ? Mathf.Clamp(hangForce.magnitude / length, -1, 1) : 0;

        recoilSpeed = recoilSpeed + degree * variation;
    }
    void updateRecoil()
    {
        nowRecoil += recoilSpeed;
        if(nowRecoil.magnitude == 0) recoilSpeed = Vector2.zero;
        else if(nowRecoil.magnitude < tokenHand.power) recoilSpeed = -nowRecoil;
        else setRecoil(-nowRecoil.recalculation(tokenHand.power));
    }
    Vector2 nowRecoil = Vector2.zero;
    Vector2 recoilSpeed = Vector2.zero;
    public override Vector2 correctionVector
    {
        get {
            return base.correctionVector + nowRecoil.rescaling(baseMas);
        }

        set {
            base.correctionVector = value;
        }
    }
}
