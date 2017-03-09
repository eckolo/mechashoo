using UnityEngine;
using System.Collections;

public class Blast : Bullet
{
    /// <summary>
    /// 最大サイズ
    /// </summary>
    public float maxSize = 1;
    /// <summary>
    ///炸裂時SE
    /// </summary>
    public AudioClip explodeSE = null;

    protected override IEnumerator motion(int actionNum)
    {
        soundSE(explodeSE);

        for(int time = 0; time < destroyLimit; time++)
        {
            transform.localScale = Vector2.one * Easing.quintic.Out(maxSize, time, destroyLimit - 1);
            setAlpha(Easing.quadratic.SubIn(time, destroyLimit - 1));
            yield return wait(1);
        }

        selfDestroy();
        yield break;
    }
    public override float nowPower
    {
        get {
            return basePower * nowAlpha;
        }
    }

    protected override Vector2 impactDirection(Things target)
    {
        return target.position - position;
    }
}
