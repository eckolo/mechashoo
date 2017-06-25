using UnityEngine;
using System.Collections;
using System.Linq;

public class Luwucijiqi : Npc
{
    /// <summary>
    /// 画面内に位置強制するフラグ
    /// </summary>
    protected override bool forcedInScreen
    {
        get {
            if(onTheWay && normalCourse.normalized == nowSpeed.normalized) return false;
            return base.forcedInScreen;
        }
    }

    /// <summary>
    /// 非反応時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionNonCombat(int actionNum)
    {
        yield return NomalMoving((maximumSpeed + lowerSpeed) / 2);
        yield break;
    }
    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        var wraps = Random.Range(2, 3 + (int)shipLevel) + onTheWay.ToInt();
        var baseSpeed = normalCourse;
        var initialDirection = new[] { 1, -1 }.SelectRandom();
        for(int wrap = 0; wrap < wraps && inField; wrap++)
        {
            var nowAngle = (baseSpeed.magnitude > 0 ? baseSpeed : siteAlignment).ToAngle();
            var correctAngle = !onTheWay
                ? Random.Range(90f, 270f)
                : Random.Range(45f, 75f) * initialDirection * (wrap % 2 == 0).ToSign();
            var direction = (nowAngle + correctAngle).Compile();
            yield return AimingAction(nearTarget.position,
                !onTheWay ? interval : interval * 2,
                aimingProcess: () => Thrust(direction.ToVector(), reactPower, maximumSpeed));
            if(!onTheWay) baseSpeed = nowSpeed;
            if(onTheWay) yield return NomalAttack();
        }
        normalCourse = baseSpeed;
        yield break;
    }
    /// <summary>
    /// 照準操作行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionAiming(int actionNum)
    {
        nextActionState = ActionPattern.ATTACK;
        nextActionIndex = (nearTarget.position.magnitude < arms[1].tipReach).ToInt();
        yield return AimingAction(() => nearTarget.position,
            interval,
            aimingProcess: () => Thrust(!onTheWay ? nearTarget.position - position : normalCourse, reactPower, lowerSpeed));
        yield break;
    }
    /// <summary>
    /// 攻撃行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionAttack(int actionNum)
    {
        nextActionState = !onTheWay ? ActionPattern.MOVE : ActionPattern.NON_COMBAT;
        yield return NomalAttack();
        yield break;
    }
    /// <summary>
    /// 逃走時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionEscape(int actionNum)
    {
        yield return NomalMoving(maximumSpeed);
        yield break;
    }

    IEnumerator NomalMoving(float speed)
    {
        var direction = Random.Range(-10f, 10f).ToRotation() * normalCourse;
        for(int time = 0; time < interval; time++)
        {
            Thrust(direction, reactPower, speed);
            Aiming(position + baseAimPosition);
            yield return Wait(1);
        }
        yield return StoppingAction();
        yield break;
    }
    IEnumerator NomalAttack()
    {
        if(!inField) yield break;
        int armNum = (siteAlignment.magnitude > arms[1].tipReach).ToInt();
        var fixedAlignmentPosition = armNum == 0
            ? siteAlignment.ToVector(arms[1].tipReach)
            : siteAlignment;
        SetFixedAlignment(position + fixedAlignmentPosition);
        arms[armNum].tipHand.ActionWeapon(Weapon.ActionType.NOMAL);
        yield return StoppingAction();
        yield return Wait(() => !allWeapons.Any(weapon => !weapon.canAction));
    }
}
