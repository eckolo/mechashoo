using UnityEngine;
using System.Collections;

public class Laser : Bullet
{
    /// <summary>
    /// 最大射程
    /// </summary>
    [SerializeField]
    private float maxReach = 12f;
    /// <summary>
    /// 最大弾幅
    /// </summary>
    [SerializeField]
    private float maxWidth = 1f;
    /// <summary>
    /// 消滅までの時間
    /// </summary>
    [SerializeField]
    private int timeLimit = 72;
    /// <summary>
    /// モーション前半の全体時間に占める割合
    /// </summary>
    [SerializeField]
    private float firstHalf = 0.5f;

    /// <summary>
    /// 初期位置記憶
    /// </summary>
    Vector2 startPosition = Vector2.zero;

    public override void Start()
    {
        SetVerosity(Vector2.zero, 0);
        transform.localScale = Vector2.zero;
        base.Start();
    }

    protected override IEnumerator Motion(int actionNum)
    {
        startPosition = position;
        if(nowParent != null && nowParent.GetComponent<Weapon>() != null) SetAngle(0);

        var firstHalfLimit = Mathf.FloorToInt(timeLimit * firstHalf);
        var secondHalfLimit = timeLimit - firstHalfLimit;

        for(var time = 0; time < timeLimit; time++)
        {
            var behind = time < firstHalfLimit;
            var halfTime = behind ? time : time - firstHalfLimit;

            var scaleX = behind ?
                Easing.quadratic.Out(maxReach, halfTime, firstHalfLimit * 2) / spriteSize.x :
                Easing.quadratic.Out(maxReach, halfTime + secondHalfLimit, secondHalfLimit * 2) / spriteSize.x;
            var scaleY = behind ?
                Easing.quintic.Out(maxWidth, halfTime, firstHalfLimit) :
                Easing.quadratic.SubOut(maxWidth, halfTime, secondHalfLimit);
            transform.localScale = new Vector2(scaleX, scaleY);

            position = startPosition + (Vector2)(transform.localRotation * Vector2.right * transform.localScale.x * spriteSize.x / 2);

            var alpha = behind
                ? Easing.quadratic.Out(halfTime, firstHalfLimit)
                : Easing.quadratic.SubOut(halfTime, secondHalfLimit);
            nowAlpha = alpha;

            yield return Wait(1);
        }

        DestroyMyself();
        yield break;
    }

    protected override Vector2 GetHitPosition(Things target)
    {
        var degree = target.globalPosition - globalPosition;
        float angle = Quaternion.FromToRotation(transform.rotation * Vector2.right, degree).eulerAngles.z * Mathf.Deg2Rad;

        return transform.rotation * Vector2.right * degree.magnitude * Mathf.Cos(angle);
    }

    public override Vector2 nowSpeed
    {
        get {
            return Vector2.zero;
        }
    }

    public override float nowPower
    {
        get {
            return base.nowPower * transform.localScale.y / maxWidth / (((float)hitCount).Log(2) + 1);
        }
    }

    protected override Vector2 ImpactDirection(Things target)
    {
        return target.position - startPosition;
    }

    uint hitCount = 0;
    protected override bool ContactBullet(Bullet target)
    {
        var contact = base.ContactBullet(target);
        if(contact) hitCount++;
        return contact;
    }
    protected override bool ContactShip(Ship target, bool first)
    {
        var contact = base.ContactShip(target, first);
        if(contact) hitCount++;
        return contact;
    }
}
