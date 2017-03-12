using UnityEngine;
using System.Collections;

public class Luwuxoji : Npc
{
    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator motionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        var baseDegree = normalCourse;
        yield return aimingAction(() => nearTarget.position, interval * 2, () => {
            var digree = getProperPosition(nearTarget);
            var speed = baseDegree.recalculation(digree) + digree;
            thrust(speed, reactPower, maximumSpeed);
        });
        yield break;
    }
    /// <summary>
    /// 照準操作行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator motionAiming(int actionNum)
    {
        nextActionState = ActionPattern.ATTACK;
        nextActionIndex = (nearTarget.position.magnitude < arms[1].tipLength).toInt();
        yield return aimingAction(() => nearTarget.position, () => nowSpeed.magnitude > 0, () => thrustStop());
        yield break;
    }
    /// <summary>
    /// 攻撃行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator motionAttack(int actionNum)
    {
        nextActionState = ActionPattern.MOVE;

        int armNum = (siteAlignment.magnitude < arms[1].tipLength).toInt();
        arms[armNum].tipHand.actionWeapon(Weapon.ActionType.NOMAL);

        if(onTheWay && actionCount++ >= shipLevel) nextActionState = ActionPattern.ESCAPE;

        yield return wait(interval);
        yield break;
    }
    int actionCount = 0;
}
