using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// じわっと消えるエフェクトクラス
/// 画像の定期切り替え機能付き
/// </summary>
public class Residuum : Effect
{
    /// <summary>
    /// 画像セット
    /// </summary>
    [SerializeField]
    private List<Sprite> spriteSet = new List<Sprite>();
    /// <summary>
    /// アニメーション頻度
    /// </summary>
    [SerializeField]
    private int interval = 12;

    protected override IEnumerator Motion(int actionNum)
    {
        int limit = Mathf.Max(spriteSet.Count, 1) * interval;

        for(int time = 0; time < limit; time++)
        {
            if(spriteSet.Count > 0 && interval > 0 && time % interval == 0)
            {
                GetComponent<SpriteRenderer>().sprite = spriteSet[time / interval];
            }

            nowAlpha = Easing.quadratic.SubIn(time, limit);
            yield return Wait(1);
        }
        DestroyMyself();
    }
}
