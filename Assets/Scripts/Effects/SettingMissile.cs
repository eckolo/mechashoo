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
    /// 消滅までの時間
    /// </summary>
    [SerializeField]
    private int timeLimit = 72;

    protected override IEnumerator motion(int actionNum)
    {
        Vector2 startPosition = position;
        transform.localScale = Vector2.zero;

        for(int time = 0; time < timeLimit; time++)
        {
            float scaleX = Easing.quadratic.Out(maxSize.x, time, timeLimit);
            float scaleY = Easing.quadratic.In(maxSize.y, time, timeLimit);
            transform.localScale = new Vector2(scaleX, scaleY);

            nowAlpha = Easing.quadratic.Out(time, timeLimit);

            yield return wait(1);
        }

        selfDestroy();
        yield break;
    }
}
