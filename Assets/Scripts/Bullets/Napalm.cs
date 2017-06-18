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
    protected Blast blast = null;
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

    protected override IEnumerator motion(int actionNum)
    {
        soundSE(explodeSE);

        for(int time = 0; time < destroyLimit; time++)
        {
            var nowRange = Easing.quadratic.In(maxRange, time, destroyLimit - 1);
            var nowWidth = Easing.quadratic.Out(maxWidth, time, destroyLimit - 1);

            for(int index = 0; index < density; index++)
            {
                var incidencePoint = Vector2.right * nowRange
                    + Vector2.up * nowWidth.toMildRandom();
                inject(blast, incidencePoint);
            }

            yield return wait(1);
        }

        selfDestroy();
        yield break;
    }
    public override float nowPower => 0;
}
