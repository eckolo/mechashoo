using UnityEngine;
using System.Collections;

/// <summary>
/// 連鎖弾オブジェクト
/// </summary>
public class ChainBullet : Grenade
{
    /// <summary>
    /// 射出周りの情報
    /// </summary>
    [SerializeField]
    protected Weapon.Injection injection;
    /// <summary>
    /// 射出間隔
    /// </summary>
    [SerializeField]
    protected int interval = 1;
    /// <summary>
    /// 最大射出回数
    /// </summary>
    [SerializeField]
    protected int injectLimit = 1;

    /// <summary>
    /// 射出回数
    /// </summary>
    int injectCount = 0;

    protected override void MotionProcess(int time)
    {
        if(injectCount >= injectLimit) return;
        if(time % interval < interval - 1) return;
        Inject(injection);
    }

    /// <summary>
    /// 射出関数
    /// </summary>
    /// <param name="injection">射出関連パラメータ</param>
    /// <returns>射出した弾丸</returns>
    protected virtual Bullet Inject(Weapon.Injection injection)
    {
        if(injection == null) return null;

        var confirmBullet = injection.bullet;
        if(confirmBullet == null) return null;
        var injectSign = (injectCount % 2 == 0).ToSign();
        var forwardAngle = injection.angle * injectSign;

        SoundSE(injection.se);
        var bullet = Inject(confirmBullet, injection.hole, forwardAngle + injection.bulletAngle * injectSign);
        if(bullet == null) return bullet;

        injectCount++;
        bullet.user = user;
        bullet.userWeapon = userWeapon;
        bullet.SetVerosity(forwardAngle.ToRotation() * nowSpeed.normalized * injection.initialVelocity);
        if(injection.union) bullet.nowParent = transform;

        return bullet;
    }
}
