using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Npc.ActionPattern;

public class Jodimucuji : Kemi
{
    Weapon grenade => allWeapons[0];
    Weapon laser => allWeapons[1];
    Weapon assaulter => allWeapons[2];
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
        var diff = Mathf.Max(Mathf.Abs(positionDiff.magnitude / 2), 2);

        switch(actionNum)
        {
            case 0:
                if(seriousMode)
                {
                    yield return aimingAction(() => nearTarget.position, () => nowSpeed.magnitude > 0, aimingProcess: () => {
                        aiming(nearTarget.position + (Vector2)(siteAlignment.toRotation() * Vector2.up * diff * vertical), 0);
                        thrustStop();
                    });
                    setFixedAlignment(0);
                }
                else
                {
                    var targetPosition = nearTarget.position;
                    setFixedAlignment(targetPosition);
                    yield return aimingAction(targetPosition, () => nowSpeed.magnitude > 0, aimingProcess: () => {
                        aiming(targetPosition);
                        thrustStop();
                    });
                }
                yield return aimingAction(() => nearTarget.position, () => !grenade.canAction);
                break;
            case 1:
                yield return aimingAction(() => getDeviationTarget(nearTarget), () => nowSpeed.magnitude > 0, aimingProcess: () => thrustStop());
                break;
            case 2:
                yield return headingDestination(bodyAimPosition, maximumSpeed, () => {
                    aiming(nearTarget.position);
                    if(seriousMode)
                    {
                        aiming(nearTarget.position + Vector2.up * diff * vertical, 0);
                    }
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

        //炸裂弾
        //本気時：3連バースト射撃
        if(actionNum == 0)
        {
            if(seriousMode)
            {
                grenade.action(Weapon.ActionType.NOMAL, 0.1f);

                var diff = armAlignments[0] - siteAlignment;
                yield return aimingAction(() => nearTarget.position - diff, 0, aimingProcess: () => aiming(nearTarget.position), finishRange: 0);
                yield return aimingAction(() => nearTarget.position, () => !grenade.canAction);
                setFixedAlignment(0);
                grenade.action(Weapon.ActionType.NOMAL, 0.1f);

                yield return aimingAction(() => nearTarget.position, 0, aimingProcess: () => aiming(nearTarget.position), finishRange: 0);
                yield return aimingAction(() => nearTarget.position, () => !grenade.canAction);
                setFixedAlignment(0);
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
        //剛体弾連射
        //本気時：炸裂弾との交互連射
        else if(actionNum == 1)
        {
            var limit = Random.Range(1, shipLevel + 1) + 1;
            assaulter.action(Weapon.ActionType.NOMAL);
            for(int index = 0; index < limit && nearTarget.isAlive; index++)
            {
                setFixedAlignment(new Vector2(Mathf.Abs(nearTarget.position.x - position.x), bodyWeaponRoot.y), true);
                while(!assaulter.canAction)
                {
                    aiming(nearTarget.position);
                    thrustStop();
                    yield return wait(1);
                }
                assaulter.action(Weapon.ActionType.NOMAL);
                var distination = nearTarget.position;
                if(seriousMode) setFixedAlignment(distination);
                for(int time = 0; time < interval || !grenade.canAction; time++)
                {
                    aiming(distination);
                    thrust(bodyAimPosition - position, reactPower, maximumSpeed);
                    yield return wait(1);
                }
                if(seriousMode) grenade.action(Weapon.ActionType.NOMAL);
            }
            while(armAlignments.Any(alignment => alignment != siteAlignment))
            {
                resetAllAim(2);
                thrustStop();
                yield return wait(1);
            }
        }
        //レーザー砲
        //本気時：牽制射撃の後レーザー砲
        else if(actionNum == 2)
        {
            for(int time = 0; time < interval; time++)
            {
                thrust(new Vector2(position.x, nearTarget.position.y) - position, reactPower, moderateSpeed);
                aiming(nearTarget.position);
                yield return wait(1);

                var remaining = 5 - (interval - time);
                if(remaining > 0) setFixedAlignment(new Vector2(viewSize.x * remaining / 8, bodyWeaponRoot.y), true);
            }
            if(seriousMode)
            {
                setFixedAlignment(0);
                grenade.action(Weapon.ActionType.NOMAL);
                assaulter.action(Weapon.ActionType.NOMAL);
            }
            laser.action(Weapon.ActionType.NOMAL);
            yield return headingDestination(bodyAimPosition, moderateSpeed, () => resetAllAim(2));
            yield return stoppingAction();

            yield return aimingAction(() => nearTarget.position, interval, aimingProcess: () => resetAllAim(2));
        }
        yield break;
    }
}
