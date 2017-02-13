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

    /// <summary>
    /// 発射システム
    /// </summary>
    protected override IEnumerator motion(ActionType actionType)
    {
        yield return charging(actionType);
        var onTypeInjections = getOnTypeInjections(actionType);
        if(!onTypeInjections.Any()) yield break;

        for(int fire = 0; fire < fireNum; fire++)
        {
            foreach(var injection in onTypeInjections)
            {
                var shake = Mathf.Abs(Easing.quadratic.In(noAccuracy, fire, fireNum - 1));
                var bullet = inject(injection, 1 / (float)fireNum);

                if(bullet != null)
                {
                    soundSE(shotSE, 0.5f, 1 + 0.1f * fireNum);
                    if(shake > 0) bullet.transform.rotation *= Quaternion.AngleAxis(Random.Range(-shake, shake), Vector3.forward);
                }
            }

            //反動発生
            // shotDelayフレーム待つ
            yield return startRecoil(onTypeInjections, recoilRate, shotDelay);
        }

        yield break;
    }

    /// <summary>
    /// 発射前のチャージモーション
    /// </summary>
    protected IEnumerator charging(ActionType actionType)
    {
        if(chargeEffect == null) yield break;
        var onTypeInjections = getOnTypeInjections(actionType);
        var effects = new List<Effect>();

        foreach(var injection in onTypeInjections)
        {
            var effect = Instantiate(injection.charge ?? chargeEffect);
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
    protected IEnumerator startRecoil(List<Injection> injections, float recoilRate, int returnTime)
    {
        int halfLimit = returnTime / 2;
        var setRecoil = Vector2.zero;
        foreach(var injection in injections)
        {
            var recoilPower = recoilRate * injection.initialVelocity;
            setRecoil += (injection.angle + 180).recalculation(recoilPower);
        }
        var ship = nowParent.GetComponent<Ship>();
        if(ship != null)
        {
            var direction = getWidthRealRotation(getLossyRotation() * (lossyScale.y.toSign() * injections.Sum(injection => injection.angle)).toRotation()) * Vector2.left;
            for(int time = 0; time < halfLimit; time++)
            {
                var power = Easing.quadratic.SubOut(setRecoil.magnitude, time, halfLimit);
                ship.exertPower(direction, power);
                yield return wait(1);
            }
            yield return wait(halfLimit);
        }
        else
        {
            setRecoil = setRecoil * setRecoil.magnitude / nowRoot.weight;
            for(int time = 0; time < halfLimit; time++)
            {
                nowRecoil = new Vector2(
                    Easing.quintic.Out(setRecoil.x, time, halfLimit - 1),
                    Easing.quintic.Out(setRecoil.y, time, halfLimit - 1)
                    );
                yield return wait(1);
            }
            for(int time = 0; time < halfLimit; time++)
            {
                nowRecoil = new Vector2(
                    Easing.quadratic.SubOut(setRecoil.x, time, halfLimit - 1),
                    Easing.quadratic.SubOut(setRecoil.y, time, halfLimit - 1)
                    );
                yield return wait(1);
            }
        }

        yield break;
    }
    Vector2 nowRecoil = Vector2.zero;
    public override Vector2 correctionVector
    {
        get {
            return base.correctionVector + nowRecoil;
        }

        set {
            base.correctionVector = value;
        }
    }
}
