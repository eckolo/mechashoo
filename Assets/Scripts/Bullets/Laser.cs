using UnityEngine;
using System.Collections;

public class Laser : Bullet
{
    /// <summary>
    ///画像の横ピクセルサイズ
    /// </summary>
    [SerializeField]
    private int pixelSize;

    /// <summary>
    ///最大射程
    /// </summary>
    [SerializeField]
    private int maxReach;
    /// <summary>
    ///最大弾幅
    /// </summary>
    [SerializeField]
    private int maxWidth;
    /// <summary>
    ///消滅までの時間
    /// </summary>
    [SerializeField]
    private int timeLimit;

    protected override IEnumerator Motion(int actionNum)
    {
        Vector2 startPosition = transform.position;
        Vector2 defaultScale = transform.localScale.normalized;

        setVerosity(Vector2.zero, 0);
        transform.localScale = Vector2.zero;

        int halfLimit = timeLimit / 2;

        for (int time = 0; time < timeLimit; time++)
        {
            bool behind = time < halfLimit;
            int halfTime = behind ? time : time - halfLimit;

            float scaleX = easing.quadratic.Out(maxReach, time, timeLimit);
            float scareY = behind
                ? easing.quadratic.Out(maxWidth, halfTime, halfLimit)
                : easing.quadratic.SubOut(maxWidth, halfTime, halfLimit);
            transform.localScale = new Vector2(scaleX * defaultScale.x, scareY * defaultScale.y) / pixelSize;

            transform.position = startPosition
                + (Vector2)(transform.rotation * Vector2.right * transform.localScale.x * pixelSize / getPixel() / 2);

            float alpha = behind
                ? easing.quadratic.Out(halfTime, halfLimit)
                : easing.quadratic.SubOut(halfTime, halfLimit);
            setAlpha(alpha);

            yield return null;
        }

        selfDestroy();
        yield break;
    }

    protected override Vector2 getHitPosition(Things target)
    {
        var degree = target.transform.position - transform.position;
        float angle = Quaternion.FromToRotation(transform.rotation * Vector2.right, degree).eulerAngles.z * Mathf.Deg2Rad;

        return transform.rotation * Vector2.right * degree.magnitude * Mathf.Cos(angle);
    }
}
