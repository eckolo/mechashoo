using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Yikegojomuo : Boss
{
    Weapon sword => allWeapons[0];
    Weapon grenade => allWeapons[1];

    enum MotionType
    {
        SWORD,
        SWORD_TREMOR,
        SWORD_CHARGE,
        SLASHING_YEN,
        GRENADE,
        GRENADE_BURST,
        GRENADE_TURNBACK
    }

    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns>コルーチン</returns>
    protected override IEnumerator MotionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        yield return HeadingDestination(standardPosition, maximumSpeed, grappleDistance, () => {
            Aiming(nearTarget.position);
            SetBaseAimingAll();
        });
        yield return StoppingAction();
        nextActionIndex = seriousMode ?
            (int)new[] {
                MotionType.SWORD,
                MotionType.SWORD_TREMOR,
                MotionType.SWORD_CHARGE,
                MotionType.SLASHING_YEN,
                MotionType.GRENADE,
                MotionType.GRENADE_BURST,
                MotionType.GRENADE_TURNBACK
            }.SelectRandom(new[] { 1, 2, 5, 3, 1, 3, 5 }) :
            (int)new[] {
                MotionType.SWORD,
                MotionType.SWORD_TREMOR,
                MotionType.SWORD_CHARGE,
                MotionType.SLASHING_YEN,
                MotionType.GRENADE,
                MotionType.GRENADE_BURST,
                MotionType.GRENADE_TURNBACK
            }.SelectRandom(new[] { 3, 5, 3, 1, 5, 3, 1 });
        nextActionIndex = (int)MotionType.GRENADE_BURST;
        yield break;
    }
    /// <summary>
    /// 照準操作行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns>コルーチン</returns>
    protected override IEnumerator MotionAiming(int actionNum)
    {
        nextActionState = ActionPattern.ATTACK;
        var motion = actionNum.Normalize<MotionType>();

        switch(motion)
        {
            case MotionType.SWORD:
                yield return HeadingDestination(approachPosition, maximumSpeed, nowSpeed.magnitude, () => {
                    Aiming(nearTarget.position, siteSpeedTweak: 0.5f);
                    Aiming(nearTarget.position, 0);
                    Aiming(standardAimPosition, 1);
                });
                if(seriousMode)
                {
                    yield return HeadingDestination(approachPosition, maximumSpeed * 2, nowSpeed.magnitude, () => {
                        Aiming(nearTarget.position, siteSpeedTweak: 0.5f);
                        Aiming(nearTarget.position, 0);
                        Aiming(standardAimPosition, 1);
                    });
                }
                SetFixedAlignment(0);
                yield return StoppingAction();
                break;
            case MotionType.SWORD_TREMOR:
                yield return HeadingDestination(approachPosition, maximumSpeed * (seriousMode ? 3 : 1.5f), nowSpeed.magnitude, () => {
                    Aiming(nearTarget.position);
                    Aiming(nearTarget.position, 0);
                    Aiming(standardAimPosition, 1, 2);
                });
                SetFixedAlignment(0);
                break;
            case MotionType.SWORD_CHARGE:
                for(int time = 0; time < interval; time++)
                {
                    Aiming(nearTarget.position);
                    Aiming(nearTarget.position, 0);
                    Aiming(standardAimPosition, 1);
                    yield return Wait(1);
                }
                break;
            case MotionType.SLASHING_YEN:
                var destination = nearTarget.position + Vector2.right * viewSize.x * targetSign;
                yield return HeadingDestination(destination, maximumSpeed * 1.5f, nowSpeed.magnitude, () => {
                    Aiming(nearTarget.position);
                    Aiming(nearTarget.position, 0);
                    Aiming(standardAimPosition, 1);
                });
                yield return StoppingAction();
                break;
            case MotionType.GRENADE:
                SetFixedAlignment(nearTarget.position);
                yield return AimingAction(nearTarget.position,
                    armIndex: 1,
                    siteSpeedTweak: 0.5f,
                    aimingProcess: () => {
                        Aiming(nearTarget.position, siteSpeedTweak: 0.5f);
                        Aiming(standardAimPosition, 0, 2);
                    });
                yield return StoppingAction();
                break;
            case MotionType.GRENADE_BURST:
                {
                    var positionDiff = nearTarget.position - position;
                    var vertical = positionDiff.y.ToSign();
                    var diff = Vector2.up * Mathf.Max(positionDiff.magnitude / 2, grappleDistance) * vertical;
                    var targetPosition = nearTarget.position + (Vector2)(siteAlignment.ToRotation() * (seriousMode ? diff * 2 : diff));
                    SetFixedAlignment(targetPosition);
                    yield return AimingAction(targetPosition,
                        armIndex: 1,
                        siteSpeedTweak: 0.5f,
                        aimingProcess: () => {
                            Aiming(nearTarget.position, siteSpeedTweak: 0.5f);
                            Aiming(standardAimPosition, 0, 2);
                        });
                    yield return StoppingAction();
                }
                break;
            case MotionType.GRENADE_TURNBACK:
                {
                    yield return HeadingDestination(standardPosition, maximumSpeed, nowSpeed.magnitude, () => {
                        Aiming(nearTarget.position);
                        Aiming(standardAimPosition, 0, 2);
                        Aiming(standardAimPosition, 1, 2);
                    });
                    var positionDiff = nearTarget.position - position;
                    var vertical = positionDiff.y.ToSign();
                    var diff = Vector2.up * Mathf.Abs(positionDiff.magnitude / 2) * vertical;
                    var tweakPosition = (Vector2)(siteAlignment.ToRotation() * diff);
                    var targetPosition = seriousMode ? nearTarget.position - tweakPosition : nearTarget.position;
                    SetFixedAlignment(targetPosition);
                    yield return HeadingDestination((nearTarget.position + tweakPosition) * 2 - position, maximumSpeed * 2, nowSpeed.magnitude, () => {
                        Aiming(targetPosition);
                        Aiming(standardAimPosition, 0, 2);
                        Aiming(targetPosition, 1, 2);
                    });
                }
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
    /// <returns>コルーチン</returns>
    protected override IEnumerator MotionAttack(int actionNum)
    {
        nextActionState = ActionPattern.MOVE;
        var motion = actionNum.Normalize<MotionType>();
        var finishMotion = true;
        switch(motion)
        {
            case MotionType.SWORD:
                yield return Wait(() => sword.canAction);
                sword.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => sword.canAction);
                break;
            case MotionType.SWORD_TREMOR:
                yield return Wait(() => sword.canAction);
                sword.Action(Weapon.ActionType.NPC);
                yield return StoppingAction();
                yield return Wait(() => sword.canAction);
                break;
            case MotionType.SWORD_CHARGE:
                {
                    SetFixedAlignment(nearTarget.position);
                    yield return Wait(() => sword.canAction);
                    sword.Action(Weapon.ActionType.NOMAL);
                    while(!sword.onFollowThrough)
                    {
                        var direction = nearTarget.position - position;
                        var endDistance = grappleDistance + nowSpeed.magnitude;
                        if(direction.magnitude > endDistance) Thrust(direction, targetSpeed: maximumSpeed * 2);
                        else ThrustStop();
                        Debug.Log(nowSpeed);
                        yield return Wait(1);
                    }
                    yield return StoppingAction();
                    yield return Wait(() => sword.canAction);
                }
                break;
            case MotionType.SLASHING_YEN:
                if(seriousMode)
                {
                    var positionDiff = nearTarget.position - position;
                    var vertical = positionDiff.y.ToSign();
                    var diff = Vector2.up * Mathf.Abs(positionDiff.magnitude / 2) * vertical;
                    yield return AimingAction(nearTarget.position + (Vector2)(siteAlignment.ToRotation() * diff), armIndex: 1, aimingProcess: () => {
                        Aiming(nearTarget.position);
                        Aiming(standardAimPosition, 0, 2);
                    });
                    grenade.Action(Weapon.ActionType.NPC);
                }
                yield return Wait(() => sword.canAction);
                sword.Action(Weapon.ActionType.SINK);
                SetFixedAlignment(nearTarget.position);
                yield return Wait(() => sword.onAntiSeptation);
                while(sword.onAntiSeptation)
                {
                    Thrust(nearTarget.position - position, targetSpeed: maximumSpeed * (seriousMode ? 3 : 2));
                    Aiming(nearTarget.position);
                    Aiming(nearTarget.position, 0);
                    Aiming(standardAimPosition, 1, 2);
                    yield return Wait(1);
                }
                yield return Wait(() => sword.onAttack);
                while(sword.onAttack)
                {
                    Thrust(nearTarget.position - position, targetSpeed: maximumSpeed * (seriousMode ? 2 : 1.5f));
                    Aiming(nearTarget.position, 0);
                    Aiming(standardAimPosition, 1);
                    yield return Wait(1);
                }
                yield return StoppingAction();
                yield return Wait(() => sword.canAction);
                break;
            case MotionType.GRENADE:
                yield return Wait(() => grenade.canAction);
                var diffDistance = (nearTarget.position - position).magnitude;
                grenade.Action(seriousMode && diffDistance <= grappleDistance ?
                    Weapon.ActionType.SINK :
                    seriousMode && diffDistance > gunDistance ?
                    Weapon.ActionType.NPC :
                    Weapon.ActionType.NOMAL);
                yield return Wait(() => grenade.canAction);
                break;
            case MotionType.GRENADE_BURST:
                {
                    yield return Wait(() => grenade.canAction);
                    var diffAlignment = armAlignments[1] + position - nearTarget.position;
                    var targetPositions = new[]
                    {
                        nearTarget.position + diffAlignment / 2,
                        nearTarget.position,
                        nearTarget.position - diffAlignment / 2,
                        nearTarget.position - diffAlignment
                    };
                    grenade.Action(Weapon.ActionType.NOMAL, 0.1f);

                    if(seriousMode)
                    {
                        SetFixedAlignment(targetPositions[0]);
                        yield return AimingAction(targetPositions[0], armIndex: 1, siteSpeedTweak: seriousMode ? 2 : 1);
                        yield return Wait(() => grenade.canAction);
                        grenade.Action(Weapon.ActionType.NPC, 0.1f);
                    }

                    SetFixedAlignment(targetPositions[1]);
                    yield return AimingAction(targetPositions[1], armIndex: 1, siteSpeedTweak: seriousMode ? 2 : 1);
                    yield return Wait(() => grenade.canAction);
                    grenade.Action(Weapon.ActionType.NOMAL, 0.1f);

                    if(seriousMode)
                    {
                        SetFixedAlignment(targetPositions[2]);
                        yield return AimingAction(targetPositions[2], armIndex: 1, siteSpeedTweak: seriousMode ? 2 : 1);
                        yield return Wait(() => grenade.canAction);
                        grenade.Action(Weapon.ActionType.NPC, 0.1f);
                    }

                    SetFixedAlignment(targetPositions[3]);
                    yield return AimingAction(targetPositions[3], armIndex: 1, siteSpeedTweak: seriousMode ? 2 : 1);
                    yield return Wait(() => grenade.canAction);
                    grenade.Action(Weapon.ActionType.NOMAL);
                    yield return Wait(() => grenade.canAction);
                }
                break;
            case MotionType.GRENADE_TURNBACK:
                yield return Wait(() => grenade.canAction);
                grenade.Action(Weapon.ActionType.NOMAL);
                yield return StoppingAction(power: 2);
                if(seriousMode)
                {
                    yield return AimingAction(nearTarget.position, aimingProcess: () => {
                        Aiming(nearTarget.position, armIndex: 0);
                        Aiming(standardAimPosition, armIndex: 1);
                    });
                    nextActionState = ActionPattern.ATTACK;
                    nextActionIndex = (nearTarget.position - position).magnitude <= grappleDistance ?
                        (int)MotionType.SWORD_TREMOR :
                        (int)MotionType.SWORD_CHARGE;
                    finishMotion = false;
                }
                break;
            default:
                break;
        }
        for(int time = 0; finishMotion && time < interval; time++)
        {
            Aiming(nearTarget.position);
            SetBaseAimingAll();
            ThrustStop();
            yield return Wait(1);
        }
        yield break;
    }
}
