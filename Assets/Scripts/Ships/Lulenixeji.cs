using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Lulenixeji : Guhabaji
{
    Weapon rod => allWeapons[0];
    Weapon rifle => allWeapons[1];
    Weapon hammer => allWeapons[2];

    enum MotionType
    {
        ROD,
        ROD_LASER,
        CHAIN,
        CHAIN_DUBLE,
        HAMMER,
        HAMMER_BURST,
        HAMMER_HUGE,
    }

    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        var moderateSpeed = (lowerSpeed + maximumSpeed) / 2;
        yield return HeadingDestination(standardPosition, moderateSpeed, grappleDistance, () => {
            Aiming(nearTarget.position);
            SetBaseAiming();
        });
        yield return StoppingAction();
        nextActionIndex = seriousMode ?
            (int)new[] {
                MotionType.ROD,
                MotionType.ROD_LASER,
                MotionType.CHAIN,
                MotionType.CHAIN_DUBLE,
                MotionType.HAMMER,
                MotionType.HAMMER_BURST,
                MotionType.HAMMER_HUGE
            }.SelectRandom(new[] { 2, 5, 1, 3, 3, 4, 3 }) :
            (int)new[] {
                MotionType.ROD,
                MotionType.ROD_LASER,
                MotionType.CHAIN,
                MotionType.CHAIN_DUBLE,
                MotionType.HAMMER,
                MotionType.HAMMER_BURST
            }.SelectRandom(new[] { 3, 3, 5, 3, 5, 2 });
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
        var motion = actionNum.Normalize<MotionType>();
        switch(motion)
        {
            case MotionType.ROD:
            case MotionType.ROD_LASER:
                yield return HeadingDestination(approachPosition, maximumSpeed, grappleDistance, () => {
                    Aiming(nearTarget.position);
                    var tweak = Mathf.Abs(nearTarget.position.x - position.x) * Vector2.down;
                    Aiming(nearTarget.position + tweak, 0);
                });
                yield return StoppingAction();
                SetFixedAlignment(Vector2.right * grappleDistance + bodyWeaponRoot, true);
                break;
            case MotionType.CHAIN:
            case MotionType.CHAIN_DUBLE:
                yield return HeadingDestination(standardPosition, maximumSpeed, grappleDistance, () => {
                    Aiming(nearTarget.position);
                    SetBaseAiming();
                });
                SetFixedAlignment(Vector2.right * gunDistance + bodyWeaponRoot, true);
                yield return StoppingAction();
                break;
            case MotionType.HAMMER:
                yield return HeadingDestination(standardPosition, maximumSpeed, grappleDistance, () => {
                    Aiming(nearTarget.position);
                    SetBaseAiming();
                });
                yield return StoppingAction();
                break;
            case MotionType.HAMMER_BURST:
                yield return HeadingDestination(standardPosition * 2 - nearTarget.position, maximumSpeed, grappleDistance, () => {
                    Aiming(nearTarget.position);
                    SetBaseAiming();
                });
                yield return StoppingAction();
                break;
            case MotionType.HAMMER_HUGE:
                var distination = nearTarget.position + (Vector2)(Random.Range(-90f, 90f).ToRotation() * Vector2.left * gunDistance * targetSign);
                yield return HeadingDestination(distination, maximumSpeed, grappleDistance, () => {
                    Aiming(nearTarget.position);
                    SetBaseAiming();
                });
                yield return StoppingAction();
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
        nextActionState = ActionPattern.MOVE;
        var motion = actionNum.Normalize<MotionType>();
        var moderateSpeed = (lowerSpeed + maximumSpeed) / 2;
        switch(motion)
        {
            case MotionType.ROD:
                yield return Wait(() => rod.canAction);
                if(seriousMode) Thrust(approachPosition - position);
                rod.Action(Weapon.ActionType.NOMAL);
                if(seriousMode)
                {
                    yield return Wait(() => rod.onAttack);
                    yield return Wait(() => !rod.onAttack);
                    hammer.Action(Weapon.ActionType.NOMAL);
                    yield return HeadingDestination(approachPosition, maximumSpeed, () => {
                        Aiming(nearTarget.position);
                        SetBaseAiming();
                    });
                    yield return StoppingAction();
                }
                yield return Wait(() => rod.canAction);
                break;
            case MotionType.ROD_LASER:
                yield return Wait(() => rod.canAction);
                rod.Action(Weapon.ActionType.NPC);
                rod.Action(Weapon.ActionType.SINK);
                yield return Wait(() => rod.onAttack);
                yield return Wait(() => !rod.onAttack);
                SetFixedAlignment(nearTarget.position);
                if(seriousMode) yield return AimingAction(() => nearTarget.position, armIndex: 0);
                else yield return AimingAction(nearTarget.position, armIndex: 0);
                yield return Wait(() => rod.canAction);
                break;
            case MotionType.CHAIN:
                yield return Wait(() => rod.canAction);
                if(seriousMode) Thrust(standardPosition - position);
                rifle.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => rod.canAction);
                break;
            case MotionType.CHAIN_DUBLE:
                yield return Wait(() => rifle.canAction);
                rifle.Action(Weapon.ActionType.SINK);
                for(int time = 0; time < interval; time++)
                {
                    Thrust(nowForward * -1, reactPower, maximumSpeed);
                    Aiming(nearTarget.position);
                    SetBaseAiming();
                    yield return Wait(1);
                }
                yield return StoppingAction();
                if(seriousMode)
                {
                    yield return Wait(() => rifle.canAction);
                    SetFixedAlignment(Vector2.right * gunDistance + bodyWeaponRoot, true);
                    rifle.Action(Weapon.ActionType.SINK);
                    yield return Wait(() => rifle.canAction);
                }
                break;
            case MotionType.HAMMER:
                yield return Wait(() => hammer.canAction);
                yield return HeadingDestination(approachPosition, maximumSpeed, () => {
                    Aiming(nearTarget.position, siteSpeedTweak: 2);
                    SetBaseAiming(2);
                });
                SetFixedAlignment(Vector2.right * grappleDistance + bodyWeaponRoot, true);
                hammer.Action(Weapon.ActionType.NOMAL);
                yield return StoppingAction();
                if(seriousMode && (nearTarget.position - position).magnitude < grappleDistance)
                {
                    hammer.Action(Weapon.ActionType.NOMAL);
                    yield return HeadingDestination(approachPosition, maximumSpeed, () => {
                        Aiming(nearTarget.position, siteSpeedTweak: 2);
                        SetBaseAiming(2);
                    });
                }
                yield return Wait(() => hammer.canAction);
                break;
            case MotionType.HAMMER_BURST:
                var approachDirection = approachPosition - position;
                for(var time = 0; time < interval; time++)
                {
                    Thrust(approachDirection, targetSpeed: maximumSpeed);
                    yield return Wait(1);
                }
                yield return Wait(() => hammer.canAction);
                var repetitions = Random.Range(1, shipLevel) + 2;
                for(int count = 0; count < repetitions; count++)
                {
                    SetFixedAlignment(Vector2.right * grappleDistance + bodyWeaponRoot, true);
                    hammer.Action(Weapon.ActionType.NPC);
                    while(!hammer.onAttack)
                    {
                        Thrust(approachPosition - position, targetSpeed: maximumSpeed);
                        Aiming(nearTarget.position);
                        SetBaseAiming();
                        yield return Wait(1);
                    }
                    hammer.Action(Weapon.ActionType.NPC);
                    while(hammer.onAttack)
                    {
                        var attackWay = seriousMode ?
                            approachPosition - position :
                            approachPosition - position + nowSpeed;
                        Thrust(attackWay, targetSpeed: maximumSpeed);
                        Aiming(nearTarget.position, siteSpeedTweak: 0.5f);
                        SetBaseAiming();
                        yield return Wait(1);
                    }
                }
                yield return Wait(() => hammer.canAction);
                break;
            case MotionType.HAMMER_HUGE:
                yield return Wait(() => hammer.canAction);
                SetFixedAlignment(nearTarget.position);
                hammer.Action(Weapon.ActionType.SINK);
                yield return HeadingDestination(approachPosition, maximumSpeed * 2, () => {
                    Aiming(nearTarget.position);
                    SetBaseAiming();
                });
                yield return StoppingAction(power: 2);
                yield return Wait(() => hammer.canAction);
                break;
            default:
                break;
        }
        var way = Random.Range(0f, 360f).ToRotation() * Vector2.right;
        for(int time = 0; time < interval; time++)
        {
            Aiming(nearTarget.position);
            SetBaseAiming(2);
            Thrust(way, targetSpeed: moderateSpeed);
            yield return Wait(1);
        }
        yield return StoppingAction();
        yield break;
    }
    protected override float properDistance => base.properDistance * 2;
}
