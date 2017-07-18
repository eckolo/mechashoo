using UnityEngine;
using System.Collections;

public class Luwuxoji : Npc
{
    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        var baseDegree = normalCourse;
        yield return AimingAction(() => nearTarget.position, interval * 2, aimingProcess: () => {
            var digree = GetProperPosition(nearTarget);
            var speed = baseDegree.ToVector(digree) + digree;
            Thrust(speed, reactPower, maximumSpeed);
        });
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
        yield return AimingAction(() => nearTarget.position, () => nowSpeed.magnitude > 0, aimingProcess: () => ThrustStop());
        yield break;
    }
    /// <summary>
    /// 攻撃行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionAttack(int actionNum)
    {
        nextActionState = ActionPattern.MOVE;

        int armNum = (siteAlignment.magnitude < arms[1].tipReach).ToInt();
        arms[armNum].tipHand.ActionWeapon(Weapon.ActionType.NOMAL);

        if(onTheWay && actionCount++ >= shipLevel) nextActionState = ActionPattern.ESCAPE;

        yield return Wait(interval);
        yield break;
    }
    int actionCount = 0;
}
