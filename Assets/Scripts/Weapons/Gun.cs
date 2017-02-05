using UnityEngine;
using System.Collections;
using System.Linq;

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
    protected Vector2 baseRecoil = Vector2.zero;
    /// <summary>
    /// 弾ブレ度合い
    /// </summary>
    [SerializeField]
    protected int noAccuracy = 0;

    /// <summary>
    /// チャージエフェクト
    /// </summary>
    [SerializeField]
    protected Effect chargeEffect = null;

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

        for(int fire = 0; fire < fireNum; fire++)
        {
            var injection = injections[fire % injections.Count];
            var shake = Mathf.Abs(easing.quadratic.In(noAccuracy, fire, fireNum - 1));
            var bullet = inject(injection, 1 / (float)fireNum);

            if(bullet != null)
            {
                soundSE(shotSE, 0.5f, 1 + 0.1f * fireNum);
                if(shake > 0) bullet.transform.rotation *= Quaternion.AngleAxis(Random.Range(-shake, shake), Vector3.forward);
            }

            //反動発生
            // shotDelayフレーム待つ
            yield return startRecoil(baseRecoil, shotDelay, injection);
        }
        yield break;
    }

    /// <summary>
    /// 発射前のチャージモーション
    /// </summary>
    protected IEnumerator charging()
    {
        if(chargeEffect == null) yield break;

        var effect = Instantiate(chargeEffect);
        effect.nowParent = transform;
        effect.position = injections.First().hole;
        effect.setAngle(0);

        while(effect != null) yield return wait(1);

        yield break;
    }

    /// <summary>
    ///反動関数
    /// </summary>
    protected IEnumerator startRecoil(Vector2 setRecoil, int returnTime, Injection injection)
    {
        int halfLimit = returnTime / 2;
        var ship = nowParent.GetComponent<Ship>();
        if(ship != null)
        {
            var direction = getWidthRealRotation(getLossyRotation() * MathA.toRotation(toSign(lossyScale.y) * injection.angle)) * Vector2.left;
            for(int time = 0; time < halfLimit; time++)
            {
                var power = easing.quadratic.SubOut(setRecoil.magnitude * 10, time, halfLimit);
                ship.exertPower(direction, power);
                yield return wait(1);
            }
            yield return wait(halfLimit);
        }
        else
        {
            for(int time = 0; time < halfLimit; time++)
            {
                nowRecoil = new Vector2(
                    easing.quintic.Out(setRecoil.x, time, halfLimit - 1),
                    easing.quintic.Out(setRecoil.y, time, halfLimit - 1)
                    );
                yield return wait(1);
            }
            for(int time = 0; time < halfLimit; time++)
            {
                nowRecoil = new Vector2(
                    easing.quadratic.SubOut(setRecoil.x, time, halfLimit - 1),
                    easing.quadratic.SubOut(setRecoil.y, time, halfLimit - 1)
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
