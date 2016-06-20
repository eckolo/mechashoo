using UnityEngine;
using System.Collections;

public class Gun : Weapon
{
    /// <summary>
    /// 連射数
    /// </summary>
    public int fileNum;
    /// <summary>
    /// 弾を撃つ間隔
    /// </summary>
    public int shotDelay;

    /// <summary>
    /// 発射システム
    /// </summary>
    protected override IEnumerator Motion(int actionNum)
    {
        for (int i = 0; i < fileNum; i++)
        {
            injection(i).velocity = transform.rotation * Vector2.right * getLossyScale(transform).x;

            //反動発生
            startRecoil(new Vector2(0, 0.1f));

            // shotDelayフレーム待つ
            yield return StartCoroutine(wait(shotDelay));
        }
        yield break;
    }
}
