using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    private int interval;

    // Update is called once per frame
    public override void Start()
    {
        base.Start();
        StartCoroutine(annimationSprite());
    }

    private IEnumerator annimationSprite()
    {
        int limit = spriteSet.Count * interval;

        for (int time = 0; time < limit; time++)
        {
            if (interval > 0 && time % interval == 0)
            {
                GetComponent<SpriteRenderer>().sprite = spriteSet[time / interval];
            }

            setAlpha(easing.quadratic.SubIn(time, limit));
            yield return null;
        }
        selfDestroy();
    }
}
