using UnityEngine;
using System.Collections;

public class Ueugazi : Boss
{
    Weapon gust => allWeapons[0];
    Weapon grenade => allWeapons[1];
    Weapon explosionFlont => allWeapons[2];
    Weapon explosionBack => allWeapons[3];

    enum MotionType
    {
        SUCTION,
        SUCTION_WIDE,
        BLOWING,
        BLOWING_WIDE,
        GRENADE,
        GRENADE_BURST,
        GRENADE_HUGE,
        EXPLOSION,
        EXPLOSION_RUSH,
        EXPLOSION_HUGE
    }

    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns>コルーチン</returns>
    protected override IEnumerator MotionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        for(int time = 0; time < interval * 2; time++)
        {
            var direction = nearTarget.position - position;
            if(direction.magnitude > grappleDistance) Thrust(direction, targetSpeed: maximumSpeed);
            Aiming(nearTarget.position);
            SetBaseAimingAll();
            yield return Wait(1);
        }
        yield return StoppingAction();
        var diff = (position + armRoot - nearTarget.position + nearTarget.nowSpeed).magnitude;
        nextActionIndex = seriousMode ?
            (int)new[] {
                MotionType.SUCTION,
                MotionType.SUCTION_WIDE,
                MotionType.BLOWING_WIDE,
                MotionType.GRENADE_BURST,
                MotionType.GRENADE_HUGE,
                MotionType.EXPLOSION,
                MotionType.EXPLOSION_RUSH,
                MotionType.EXPLOSION_HUGE
            }.SelectRandom(diff > grappleDistance ?
                new[] { 6, 10, 3, 4, 3, 0, 0, 0 } :
                new[] { 6, 10, 1, 2, 3, 1, 4, 5 }) :
            (int)new[] {
                MotionType.SUCTION,
                MotionType.SUCTION_WIDE,
                MotionType.BLOWING_WIDE,
                MotionType.GRENADE,
                MotionType.GRENADE_BURST,
                MotionType.GRENADE_HUGE,
                MotionType.EXPLOSION,
                MotionType.EXPLOSION_RUSH,
                MotionType.EXPLOSION_HUGE
            }.SelectRandom(diff > grappleDistance ?
                new[] { 8, 6, 3, 5, 3, 3, 2, 1, 1 } :
                new[] { 8, 6, 1, 1, 2, 1, 5, 4, 1 });
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
        var positionDiff = nearTarget.position - position;
        var vertical = positionDiff.y.ToSign();
        if(vertical == 0) vertical = 1;
        var diff = Vector2.up * Mathf.Max(Mathf.Abs(positionDiff.magnitude), grappleDistance) * vertical;

        switch(motion)
        {
            case MotionType.SUCTION:
                {
                    yield return AimingAction(nearTarget.position, armIndex: 0, aimingProcess: () => {
                        Thrust(approachPosition - position, targetSpeed: lowerSpeed);
                        Aiming(nearTarget.position);
                        Aiming(standardAimPosition, armIndex: 1, siteSpeedTweak: 2);
                    });
                    yield return StoppingAction();
                }
                break;
            case MotionType.SUCTION_WIDE:
                {
                    var tweak = (Vector2)(siteAlignment.ToRotation() * (seriousMode ? diff * 2 : diff));
                    yield return AimingAction(nearTarget.position + tweak, armIndex: 0, aimingProcess: () => {
                        Thrust(approachPosition - position, targetSpeed: lowerSpeed);
                        Aiming(nearTarget.position);
                        Aiming(standardAimPosition, armIndex: 1, siteSpeedTweak: 2);
                    });
                    yield return StoppingAction();
                }
                break;
            case MotionType.BLOWING:
                {
                    var tweak = (Vector2)(siteAlignment.ToRotation() * diff * vertical);
                    yield return AimingAction(nearTarget.position + tweak, armIndex: 0, aimingProcess: () => {
                        Thrust(approachPosition - position, targetSpeed: lowerSpeed);
                        Aiming(nearTarget.position);
                        Aiming(standardAimPosition, armIndex: 1, siteSpeedTweak: 2);
                    });
                    yield return StoppingAction();
                }
                break;
            case MotionType.BLOWING_WIDE:
                {
                    var tweak = (Vector2)(siteAlignment.ToRotation() * (seriousMode ? diff * 2 : diff) * -1);
                    yield return AimingAction(nearTarget.position + tweak, armIndex: 0, aimingProcess: () => {
                        Thrust(approachPosition - position, targetSpeed: lowerSpeed);
                        Aiming(nearTarget.position);
                        Aiming(standardAimPosition, armIndex: 1, siteSpeedTweak: 2);
                    });
                    yield return StoppingAction();
                }
                break;
            case MotionType.GRENADE:
            case MotionType.GRENADE_BURST:
            case MotionType.GRENADE_HUGE:
                {
                    yield return AimingAction(nearTarget.position, armIndex: 1, aimingProcess: () => {
                        Thrust(approachPosition - position, targetSpeed: lowerSpeed);
                        Aiming(nearTarget.position);
                        Aiming(standardAimPosition, armIndex: 0, siteSpeedTweak: 2);
                    });
                    yield return StoppingAction();
                }
                break;
            case MotionType.EXPLOSION:
            case MotionType.EXPLOSION_RUSH:
                {
                    var approachDistance = spriteSize.y / 2;
                    var destination = approachPosition + Vector2.up * approachDistance;
                    yield return HeadingDestination(destination, maximumSpeed, approachDistance, () => {
                        Aiming(nearTarget.position);
                        SetBaseAimingAll();
                    });
                    yield return StoppingAction();
                }
                break;
            case MotionType.EXPLOSION_HUGE:
                {
                    var approachDistance = spriteSize.y / 2;
                    var destination = nearTarget.position + Vector2.up * approachDistance;
                    yield return HeadingDestination(destination, maximumSpeed, approachDistance, () => {
                        Aiming(nearTarget.position);
                        SetBaseAimingAll();
                    });
                    yield return StoppingAction();
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
            case MotionType.SUCTION:
                {
                    for(int time = 0; time < interval; time++)
                    {
                        ThrustStop();
                        Aiming(nearTarget.position, armIndex: 0, siteSpeedTweak: seriousMode ? 1 : 0.1f);
                        gust.Action(Weapon.ActionType.SINK);
                        yield return Wait(1);
                    }
                    yield return Wait(() => gust.canAction);
                    var diff = (position + armRoot - nearTarget.position + nearTarget.nowSpeed).magnitude;
                    nextActionIndex = diff > grappleDistance ?
                        (int)new[] {
                            MotionType.BLOWING,
                            MotionType.BLOWING_WIDE,
                            MotionType.GRENADE_HUGE,
                        }.SelectRandom(seriousMode ? new[] { 3, 2, 1 } : new[] { 3, 1, 0 }) :
                        (int)new[] {
                            MotionType.EXPLOSION,
                            MotionType.GRENADE_HUGE,
                            MotionType.EXPLOSION_RUSH,
                            MotionType.EXPLOSION_HUGE
                        }.SelectRandom(seriousMode ? new[] { 1, 1, 3, 5 } : new[] { 2, 0, 1, 0 });
                    nextActionState = diff > grappleDistance ? ActionPattern.AIMING : ActionPattern.ATTACK;
                    finishMotion = false;
                }
                break;
            case MotionType.SUCTION_WIDE:
                {
                    var diffAlignment = armAlignments[0] - (nearTarget.position - position);
                    yield return AimingAction(nearTarget.position - diffAlignment,
                        armIndex: 0,
                        siteSpeedTweak: seriousMode ? 2 : 1,
                        aimingProcess: () => {
                            ThrustStop();
                            gust.Action(Weapon.ActionType.SINK);
                        });
                    yield return Wait(() => gust.canAction);
                    var diff = (position + armRoot - nearTarget.position + nearTarget.nowSpeed).magnitude;
                    nextActionIndex = diff > grappleDistance ?
                        (int)new[] {
                            MotionType.BLOWING,
                            MotionType.BLOWING_WIDE,
                            MotionType.GRENADE_HUGE
                        }.SelectRandom(seriousMode ? new[] { 2, 2, 1 } : new[] { 1, 1, 0 }) :
                        (int)new[] {
                            MotionType.GRENADE_HUGE,
                            MotionType.EXPLOSION_RUSH,
                            MotionType.EXPLOSION_HUGE
                        }.SelectRandom(seriousMode ? new[] { 1, 3, 5 } : new[] { 1, 1, 0 });
                    nextActionState = diff > grappleDistance ? ActionPattern.AIMING : ActionPattern.ATTACK;
                    finishMotion = false;
                }
                break;
            case MotionType.BLOWING:
                {
                    var diffAlignment = armAlignments[0] - (nearTarget.position - position);
                    var chargePosition = nearTarget.position + diffAlignment;
                    var targetPosition = nearTarget.position;
                    for(int time = 0; time < interval / 2; time++)
                    {
                        ThrustStop();
                        gust.Action(Weapon.ActionType.NOMAL);
                        Aiming(chargePosition, armIndex: 0, siteSpeedTweak: 0.3f);
                        yield return Wait(1);
                    }
                    yield return AimingAction(targetPosition,
                        armIndex: 0,
                        aimingProcess: () => {
                            ThrustStop();
                            gust.Action(Weapon.ActionType.NPC);
                        });
                    for(int time = 0; time < interval; time++)
                    {
                        ThrustStop();
                        gust.Action(Weapon.ActionType.NPC);
                        yield return Wait(1);
                    }
                    yield return Wait(() => gust.canAction);
                }
                break;
            case MotionType.BLOWING_WIDE:
                {
                    var diffAlignment = armAlignments[0] - (nearTarget.position - position);
                    var wrappingPosition = nearTarget.position - diffAlignment;
                    var targetPosition = nearTarget.position + diffAlignment;
                    yield return AimingAction(wrappingPosition,
                        armIndex: 0,
                        siteSpeedTweak: seriousMode ? 2 : 1,
                        aimingProcess: () => {
                            ThrustStop();
                            gust.Action(Weapon.ActionType.NOMAL);
                        });
                    if(new[] { true, false }.SelectRandom(seriousMode ? new[] { 1, 1 } : new[] { 0, 1 }))
                    {
                        nextActionIndex = (int)new[] {
                            MotionType.BLOWING,
                            MotionType.GRENADE_BURST,
                            MotionType.GRENADE_HUGE
                        }.SelectRandom(new[] { 4, 2, 3 });
                        nextActionState = ActionPattern.AIMING;
                        finishMotion = false;
                    }
                    else
                    {
                        yield return AimingAction(targetPosition,
                            armIndex: 0,
                        siteSpeedTweak: seriousMode ? 1.5f : 0.8f,
                            aimingProcess: () => {
                                ThrustStop();
                                gust.Action(Weapon.ActionType.NPC);
                            });
                        yield return Wait(() => gust.canAction);
                    }
                }
                break;
            case MotionType.GRENADE:
                {
                    yield return Wait(() => grenade.canAction);
                    grenade.Action(Weapon.ActionType.NOMAL);
                    yield return Wait(() => grenade.canAction);
                }
                break;
            case MotionType.GRENADE_BURST:
                {
                    yield return Wait(() => grenade.canAction);
                    grenade.Action(Weapon.ActionType.NOMAL, setActionDelayTweak: 0.5f);
                    yield return Wait(() => grenade.canAction);
                    yield return AimingAction(nearTarget.position, armIndex: 1);
                    grenade.Action(Weapon.ActionType.NOMAL, setActionDelayTweak: 0.5f);
                    yield return Wait(() => grenade.canAction);
                    if(seriousMode)
                    {
                        yield return AimingAction(() => nearTarget.position + nearTarget.nowSpeed,
                            armIndex: 1,
                            siteSpeedTweak: 2);
                    }
                    else
                    {
                        yield return AimingAction(nearTarget.position, armIndex: 1, siteSpeedTweak: 2);
                    }
                    grenade.Action(Weapon.ActionType.NOMAL);
                    yield return Wait(() => grenade.canAction);
                }
                break;
            case MotionType.GRENADE_HUGE:
                {
                    yield return Wait(() => grenade.canAction);
                    var diff = (position + armRoot - nearTarget.position).magnitude;
                    grenade.Action(diff > gunDistance ? Weapon.ActionType.SINK : Weapon.ActionType.NPC);
                    while(!grenade.onAttack)
                    {
                        Aiming(nearTarget.position, armIndex: 1, siteSpeedTweak: seriousMode ? 0.5f : 0.1f);
                        yield return Wait(1);
                    }
                    yield return Wait(() => grenade.canAction);
                }
                break;
            case MotionType.EXPLOSION:
                {
                    if(nWidthPositive * targetSign < 0)
                    {
                        yield return Wait(() => explosionFlont.canAction);
                        explosionFlont.Action(Weapon.ActionType.NOMAL);
                        yield return Wait(() => explosionFlont.canAction);
                    }
                    else
                    {
                        yield return Wait(() => explosionBack.canAction);
                        explosionBack.Action(Weapon.ActionType.NOMAL);
                        yield return Wait(() => explosionBack.canAction);
                    }
                }
                break;
            case MotionType.EXPLOSION_RUSH:
                {
                    yield return Wait(() => explosionFlont.canAction);
                    explosionFlont.Action(Weapon.ActionType.NOMAL, setActionDelayTweak: 0.5f);
                    while(!explosionFlont.onFollowThrough)
                    {
                        Thrust(nearTarget.position - position, targetSpeed: lowerSpeed);
                        yield return Wait(1);
                    }
                    explosionBack.Action(Weapon.ActionType.NOMAL, setActionDelayTweak: 0.5f);
                    while(!explosionBack.onFollowThrough)
                    {
                        Thrust(nearTarget.position - position, targetSpeed: lowerSpeed);
                        yield return Wait(1);
                    }
                    if(new[] { true, false }.SelectRandom(seriousMode ? new[] { 3, 1 } : new[] { 0, 1 }))
                    {
                        nextActionIndex = (int)MotionType.EXPLOSION_HUGE;
                        nextActionState = ActionPattern.ATTACK;
                        finishMotion = false;
                    }
                    else
                    {
                        explosionFlont.Action(Weapon.ActionType.NOMAL);
                        while(!explosionFlont.onFollowThrough)
                        {
                            Thrust(nearTarget.position - position, targetSpeed: lowerSpeed);
                            yield return Wait(1);
                        }
                        explosionBack.Action(Weapon.ActionType.NOMAL);
                        while(!explosionBack.onFollowThrough)
                        {
                            Thrust(nearTarget.position - position, targetSpeed: lowerSpeed);
                            yield return Wait(1);
                        }
                        yield return StoppingAction();
                        yield return Wait(() => explosionBack.canAction);
                    }
                }
                break;
            case MotionType.EXPLOSION_HUGE:
                {
                    yield return Wait(() => explosionFlont.canAction && explosionBack.canAction);
                    explosionFlont.Action(Weapon.ActionType.SINK);
                    explosionBack.Action(Weapon.ActionType.SINK);
                    while(!explosionFlont.onAttack && !explosionBack.onAttack)
                    {
                        var approachDistance = spriteSize.y / 2;
                        var destination = nearTarget.position + Vector2.up * approachDistance;
                        Thrust(destination - position, targetSpeed: maximumSpeed);
                        yield return Wait(1);
                    }
                    yield return StoppingAction();
                    yield return Wait(() => explosionFlont.canAction && explosionBack.canAction);
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

    protected override float grappleDistance => arms[0].tipLength + (grenade?.nowLengthVector.magnitude ?? 0);
    protected override float gunDistance => viewSize.x / 2;
    protected override Vector2 approachPosition => nearTarget.position + Vector2.right * spriteSize.x * targetSign / 2;

    protected override float siteSpeed => base.siteSpeed - palamates.baseSiteSpeed * shipLevel;
}
