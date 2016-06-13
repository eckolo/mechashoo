using UnityEngine;
using System.Collections;

public class Gun : Weapon
{
    // 連射数
    public int fileNum;
    // 弾を撃つ間隔
    public float shotDelay;

    public override bool Action()
    {
        if (!canAction) return false;
        if (!canStartAction) return false;
        canStartAction = false;
        StartCoroutine(basicMotion(fileNum));
        return true;
    }

    // 発射システム
    protected IEnumerator basicMotion(int burstNum = 1)
    {
        for (int i = 0; i < burstNum; i++)
        {
            injection(i);

            //反動発生
            startRecoil(new Vector2(0, 0.1f));

            // shotDelay秒待つ
            yield return new WaitForSeconds(shotDelay);
        }

        // actionDelay秒待つ
        yield return new WaitForSeconds(actionDelay);
        canStartAction = true;
    }
}
