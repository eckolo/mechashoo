using UnityEngine;
using System.Collections;

public class Napalm : Blast
{
    /// <summary>
    /// 発生濃度
    /// </summary>
    [SerializeField]
    public int density = 1;
    /// <summary>
    /// 発生弾オブジェクト
    /// </summary>
    [SerializeField]
    protected Bullet injectBullet = null;
    /// <summary>
    /// 発生地点の最大距離
    /// </summary>
    [SerializeField]
    protected float maxRange = 1;
    /// <summary>
    /// 発生地点幅の最大値
    /// </summary>
    [SerializeField]
    protected float maxWidth = 1;
    /// <summary>
    /// 発生弾丸の初期角度補正
    /// </summary>
    [SerializeField]
    protected float injectAngle = 0;

    protected override IEnumerator Motion(int actionNum)
    {
        SoundSE(explodeSE);

        for(int time = 0; time < destroyLimit; time++)
        {
            var nowRange = Easing.liner.In(maxRange, time, destroyLimit - 1);
            var nowWidth = Easing.quintic.Out(maxWidth, time, destroyLimit - 1);

            for(int index = 0; index < density; index++)
            {
                var setAngle = injectAngle * (time % 2 == 0).ToSign() * (index % 2 == 0).ToSign();
                var incidencePoint = Vector2.right * nowRange + Vector2.up * nowWidth.ToMildRandom(centering: 2);
                var bullet = Inject(injectBullet, incidencePoint, setAngle);
                var slash = bullet.GetComponent<Slash>();
                if(slash != null) slash.SetParamate(initialScale.magnitude, 1);
            }

            yield return Wait(1);
        }

        DestroyMyself();
        yield break;
    }
    public override float nowPower => 0;
}
