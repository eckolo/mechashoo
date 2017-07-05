using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Yikegojomuo : Npc
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
    /// <returns>イテレータ</returns>
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
        yield break;
    }
    /// <summary>
    /// 照準操作行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns>イテレータ</returns>
    protected override IEnumerator MotionAiming(int actionNum)
    {
        nextActionState = ActionPattern.ATTACK;
        var motion = actionNum.Normalize<MotionType>();
        var positionDiff = nearTarget.position - position;
        var vertical = positionDiff.y.ToSign();
        var diff = Vector2.up * Mathf.Abs(positionDiff.magnitude / 2) * vertical;

        switch(motion)
        {
            case MotionType.SWORD:
                yield return HeadingDestination(approachPosition, maximumSpeed * 1.5f, () => {
                    Aiming(nearTarget.position);
                    ResetAllAim(2);
                });
                if(seriousMode)
                {
                    yield return HeadingDestination(approachPosition, maximumSpeed * 3, () => {
                        Aiming(nearTarget.position);
                        ResetAllAim(2);
                    });
                }
                SetFixedAlignment(0);
                yield return StoppingAction();
                break;
            case MotionType.SWORD_TREMOR:
                yield return HeadingDestination(approachPosition, maximumSpeed * (seriousMode ? 4 : 2), () => {
                    Aiming(nearTarget.position, siteSpeedTweak: 2);
                    ResetAllAim(2);
                });
                SetFixedAlignment(0);
                break;
            case MotionType.SWORD_CHARGE:
                yield return HeadingDestination(standardPosition, maximumSpeed, () => {
                    Aiming(nearTarget.position, siteSpeedTweak: 2);
                    ResetAllAim(2);
                });
                break;
            case MotionType.SLASHING_YEN:
                var distination = nearTarget.position + Vector2.right * viewSize.x * targetSign;
                yield return HeadingDestination(distination, maximumSpeed * 2, () => {
                    Aiming(nearTarget.position, siteSpeedTweak: 2);
                    ResetAllAim(2);
                });
                yield return StoppingAction();
                break;
            case MotionType.GRENADE:
                yield return HeadingDestination(nearTarget.position, maximumSpeed, gunDistance);
                SetFixedAlignment(nearTarget.position);
                yield return AimingAction(nearTarget.position, 2, aimingProcess: () => ResetAllAim(2));
                yield return StoppingAction();
                break;
            case MotionType.GRENADE_BURST:
                yield return HeadingDestination(nearTarget.position, maximumSpeed, gunDistance);
                yield return AimingAction(() => nearTarget.position + (Vector2)(siteAlignment.ToRotation() * (seriousMode ? diff * 2 : diff)), armIndex: 1, aimingProcess: () => Aiming(nearTarget.position));
                SetFixedAlignment(1);
                yield return StoppingAction();
                break;
            case MotionType.GRENADE_TURNBACK:
                yield return HeadingDestination(standardPosition, maximumSpeed, () => {
                    Aiming(nearTarget.position);
                    ResetAllAim();
                });
                var tweakPosition = (Vector2)(siteAlignment.ToRotation() * diff);
                var targetPosition = seriousMode ? nearTarget.position - tweakPosition : nearTarget.position;
                SetFixedAlignment(targetPosition);
                yield return HeadingDestination((nearTarget.position + tweakPosition * 2) * 2 - position, maximumSpeed * 3, () => {
                    Aiming(nearTarget.position);
                    if(seriousMode) Aiming(targetPosition, 1);
                    else ResetAllAim();
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
    /// <returns>イテレータ</returns>
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
                SetFixedAlignment(nearTarget.position);
                yield return Wait(() => sword.canAction);
                sword.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => sword.onAttack);
                var distination = nearTarget.position
                    + Vector2.up * grappleDistance * Random.Range(-1, 1);
                yield return HeadingDestination(distination, maximumSpeed * 3, () => {
                    Aiming(nearTarget.position, siteSpeedTweak: 2);
                    ResetAllAim(2);
                });
                yield return Wait(() => !sword.onAttack);
                yield return StoppingAction();
                yield return Wait(() => sword.canAction);
                break;
            case MotionType.SLASHING_YEN:
                if(seriousMode)
                {
                    var positionDiff = nearTarget.position - position;
                    var vertical = positionDiff.y.ToSign();
                    var diff = Vector2.up * Mathf.Abs(positionDiff.magnitude / 2) * vertical;
                    yield return AimingAction(nearTarget.position + (Vector2)(siteAlignment.ToRotation() * diff), 1);
                    grenade.Action(Weapon.ActionType.NPC);
                }
                yield return Wait(() => sword.canAction);
                sword.Action(Weapon.ActionType.SINK);
                SetFixedAlignment(nearTarget.position);
                yield return HeadingDestination(nearTarget.position, maximumSpeed * (seriousMode ? 3 : 2), () => {
                    Aiming(nearTarget.position);
                    ResetAllAim(2);
                });
                yield return Wait(() => sword.onAttack);
                yield return HeadingDestination(nearTarget.position, maximumSpeed * (seriousMode ? 3 : 2), () => {
                    Aiming(nearTarget.position);
                    ResetAllAim(2);
                });
                yield return Wait(() => !sword.onAttack);
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
                yield return Wait(() => grenade.canAction);
                var diffAlignment = armAlignments[1] - siteAlignment;
                grenade.Action(Weapon.ActionType.NOMAL, 0.1f);

                if(seriousMode)
                {
                    yield return AimingAction(() => nearTarget.position + diffAlignment / 2, 1, siteSpeedTweak: 2, aimingProcess: () => Aiming(nearTarget.position), finishRange: 0);
                    yield return AimingAction(() => nearTarget.position, () => !grenade.canAction);
                    SetFixedAlignment(1);
                    grenade.Action(Weapon.ActionType.NPC, 0.1f);
                }

                yield return AimingAction(() => nearTarget.position, 1, siteSpeedTweak: 2, aimingProcess: () => Aiming(nearTarget.position), finishRange: 0);
                yield return AimingAction(() => nearTarget.position, () => !grenade.canAction);
                SetFixedAlignment(1);
                grenade.Action(Weapon.ActionType.NOMAL, 0.1f);

                if(seriousMode)
                {
                    yield return AimingAction(() => nearTarget.position - diffAlignment / 2, 1, siteSpeedTweak: 2, aimingProcess: () => Aiming(nearTarget.position), finishRange: 0);
                    yield return AimingAction(() => nearTarget.position, () => !grenade.canAction);
                    SetFixedAlignment(1);
                    grenade.Action(Weapon.ActionType.NPC, 0.1f);
                }

                yield return AimingAction(() => nearTarget.position - diffAlignment, 1, siteSpeedTweak: 2, aimingProcess: () => Aiming(nearTarget.position), finishRange: 0);
                yield return AimingAction(() => nearTarget.position, () => !grenade.canAction);
                SetFixedAlignment(1);
                grenade.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => grenade.canAction);
                break;
            case MotionType.GRENADE_TURNBACK:
                yield return Wait(() => grenade.canAction);
                grenade.Action(Weapon.ActionType.NOMAL);
                yield return StoppingAction();
                if(seriousMode)
                {
                    yield return AimingAction(nearTarget.position);
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
