using UnityEngine;
using System.Collections;

public class Charging : Effect
{
    /// <summary>
    /// 初期サイズ
    /// </summary>
    [SerializeField]
    private Vector2 initialScale = new Vector2(2, 2);
    /// <summary>
    /// 最大長倍率
    /// </summary>
    [SerializeField]
    private float maxLengthRate = 4;
    /// <summary>
    ///消滅までの時間
    /// </summary>
    [SerializeField]
    private int timeLimit = 72;

    protected override IEnumerator motion(int actionNum)
    {
        Vector2 startPosition = position;

        transform.localScale = initialScale;

        int halfLimit = timeLimit / 2;

        for(int time = 0; time < timeLimit; time++)
        {
            bool behind = time < halfLimit;
            int halfTime = behind ? time : time - halfLimit;

            float scaleX = behind
                ? initialScale.x + easing.quintic.Out(maxLengthRate - initialScale.x, halfTime, halfLimit)
                : easing.quadratic.SubIn(maxLengthRate, halfTime, halfLimit);
            float scaleY = easing.quadratic.SubInOut(initialScale.y, time, timeLimit);
            transform.localScale = new Vector2(scaleX, scaleY);

            position = startPosition + Vector2.right * transform.localScale.x * baseSize.x / 2;

            setAlpha(easing.quadratic.In(time, timeLimit));

            yield return wait(1);
        }

        selfDestroy();
        yield break;
    }
}
