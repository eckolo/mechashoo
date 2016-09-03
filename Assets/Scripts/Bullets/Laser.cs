﻿using UnityEngine;
using System.Collections;

public class Laser : Bullet
{
    /// <summary>
    ///最大射程
    /// </summary>
    [SerializeField]
    private float maxReach;
    /// <summary>
    ///最大弾幅
    /// </summary>
    [SerializeField]
    private float maxWidth;
    /// <summary>
    ///消滅までの時間
    /// </summary>
    [SerializeField]
    private int timeLimit;

    protected override IEnumerator Motion(int actionNum)
    {
        Vector2 startPosition = transform.position;

        setVerosity(Vector2.zero, 0);
        transform.localScale = Vector2.zero;

        int halfLimit = timeLimit / 2;

        for (int time = 0; time < timeLimit; time++)
        {
            bool behind = time < halfLimit;
            int halfTime = behind ? time : time - halfLimit;

            float scaleX = easing.quadratic.Out(maxReach, time, timeLimit) / baseSize.x;
            float scareY = behind
                ? easing.quintic.Out(maxWidth, halfTime, halfLimit)
                : easing.quadratic.SubOut(maxWidth, halfTime, halfLimit);
            transform.localScale = new Vector2(scaleX, scareY);

            transform.position = startPosition
                + (Vector2)(transform.rotation * Vector2.right * transform.localScale.x * baseSize.x / 2);

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

    public override float nowPower
    {
        get
        {
            return base.nowPower * transform.localScale.y / maxWidth;
        }
    }
}
