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
    protected Vector2 baseRecoil = Vector2.zero;
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
    protected override IEnumerator motion(int actionNum)
    {
        for(int fire = 0; fire < fireNum; fire++)
        {
            var shake = Mathf.Abs(easing.quadratic.In(noAccuracy, fire, fireNum - 1));
            var bullet = inject(injections[fire % injections.Count], 1 / (float)fireNum);

            if(bullet != null)
            {
                soundSE(shotSE, 0.5f, 1 + 0.1f * fireNum);
                if(shake > 0) bullet.transform.rotation *= Quaternion.AngleAxis(Random.Range(-shake, shake), Vector3.forward);
                bullet.initialVelocity = bullet.transform.rotation * Vector2.right;
            }

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
        for(int time = 0; time < returnTime; time++)
        {
            var nowRecoil = setRecoil - new Vector2(
                easing.quadratic.Out(setRecoil.x, time, returnTime - 1),
                easing.quadratic.Out(setRecoil.y, time, returnTime - 1)
                );
            correctionVector = baseVector + nowRecoil;
            yield return wait(1);
        }

        yield break;
    }
}
