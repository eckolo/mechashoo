using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Npc.ActionPattern;

public class Nusepuleje : Kemi
{
    Weapon lance => allWeapons[0];
    Weapon grenade => allWeapons[1];
    Weapon assaulter => allWeapons[2];
    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator motionMove(int actionNum)
    {
        nextActionState = AIMING;
        yield return headingDestination(bodyAimPosition, maximumSpeed, () => {
            aiming(nearTarget.position);
            setBaseAiming();
        });
        yield return thrustStop();
        nextActionIndex = Random.Range(0, allWeapons.Count);
        nextActionIndex = 0;
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
            //槍
            case 0:
                var distination = seriousMode ?
                    nearTarget.position + Vector2.right * viewSize.x / 2 * targetSign :
                    nearTarget.position - Vector2.right * spriteSize.x * targetSign;
                if(!seriousMode) setFixedAlignment(nearTarget.position);
                yield return headingDestination(distination, maximumSpeed, () => {
                    aiming(nearTarget.position);
                    if(seriousMode)
                    {
                        resetAllAim(2);
                    }
                    else
                    {
                        var tweak = Mathf.Abs(nearTarget.position.x - position.x) * Vector2.down;
                        aiming(nearTarget.position + tweak, 0);
                    }
                });
                yield return stoppingAction();
                break;
            //炸裂弾
            case 1:
                yield return aimingAction(() => getDeviationTarget(nearTarget), () => nowSpeed.magnitude > 0, aimingProcess: () => {
                    thrustStop();
                    if(seriousMode) resetAllAim();
                    else setBaseAiming();
                });
                break;
            //剛体弾
            case 2:
                yield return headingDestination(bodyAimPosition, maximumSpeed, () => {
                    aiming(nearTarget.position);
                    if(seriousMode)
                    {
                        aiming(nearTarget.position + Vector2.up * diff * vertical, 0);
                        resetAllAim();
                    }
                    else
                    {
                        setBaseAiming();
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

        //槍
        if(actionNum == 0)
        {
            yield return wait(() => lance.canAction);
            if(seriousMode)
            {
                var approachDistance = Vector2.right * viewSize.x / 2 * targetSign;
                var distination = nearTarget.position - approachDistance;
                lance.action(Weapon.ActionType.SINK);
                yield return headingDestination(distination, maximumSpeed * 1.5f, () => {
                    aiming(nearTarget.position);
                    var tweak = Mathf.Abs(nearTarget.position.x - position.x) * Vector2.up;
                    aiming(nearTarget.position + tweak, 0);
                });
                yield return stoppingAction();
            }
            else
            {
                lance.action(Weapon.ActionType.NOMAL);
            }
            yield return wait(() => lance.canAction);
        }
        //炸裂弾
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
                    if(seriousMode) resetAllAim();
                    else setBaseAiming();
                    thrustStop();
                    yield return wait(1);
                }
                assaulter.action(Weapon.ActionType.NOMAL);
                var distination = nearTarget.position;
                if(seriousMode) setFixedAlignment(distination);
                for(int time = 0; time < interval || !lance.canAction; time++)
                {
                    aiming(nearTarget.position);
                    if(seriousMode) aiming(distination, 0);
                    else setBaseAiming();
                    thrust(bodyAimPosition - position, reactPower, maximumSpeed);
                    yield return wait(1);
                }
                if(seriousMode) lance.action(Weapon.ActionType.NOMAL);
            }
        }
        //剛体弾
        else if(actionNum == 2)
        {
            for(int time = 0; time < interval; time++)
            {
                thrust(new Vector2(position.x, nearTarget.position.y) - position, reactPower, moderateSpeed);
                aiming(nearTarget.position);
                if(seriousMode) resetAllAim();
                else setBaseAiming();
                yield return wait(1);

                var remaining = 5 - (interval - time);
                if(remaining > 0) setFixedAlignment(new Vector2(viewSize.x * remaining / 8, bodyWeaponRoot.y), true);
            }
            if(seriousMode)
            {
                setFixedAlignment(0);
                lance.action(Weapon.ActionType.NOMAL);
                assaulter.action(Weapon.ActionType.NOMAL);
            }
            grenade.action(Weapon.ActionType.SINK);
            yield return headingDestination(bodyAimPosition, moderateSpeed, () => setBaseAiming());
            yield return stoppingAction();

            yield return aimingAction(() => nearTarget.position, interval, aimingProcess: () => setBaseAiming());
        }

        for(int time = 0; time < interval; time++)
        {
            setBaseAiming(2);
            thrustStop();
            yield return wait(1);
        }
        yield break;
    }
}
