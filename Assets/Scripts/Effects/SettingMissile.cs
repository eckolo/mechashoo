using UnityEngine;
using System.Collections;

public class SettingMissile : Effect
{
    /// <summary>
    /// 最終サイズ
    /// </summary>
    [SerializeField]
    private Vector2 maxSize = Vector2.one;
    /// <summary>
    /// 最大化までの時間
    /// </summary>
    [SerializeField]
    private int timeLimit = 72;
    /// <summary>
    /// 最大化から消滅までの猶予時間
    /// </summary>
    [SerializeField]
    public int delay = 0;

    protected override IEnumerator Motion(int actionNum)
    {
        Vector2 startPosition = position;
        transform.localScale = Vector2.zero;

        for(int time = 0; time < timeLimit; time++)
        {
            float scaleX = Easing.quadratic.Out(maxSize.x, time, timeLimit);
            float scaleY = Easing.quadratic.In(maxSize.y, time, timeLimit);
            transform.localScale = new Vector2(scaleX, scaleY);

            nowAlpha = Easing.quadratic.Out(time, timeLimit);

            yield return Wait(1);
        }
        if(delay > 0) yield return Wait(delay);

        DestroyMyself();
        yield break;
    }
}
