using UnityEngine;
using System.Collections;

public class Blast : Bullet {
    /// <summary>
    /// 最大サイズ
    /// </summary>
    public float maxSize = 1;

    protected override IEnumerator motion(int actionNum) {
        for(int time = 0; time < destroyLimit; time++) {
            transform.localScale = Vector2.one * easing.quintic.outer(maxSize, time, destroyLimit - 1);
            setAlpha(easing.quadratic.subInner(time, destroyLimit - 1));
            yield return wait(1);
        }

        selfDestroy();
        yield break;
    }
    public override float nowPower {
        get {
            return basePower * nowAlpha;
        }
    }
}
