using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Qiyozovifo : Npc
{
    Weapon bubble => allWeapons[0];
    Weapon club => allWeapons[1];

    enum MotionType
    {
        CLUB,
        CLUB_LAST,
        CLUB_TREMOR,
        CLUB_SPRINKLE,
        BUBBLE,
        BUBBLE_BURST,
        BUBBLE_WIDE,
        BUBBLE_SURROUNDINGS,
        LASER,
        SIDEWAYS
    }

    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns>コルーチン</returns>
    protected override IEnumerator MotionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        yield return HeadingProperDestination(standardPosition.Rotate(nearTarget.position, Random.Range(-90f, 90f)),
            maximumSpeed,
            tweakDifference: 0.5f,
            endDistance: grappleDistance,
            concurrentProcess: () => {
                Aiming(nearTarget.position);
                SetBaseAimingAll();
            });
        yield return StoppingAction();
        nextActionIndex = seriousMode ?
            (int)new[] {
                MotionType.CLUB,
                MotionType.CLUB_TREMOR,
                MotionType.BUBBLE,
                MotionType.BUBBLE_BURST,
                MotionType.BUBBLE_WIDE,
                MotionType.BUBBLE_SURROUNDINGS,
                MotionType.LASER
            }.SelectRandom(new[] { 4, 5, 1, 5, 5, 4, 3 }) :
            (int)new[] {
                MotionType.CLUB,
                MotionType.CLUB_TREMOR,
                MotionType.BUBBLE,
                MotionType.BUBBLE_BURST,
                MotionType.BUBBLE_WIDE
            }.SelectRandom(new[] { 3, 2, 5, 5, 3 });
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
        var positionDiff = (nearTarget.position - position).Max(grappleDistance);
        var vertical = positionDiff.y.ToSign();
        var diff = Vector2.up * Mathf.Abs(positionDiff.magnitude / 2) * vertical;
        switch(motion)
        {
            case MotionType.CLUB:
            case MotionType.CLUB_LAST:
                yield return HeadingProperDestination(nearTarget.position,
                    maximumSpeed * 3,
                    tweakDifference: 2,
                    endDistance: grappleDistance,
                    concurrentProcess: () => {
                        Aiming(nearTarget.position);
                        Aiming(standardAimPosition, 0, 2);
                        var tweak = Mathf.Abs(nearTarget.position.x - position.x) * Vector2.up;
                        Aiming(nearTarget.position + tweak, 1);
                    });
                break;
            case MotionType.CLUB_TREMOR:
                yield return HeadingProperDestination(nearTarget.position,
                    maximumSpeed * 2,
                    directionTweak: 60,
                    tweakDifference: 2,
                    endDistance: grappleDistance,
                    concurrentProcess: () => {
                        Aiming(nearTarget.position);
                        Aiming(standardAimPosition, 0, 2);
                        Aiming(nearTarget.position, 1);
                    });
                break;
            case MotionType.CLUB_SPRINKLE:
                yield return HeadingProperDestination(standardPosition.Rotate(nearTarget.position, Random.Range(-90f, 90f)), maximumSpeed * 2, endDistance: grappleDistance, concurrentProcess: () => {
                    Aiming(nearTarget.position);
                    Aiming(standardAimPosition, 0, 2);
                    Aiming(nearTarget.position, 1);
                });
                yield return StoppingAction();
                break;
            case MotionType.BUBBLE:
                yield return AimingAction(nearTarget.position,
                    armIndex: 0,
                    aimingProcess: () => {
                        Aiming(nearTarget.position);
                        Aiming(standardAimPosition, 1, 2);
                    });
                yield return StoppingAction();
                break;
            case MotionType.BUBBLE_BURST:
                yield return AimingAction(nearTarget.position + (Vector2)(siteAlignment.ToRotation() * diff),
                    armIndex: 0,
                    aimingProcess: () => {
                        Thrust(nearTarget.position - position, targetSpeed: lowerSpeed);
                        Aiming(nearTarget.position);
                        Aiming(standardAimPosition, 1, 2);
                    });
                yield return StoppingAction();
                break;
            case MotionType.BUBBLE_WIDE:
                yield return AimingAction(nearTarget.position + (Vector2)(siteAlignment.ToRotation() * diff * -2),
                    armIndex: 0,
                    aimingProcess: () => {
                        Thrust(nearTarget.position - position, targetSpeed: lowerSpeed);
                        Aiming(nearTarget.position);
                        Aiming(standardAimPosition, 1, 2);
                    });
                yield return StoppingAction();
                break;
            case MotionType.BUBBLE_SURROUNDINGS:
                yield return HeadingProperDestination(nearTarget.position,
                    maximumSpeed,
                    directionTweak: 60,
                    tweakDifference: 2,
                    endDistance: grappleDistance,
                    concurrentProcess: () => {
                        Aiming(nearTarget.position);
                        Aiming(nearTarget.position, 0);
                        Aiming(standardAimPosition, 1, 2);
                    });
                yield return StoppingAction();
                break;
            case MotionType.LASER:
                yield return HeadingProperDestination(standardPosition.Rotate(nearTarget.position, Random.Range(-90f, 90f)), maximumSpeed * 2, endDistance: grappleDistance / 2, concurrentProcess: () => {
                    Aiming(nearTarget.position);
                    Aiming(nearTarget.position, 0);
                    Aiming(standardAimPosition, 1, 2);
                });
                yield return StoppingAction();
                break;
            case MotionType.SIDEWAYS:
                yield return HeadingDestination((Vector2.right * grappleDistance).Rotate(nearTarget.position, (position - nearTarget.position).ToAngle()),
                    maximumSpeed * 3,
                    concurrentProcess: () => {
                        Aiming(nearTarget.position);
                        Aiming(standardAimPosition, 0, 2);
                        Aiming(standardAimPosition, 1, 2);
                    });
                yield return StoppingAction(power: 2);
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
        var diffAlignment = armAlignments[0] - siteAlignment;
        var finishMotion = true;
        switch(motion)
        {
            case MotionType.CLUB:
            case MotionType.CLUB_LAST:
                SetFixedAlignment(nearTarget.position - position, true);
                club.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => !club.onAttack);
                yield return Wait(() => club.onAttack);
                yield return HeadingDestination(nearTarget.position, maximumSpeed * 2, grappleDistance, () => {
                    Aiming(nearTarget.position);
                    Aiming(standardAimPosition, 0, 2);
                    var tweak = Mathf.Abs(nearTarget.position.x - position.x) * Vector2.up;
                    Aiming(nearTarget.position + tweak, 1);
                });
                if(motion == MotionType.CLUB_LAST)
                {
                    yield return StoppingAction();
                    yield return Wait(() => club.canAction);
                }
                else if(new[] { true, false }.SelectRandom(seriousMode ? new[] { 5, 1 } : new[] { 1, 3 }))
                {
                    nextActionIndex = (int)new[] {
                        MotionType.CLUB_SPRINKLE,
                        MotionType.SIDEWAYS
                    }.SelectRandom(new[] { 5, 1 });
                    nextActionState = ActionPattern.ATTACK;
                    finishMotion = false;
                }
                else
                {
                    yield return StoppingAction();
                    yield return Wait(() => club.canAction);
                }
                break;
            case MotionType.CLUB_TREMOR:
                SetFixedAlignment(nearTarget.position - position, true);
                club.Action(Weapon.ActionType.NPC);
                yield return Wait(() => !club.onAttack);
                yield return Wait(() => club.onAttack);
                yield return HeadingDestination(nearTarget.position, maximumSpeed * 2, grappleDistance, () => {
                    Aiming(nearTarget.position);
                    Aiming(standardAimPosition, 0, 2);
                    Aiming(nearTarget.position, 1);
                });
                if(new[] { true, false }.SelectRandom(seriousMode ? new[] { 3, 1 } : new[] { 1, 1 }))
                {
                    nextActionIndex = (int)new[] {
                        MotionType.CLUB,
                        MotionType.SIDEWAYS
                    }.SelectRandom(new[] { 1, 1 });
                    nextActionState = ActionPattern.ATTACK;
                    finishMotion = false;
                }
                else
                {
                    yield return StoppingAction();
                    yield return Wait(() => club.canAction);
                }
                break;
            case MotionType.CLUB_SPRINKLE:
                SetFixedAlignment(nearTarget.position);
                club.Action(Weapon.ActionType.SINK);
                while(!club.onAttack || club.nowAction != Weapon.ActionType.SINK)
                {
                    Thrust(nearTarget.position - position, targetSpeed: (maximumSpeed + lowerSpeed) / 2);
                    Aiming(nearTarget.position);
                    Aiming(standardAimPosition, 0, 2);
                    Aiming(nearTarget.position, 1);
                    yield return Wait(1);
                }
                yield return HeadingDestination(nearTarget.position, maximumSpeed * 5, () => {
                    Aiming(nearTarget.position);
                    Aiming(standardAimPosition, 0, 2);
                    Aiming(nearTarget.position, 1);
                });
                yield return StoppingAction();
                yield return Wait(() => club.canAction);
                break;
            case MotionType.BUBBLE:
                SetFixedAlignment(0);
                bubble.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => bubble.canAction);
                nextActionIndex = (int)new[] {
                    MotionType.BUBBLE_BURST,
                    MotionType.BUBBLE_WIDE
                }.SelectRandom(new[] { 3, 1 });
                nextActionState = ActionPattern.AIMING;
                finishMotion = false;
                break;
            case MotionType.BUBBLE_BURST:
                yield return Wait(() => bubble.canAction);
                diffAlignment = armAlignments[0] - siteAlignment;
                Debug.Log($"{diffAlignment} = {armAlignments[0]} - {siteAlignment}");
                SetFixedAlignment(0);
                bubble.Action(Weapon.ActionType.NOMAL, 0.1f);

                yield return AimingAction(() => nearTarget.position, armIndex: 0, siteSpeedTweak: 2, aimingProcess: () => Aiming(nearTarget.position));
                yield return Wait(() => bubble.canAction);
                SetFixedAlignment(0);
                bubble.Action(Weapon.ActionType.NPC, 0.1f);

                yield return AimingAction(() => nearTarget.position - diffAlignment, armIndex: 0, siteSpeedTweak: 2, aimingProcess: () => Aiming(nearTarget.position));
                yield return Wait(() => bubble.canAction);
                SetFixedAlignment(0);
                bubble.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => bubble.canAction);
                finishMotion = false;
                if(new[] { true, false }.SelectRandom(seriousMode ? new[] { 3, 1 } : new[] { 2, 1 }))
                {
                    nextActionIndex = (int)new[] {
                        MotionType.CLUB_LAST,
                        MotionType.SIDEWAYS,
                        MotionType.BUBBLE_WIDE
                    }.SelectRandom(new[] { 3, 1, 1 });
                    nextActionState = ActionPattern.AIMING;
                }
                break;
            case MotionType.BUBBLE_WIDE:
                yield return Wait(() => bubble.canAction);
                diffAlignment = armAlignments[0] - siteAlignment;
                Debug.Log($"{diffAlignment} = {armAlignments[0]} - {siteAlignment}");
                SetFixedAlignment(0);
                bubble.Action(Weapon.ActionType.NPC, 0.1f);

                yield return AimingAction(() => nearTarget.position + diffAlignment / 2, armIndex: 0, siteSpeedTweak: 2, aimingProcess: () => Aiming(nearTarget.position));
                yield return Wait(() => bubble.canAction);
                SetFixedAlignment(0);
                bubble.Action(Weapon.ActionType.NOMAL, 0.1f);

                yield return AimingAction(() => nearTarget.position, armIndex: 0, siteSpeedTweak: 2, aimingProcess: () => Aiming(nearTarget.position));
                yield return Wait(() => bubble.canAction);
                SetFixedAlignment(0);
                bubble.Action(Weapon.ActionType.NOMAL, 0.1f);

                yield return AimingAction(() => nearTarget.position - diffAlignment / 2, armIndex: 0, siteSpeedTweak: 2, aimingProcess: () => Aiming(nearTarget.position));
                yield return Wait(() => bubble.canAction);
                SetFixedAlignment(0);
                bubble.Action(Weapon.ActionType.NOMAL, 0.1f);

                yield return AimingAction(() => nearTarget.position - diffAlignment, armIndex: 0, siteSpeedTweak: 2, aimingProcess: () => Aiming(nearTarget.position));
                yield return Wait(() => bubble.canAction);
                SetFixedAlignment(0);
                bubble.Action(Weapon.ActionType.NPC);
                finishMotion = false;
                if(new[] { true, false }.SelectRandom(seriousMode ? new[] { 5, 1 } : new[] { 1, 3 }))
                {
                    nextActionIndex = (int)new[] {
                        MotionType.CLUB_LAST,
                        MotionType.BUBBLE_SURROUNDINGS,
                        MotionType.CLUB_SPRINKLE
                    }.SelectRandom(seriousMode ? new[] { 1, 3, 3 } : new[] { 1, 0, 0 });
                    nextActionState = ActionPattern.ATTACK;
                }
                else
                {
                    yield return Wait(() => bubble.canAction);
                }
                break;
            case MotionType.BUBBLE_SURROUNDINGS:
                var signTweak = new[] { 1, -1 }.SelectRandom();
                var duration = interval * Random.Range(1, shipLevel) * 2;
                for(int time = 0; time < duration; time++)
                {
                    Thrust((signTweak * 60f).ToRotation() * (nearTarget.position - position));
                    var rotationTweak = Easing.quadratic.Out(signTweak * 360, time, duration - 1).ToRotation();
                    var direction = (Vector2)(rotationTweak * (nearTarget.position - position));
                    if(bubble.canAction)
                    {
                        SetFixedAlignment(position + direction);
                        bubble.Action(new[] { Weapon.ActionType.NOMAL, Weapon.ActionType.NPC }.SelectRandom(), 0.1f);
                    }
                    Aiming(nearTarget.position);
                    Aiming(position + direction, 0, 2);
                    Aiming(standardAimPosition, 1, 2);
                    yield return Wait(1);
                }
                if(new[] { true, false }.SelectRandom(seriousMode ? new[] { 5, 1 } : new[] { 3, 1 }))
                {
                    nextActionIndex = (int)new[] {
                        MotionType.CLUB,
                        MotionType.BUBBLE_WIDE,
                        MotionType.LASER
                    }.SelectRandom(new[] { 3, 3, 1 });
                    nextActionState = ActionPattern.AIMING;
                    finishMotion = false;
                }
                else
                {
                    yield return StoppingAction();
                    yield return Wait(() => bubble.canAction);
                }
                break;
            case MotionType.LASER:
                bubble.Action(Weapon.ActionType.SINK, 0.1f);
                var originTargetPosition = nearTarget.position;
                for(int time = 0; time < interval; time++)
                {
                    var startAngle = nWidthPositive * 60f;
                    var rotationTweak = Easing.quadratic.In(nWidthPositive * -360, time, interval - 1) + startAngle;
                    var direction = (Vector2)(rotationTweak.ToRotation() * (originTargetPosition - position));
                    var startDirection = (Vector2)(startAngle.ToRotation() * (originTargetPosition - position));
                    SetFixedAlignment(position + direction);
                    Aiming(position + startDirection, 0);
                    yield return Wait(1);
                }
                yield return Wait(() => bubble.onAttack);
                var timeLimit = interval * (seriousMode ? 4 : 6);
                for(int time = 0; time < timeLimit; time++)
                {
                    var startAngle = nWidthPositive * 60f;
                    var rotationTweak = Easing.quadratic.In(nWidthPositive * -360, time, timeLimit - 1) + startAngle;
                    var direction = (Vector2)(rotationTweak.ToRotation() * (originTargetPosition - position));
                    Aiming(position + direction, 0);
                    Aiming(standardAimPosition, 1);
                    yield return Wait(1);
                }
                yield return Wait(() => bubble.canAction);
                break;
            case MotionType.SIDEWAYS:
                var directionTweak = new[] { 1, -1 }.SelectRandom();
                for(int time = 0; time < interval * 2; time++)
                {
                    Thrust((directionTweak * 100f).ToRotation() * (nearTarget.position - position));
                    Aiming(nearTarget.position);
                    yield return Wait(1);
                }
                nextActionIndex = (int)new[] {
                        MotionType.CLUB_TREMOR,
                        MotionType.CLUB,
                        MotionType.BUBBLE_BURST,
                        MotionType.BUBBLE_WIDE
                    }.SelectRandom(new[] { 1, 5, 1, 3 });
                nextActionState = ActionPattern.AIMING;
                finishMotion = false;
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

    protected override float grappleDistance => base.grappleDistance / 2;
}
