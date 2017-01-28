using UnityEngine;
using System.Collections;

public class Locus : Effect
{
    /// <summary>
    /// 消滅までのフレーム数
    /// </summary>
    [SerializeField]
    protected int destroyLimit = 120;
    /// <summary>
    ///炸裂時SE
    /// </summary>
    public AudioClip explodeSE = null;

    protected override IEnumerator motion(int actionNum)
    {
        Vector3 initialScale = transform.localScale;
        soundSE(explodeSE);

        for(int time = 0; time < destroyLimit; time++)
        {
            transform.localScale = initialScale * easing.quadratic.SubOut(time, destroyLimit - 1);

            setAlpha(nowAlpha * (easing.cubic.SubIn(time, destroyLimit - 1)));

            yield return wait(1);
        }
        Destroy(gameObject);
        yield break;
    }
}
