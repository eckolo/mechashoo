using UnityEngine;
using System.Collections;

public class Laser : Bullet
{
    /// <summary>
    ///最大射程
    /// </summary>
    [SerializeField]
    private float maxReach = 12f;
    /// <summary>
    ///最大弾幅
    /// </summary>
    [SerializeField]
    private float maxWidth = 1f;
    /// <summary>
    ///消滅までの時間
    /// </summary>
    [SerializeField]
    private int timeLimit = 72;

    public override void Start()
    {
        setVerosity(Vector2.zero, 0);
        transform.localScale = Vector2.zero;
        base.Start();
    }

    protected override IEnumerator motion(int actionNum)
    {
        Vector2 startPosition = position;
        if(nowParent != null && nowParent.GetComponent<Weapon>() != null) setAngle(0);

        int halfLimit = timeLimit / 2;

        for(int time = 0; time < timeLimit; time++)
        {
            bool behind = time < halfLimit;
            int halfTime = behind ? time : time - halfLimit;

            float scaleX = Easing.quadratic.Out(maxReach, time, timeLimit) / baseSize.x;
            float scaleY = behind
                ? Easing.quintic.Out(maxWidth, halfTime, halfLimit)
                : Easing.quadratic.SubOut(maxWidth, halfTime, halfLimit);
            transform.localScale = new Vector2(scaleX, scaleY);

            position = startPosition + (Vector2)(transform.localRotation * Vector2.right * transform.localScale.x * baseSize.x / 2);

            float alpha = behind
                ? Easing.quadratic.Out(halfTime, halfLimit)
                : Easing.quadratic.SubOut(halfTime, halfLimit);
            setAlpha(alpha);

            yield return wait(1);
        }

        selfDestroy();
        yield break;
    }

    protected override Vector2 getHitPosition(Things target)
    {
        var degree = target.globalPosition - globalPosition;
        float angle = Quaternion.FromToRotation(transform.rotation * Vector2.right, degree).eulerAngles.z * Mathf.Deg2Rad;

        return transform.rotation * Vector2.right * degree.magnitude * Mathf.Cos(angle);
    }

    public override float nowPower
    {
        get {
            return base.nowPower * transform.localScale.y / maxWidth;
        }
    }
}
