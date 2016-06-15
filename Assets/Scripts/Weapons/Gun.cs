using UnityEngine;
using System.Collections;

public class Gun : Weapon
{
    // 連射数
    public int fileNum;
    // 弾を撃つ間隔
    public int shotDelay;

    // 発射システム
    protected override IEnumerator Motion()
    {
        for (int i = 0; i < fileNum; i++)
        {
            injection(i);

            //反動発生
            startRecoil(new Vector2(0, 0.1f));

            // shotDelayフレーム待つ
            yield return StartCoroutine(wait(shotDelay));
        }
        yield break;
    }
}
