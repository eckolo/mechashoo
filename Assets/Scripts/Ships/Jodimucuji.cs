using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Npc.ActionPattern;

public class Jodimucuji : Guhabaji
{
    Weapon grenade => allWeapons[0];
    Weapon laser => allWeapons[1];
    Weapon assaulter => allWeapons[2];
    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionMove(int actionNum)
    {
        var moderateSpeed = (lowerSpeed + maximumSpeed) / 2;
        nextActionState = AIMING;
        var destinationCorrection = Random.Range(-90, 90);
        yield return AimingAction(() => nearTarget.position, interval * 2, aimingProcess: () => {
            var degree = GetProperPosition(nearTarget, destinationCorrection);
            var destination = nearTarget.position - position - siteAlignment;
            Thrust(destination + degree, reactPower, moderateSpeed);
            SetBaseAiming();
        });
        nextActionIndex = Random.Range(0, allWeapons.Count);
        yield break;
    }
    /// <summary>
    /// 照準操作行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionAiming(int actionNum)
    {
        nextActionState = ATTACK;
        var positionDiff = nearTarget.position - position;
        var vertical = positionDiff.y.ToSign();
        var diff = Mathf.Max(Mathf.Abs(positionDiff.magnitude / 2), 2);

        switch(actionNum)
        {
            case 0:
                if(seriousMode)
                {
                    yield return AimingAction(() => nearTarget.position, () => nowSpeed.magnitude > 0, aimingProcess: () => {
                        Aiming(nearTarget.position + (Vector2)(siteAlignment.ToRotation() * Vector2.up * diff * vertical), 0);
                        ThrustStop();
                    });
                    SetFixedAlignment(0);
                }
                else
                {
                    var targetPosition = nearTarget.position;
                    SetFixedAlignment(targetPosition);
                    yield return AimingAction(targetPosition, () => nowSpeed.magnitude > 0, aimingProcess: () => {
                        ResetAllAim(2);
                        ThrustStop();
                    });
                }
                yield return AimingAction(() => nearTarget.position, () => !grenade.canAction);
                break;
            case 1:
                yield return AimingAction(() => GetDeviationTarget(nearTarget), () => nowSpeed.magnitude > 0, aimingProcess: () => {
                    ThrustStop();
                    if(seriousMode) ResetAllAim();
                    else SetBaseAiming();
                });
                break;
            case 2:
                yield return HeadingDestination(standardPosition, maximumSpeed, () => {
                    Aiming(nearTarget.position);
                    if(seriousMode)
                    {
                        Aiming(nearTarget.position + Vector2.up * diff * vertical, 0);
                        ResetAllAim();
                    }
                    else
                    {
                        SetBaseAiming();
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
    protected override IEnumerator MotionAttack(int actionNum)
    {
        var moderateSpeed = (lowerSpeed + maximumSpeed) / 2;
        nextActionState = MOVE;

        //炸裂弾
        //本気時：3連バースト射撃
        if(actionNum == 0)
        {
            if(seriousMode)
            {
                grenade.Action(Weapon.ActionType.NOMAL, 0.1f);

                var diff = armAlignments[0] - siteAlignment;
                yield return AimingAction(() => nearTarget.position - diff, 0, siteSpeedTweak: 2, aimingProcess: () => Aiming(nearTarget.position), finishRange: 0);
                yield return AimingAction(() => nearTarget.position, () => !grenade.canAction);
                SetFixedAlignment(0);
                grenade.Action(Weapon.ActionType.NOMAL, 0.1f);

                yield return AimingAction(() => nearTarget.position, 0, siteSpeedTweak: 2, aimingProcess: () => Aiming(nearTarget.position), finishRange: 0);
                yield return AimingAction(() => nearTarget.position, () => !grenade.canAction);
                SetFixedAlignment(0);
            }
            grenade.Action(Weapon.ActionType.NOMAL);
            yield return Wait(() => !grenade.canAction);
        }
        //剛体弾連射
        //本気時：炸裂弾との交互連射
        else if(actionNum == 1)
        {
            var limit = Random.Range(1, shipLevel + 1) + 1;
            assaulter.Action(Weapon.ActionType.NOMAL);
            for(int index = 0; index < limit && nearTarget.isAlive; index++)
            {
                SetFixedAlignment(new Vector2(Mathf.Abs(nearTarget.position.x - position.x), bodyWeaponRoot.y), true);
                while(!assaulter.canAction)
                {
                    Aiming(nearTarget.position);
                    if(seriousMode) ResetAllAim();
                    else SetBaseAiming();
                    ThrustStop();
                    yield return Wait(1);
                }
                assaulter.Action(Weapon.ActionType.NOMAL);
                var distination = nearTarget.position;
                if(seriousMode) SetFixedAlignment(distination);
                for(int time = 0; time < interval || !grenade.canAction; time++)
                {
                    Aiming(nearTarget.position);
                    if(seriousMode) Aiming(distination, 0);
                    else SetBaseAiming();
                    Thrust(standardPosition - position, reactPower, maximumSpeed);
                    yield return Wait(1);
                }
                if(seriousMode) grenade.Action(Weapon.ActionType.NOMAL);
            }
        }
        //レーザー砲
        //本気時：牽制射撃の後レーザー砲
        else if(actionNum == 2)
        {
            for(int time = 0; time < interval; time++)
            {
                Thrust(new Vector2(position.x, nearTarget.position.y) - position, reactPower, moderateSpeed);
                Aiming(nearTarget.position);
                if(seriousMode) ResetAllAim();
                else SetBaseAiming();
                yield return Wait(1);

                var remaining = 5 - (interval - time);
                if(remaining > 0) SetFixedAlignment(new Vector2(viewSize.x * remaining / 8, bodyWeaponRoot.y), true);
            }
            if(seriousMode)
            {
                SetFixedAlignment(0);
                grenade.Action(Weapon.ActionType.NOMAL);
                assaulter.Action(Weapon.ActionType.NOMAL);
            }
            laser.Action(Weapon.ActionType.NOMAL);
            yield return HeadingDestination(standardPosition, moderateSpeed, () => SetBaseAiming());
            yield return StoppingAction();

            yield return AimingAction(() => nearTarget.position, interval, aimingProcess: () => SetBaseAiming());
        }

        for(int time = 0; time < interval; time++)
        {
            SetBaseAiming(2);
            ThrustStop();
            yield return Wait(1);
        }
        yield break;
    }
}
