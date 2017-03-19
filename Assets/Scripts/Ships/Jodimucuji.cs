using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Npc.ActionPattern;

public class Jodimucuji : Npc
{
    Weapon grenade => allWeapons[0];
    Weapon assaulter => allWeapons[1];
    Weapon laser => allWeapons[2];
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
        var moderateSpeed = (lowerSpeed + maximumSpeed) / 2;
        nextActionState = MOVE;

        if(actionNum == 0)
        {

        }
        if(actionNum == 1)
        {
            var limit = Random.Range(1, shipLevel + 1);
            assaulter.action(Weapon.ActionType.NOMAL);
            for(int index = 0; index < limit && nearTarget.isAlive; index++)
            {
                var distinationTweak = new[] { 90, -90 }.selectRandom();
                var distination = nearTarget.position;
                for(int time = 0; time < interval; time++)
                {
                    aiming(nearTarget.position);
                    aiming(distination, 0);
                    resetAim(1);
                    thrust(getProperPosition(nearTarget, distinationTweak), reactPower, maximumSpeed);
                    yield return wait(1);
                }
                grenade.action(Weapon.ActionType.NOMAL);
                while(!assaulter.canAction)
                {
                    aiming(nearTarget.position);
                    aiming(getDeviationTarget(nearTarget, 5), 1);
                    resetAim(0);
                    thrustStop();
                    yield return wait(1);
                }
                assaulter.action(Weapon.ActionType.NOMAL);
            }
            while(armAlignments.Any(alignment => alignment != siteAlignment))
            {
                resetAllAim(2);
                thrust(nowSpeed, reactPower, moderateSpeed);
                yield return wait(1);
            }
        }
        if(actionNum == 2)
        {
            nextActionState = new[] { AIMING, MOVE }.selectRandom(new[] { 1, 2 });
            nextActionIndex = Random.Range(0, allWeapons.Count(weapon => weapon != allWeapons[actionNum]));
            yield return thrustStop();
            yield return aimingAction(() => nearTarget.position, interval);
        }
        yield break;
    }
}
