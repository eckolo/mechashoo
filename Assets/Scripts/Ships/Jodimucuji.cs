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
            resetAllAim();
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
        var positionDiff = nearTarget.position - position;
        var vertical = positionDiff.y.toSign();
        var diff = Mathf.Max(Mathf.Abs(positionDiff.magnitude / 2), 1);

        switch(actionNum)
        {
            case 0:
                if(!seriousMode) diff = 0;
                yield return aimingAction(nearTarget.position, () => nowSpeed.magnitude > 0, aimingProcess: () => {
                    aiming(nearTarget.position + (Vector2)(siteAlignment.toRotation() * Vector2.up * diff * vertical), 0);
                    thrustStop();
                });
                yield return aimingAction(() => nearTarget.position, () => !grenade.canAction);
                break;
            case 1:
                yield return aimingAction(() => getDeviationTarget(nearTarget), () => nowSpeed.magnitude > 0, aimingProcess: () => thrustStop());
                break;
            case 2:
                yield return headingDestination(laserAimPosition, maximumSpeed, () => {
                    aiming(nearTarget.position);
                    aiming(nearTarget.position + Vector2.up * diff * vertical, 0);
                    aiming(nearTarget.position + Vector2.up * diff * vertical * -1, 1);
                });
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
            if(seriousMode)
            {
                grenade.action(Weapon.ActionType.NOMAL, 0.1f);

                var diff = armAlignments[0] - siteAlignment;
                yield return aimingAction(() => nearTarget.position - diff, 0, aimingProcess: () => aiming(nearTarget.position), finishRange: 0);
                yield return aimingAction(() => nearTarget.position, () => !grenade.canAction);
                grenade.action(Weapon.ActionType.NOMAL, 0.1f);

                yield return aimingAction(() => nearTarget.position, 0, aimingProcess: () => aiming(nearTarget.position), finishRange: 0);
                yield return aimingAction(() => nearTarget.position, () => !grenade.canAction);
            }
            grenade.action(Weapon.ActionType.NOMAL);

            for(int time = 0; time < interval; time++)
            {
                resetAllAim(2);
                yield return wait(1);
            }
            while(armAlignments.Any(alignment => alignment != siteAlignment))
            {
                resetAllAim(2);
                yield return wait(1);
            }
        }
        if(actionNum == 1)
        {
            var limit = Random.Range(1, shipLevel + 1) + 1;
            assaulter.action(Weapon.ActionType.NOMAL);
            for(int index = 0; index < limit && nearTarget.isAlive; index++)
            {
                var distinationTweak = new[] { 90, -90 }.selectRandom();
                var distination = nearTarget.position;
                for(int time = 0; time < interval || !grenade.canAction; time++)
                {
                    aiming(nearTarget.position);
                    aiming(distination, 0);
                    resetAim(1);
                    thrust(getProperPosition(nearTarget, distinationTweak), reactPower, maximumSpeed);
                    yield return wait(1);
                }
                if(seriousMode) grenade.action(Weapon.ActionType.NOMAL);
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
                thrustStop();
                yield return wait(1);
            }
        }
        if(actionNum == 2)
        {
            for(int time = 0; time < interval; time++)
            {
                thrust(new Vector2(position.x, nearTarget.position.y) - position, reactPower, moderateSpeed);
                aiming(nearTarget.position);
                yield return wait(1);
            }
            if(seriousMode)
            {
                grenade.action(Weapon.ActionType.NOMAL);
                assaulter.action(Weapon.ActionType.SINK);
            }
            laser.action(Weapon.ActionType.NOMAL);
            yield return headingDestination(laserAimPosition, moderateSpeed, () =>
                   resetAllAim(2));
            yield return stoppingAction();

            yield return aimingAction(() => nearTarget.position, interval, aimingProcess: () =>
                  resetAllAim(2));
        }
        yield break;
    }

    Vector2 laserAimPosition => new Vector2((position.x + nearTarget.position.x) / 2, nearTarget.position.y);
}
