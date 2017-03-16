using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Npc.ActionPattern;

public class Jodimucuji : Npc
{
    //0:grenade
    //1:assaulter
    //2:laser
    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator motionMove(int actionNum)
    {
        var moderateSpeed = (lowerSpeed + maximumSpeed) / 2;
        nextActionState = AIMING;
        var destinationCorrection = Random.Range(-90, 90);
        yield return aimingAction(() => nearTarget.position, interval * 2, aimingProcess: () => {
            var degree = getProperPosition(nearTarget, destinationCorrection);
            var destination = nearTarget.position - position - siteAlignment;
            thrust(destination + degree, reactPower, moderateSpeed);
        });
        nextActionIndex = Random.Range(0, allWeapons.Count);
        yield break;
    }
    /// <summary>
    /// 照準操作行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator motionAiming(int actionNum)
    {
        nextActionState = ATTACK;
        switch(actionNum)
        {
            case 0:
                yield return aimingAction(nearTarget.position, () => nowSpeed.magnitude > 0, aimingProcess: () => thrustStop());
                break;
            case 1:
                yield return aimingAction(() => getDeviationTarget(nearTarget), () => nowSpeed.magnitude > 0, aimingProcess: () => thrustStop());
                break;
            case 2:
                var _destination = new Vector2(position.x, getDeviationTarget(nearTarget).y);
                yield return headingDestination(_destination, maximumSpeed);
                break;
            default:
                break;
        }
        yield break;
    }
    /// <summary>
    /// 攻撃行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator motionAttack(int actionNum)
    {
        allWeapons[actionNum].action(Weapon.ActionType.NOMAL);
        var continuous = actionNum == 1
            && new List<bool> { true, false }.selectRandom(new List<int> { 2, 1 });
        if(continuous)
        {
            var limit = Random.Range(1, shipLevel + 1);
            var speed = Mathf.Min(lowerSpeed * limit, maximumSpeed);
            for(int index = 0; index < limit && nearTarget.isAlive; index++)
            {
                var weapon = allWeapons[actionNum];
                yield return aimingAction(() => getDeviationTarget(nearTarget, 5),
                    interval,
                    aimingProcess: () => thrust(getProperPosition(nearTarget), reactPower, maximumSpeed));
                yield return aimingAction(() => getDeviationTarget(nearTarget, 5),
                    () => !weapon.canAction,
                    aimingProcess: () => thrustStop());
                weapon.action(Weapon.ActionType.NOMAL);
            }
        }
        if(actionNum == 2)
        {
            nextActionState = new List<ActionPattern> { AIMING, MOVE }.selectRandom(new List<int> { 1, 2 });
            nextActionIndex = Random.Range(0, allWeapons.Count(weapon => weapon != allWeapons[actionNum]));
            yield return thrustStop();
            yield return aimingAction(() => nearTarget.position, interval);
        }
        else
        {
            nextActionState = MOVE;
            yield return wait(interval);
        }
        yield break;
    }
}
