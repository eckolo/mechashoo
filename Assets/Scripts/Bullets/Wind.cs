using UnityEngine;
using System.Collections;

public class Wind : Bullet
{
    /// <summary>
    /// 最初期サイズ
    /// </summary>
    [SerializeField]
    protected float startSize = 0;
    /// <summary>
    /// 終了サイズ
    /// </summary>
    [SerializeField]
    protected float endSize = 1;
    /// <summary>
    /// 炸裂時SE
    /// </summary>
    [SerializeField]
    protected AudioClip explodeSE = null;
    /// <summary>
    /// 逆転動作フラグ
    /// </summary>
    [SerializeField]
    protected bool reverse = false;

    protected override IEnumerator Motion(int actionNum)
    {
        SoundSE(explodeSE);

        for(int time = 0; time < destroyLimit; time++)
        {
            var nowSize = !reverse ?
                startSize + Easing.quintic.Out(endSize - startSize, time, destroyLimit - 1) :
                startSize + Easing.quadratic.Out(endSize - startSize, time, destroyLimit - 1);
            transform.localScale = Vector2.one * nowSize;
            nowAlpha = Easing.quadratic.SubIn(time, destroyLimit - 1);
            yield return Wait(1);
        }

        DestroyMyself();
        yield break;
    }
    public override float nowPower
    {
        get {
            return basePower * nowAlpha;
        }
    }

    protected override Vector2 ImpactDirection(Things target)
    {
        return ((target.position - position) * (!reverse).ToSign()).ToVector(nowSpeed) + nowSpeed;
    }
}
