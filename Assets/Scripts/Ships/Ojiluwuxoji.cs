using UnityEngine;
using System.Collections;

public class Ojiluwuxoji : Npc
{
    Weapon assaulter => allWeapons[0];
    Weapon grenade => allWeapons[1];

    enum MotionType
    {
        ASSAULTER,
        ASSAULTER_STEP,
        GRENADE,
        GRENADE_BIG,
        SIDE_STEP
    }
    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        var baseDegree = normalCourse;
        yield return AimingAction(() => nearTarget.position, interval * 2, aimingProcess: () => {
            var digree = GetProperPosition(nearTarget);
            var speed = baseDegree.ToVector(digree) + digree;
            Thrust(speed, reactPower, maximumSpeed);
            SetBaseAimingAll();
        });
        nextActionIndex = (int)new[] {
            MotionType.ASSAULTER,
            MotionType.ASSAULTER_STEP,
            MotionType.GRENADE,
            MotionType.GRENADE_BIG,
            MotionType.SIDE_STEP
        }.SelectRandom(new[] { 5, 3, 5, 1, 3 });
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
            case MotionType.ASSAULTER:
                yield return AimingAction(nearTarget.position, armIndex: 0, aimingProcess: () => {
                    Aiming(nearTarget.position);
                    Aiming(standardAimPosition, 1);
                    ThrustStop();
                });
                break;
            case MotionType.ASSAULTER_STEP:
                {
                    var repeat = new[] { 1, 2, 3 }.SelectRandom(new[] { 1, 3, 1 });
                    for(int count = 0; count < repeat; count++)
                    {
                        yield return StoppingAction();
                        var direction = new[] { 90f, -90f }.SelectRandom();
                        for(int time = 0; time < interval; time++)
                        {
                            if(isDestroied) yield break;
                            var directionTweak = (Vector2)(direction.ToRotation() * (position - nearTarget.position));
                            var destination = (position + directionTweak).Within(fieldLowerLeft, fieldUpperRight);
                            Thrust(destination - position, reactPower * 2, maximumSpeed);
                            Aiming(nearTarget.position);
                            Aiming(nearTarget.position, 0);
                            Aiming(standardAimPosition, 1);
                            yield return Wait(1);
                        }
                    }
                }
                break;
            case MotionType.GRENADE:
                yield return AimingAction(() => nearTarget.position, armIndex: 1, aimingProcess: () => {
                    Aiming(nearTarget.position);
                    Aiming(standardAimPosition, 0);
                    Thrust(nearTarget.position - position);
                });
                yield return StoppingAction();
                break;
            case MotionType.GRENADE_BIG:
                for(int time = 0; time < interval; time++)
                {
                    if(isDestroied) yield break;
                    var direction = nearTarget.position - position;
                    if(direction.magnitude > grappleDistance) Thrust(direction, targetSpeed: maximumSpeed);
                    else ThrustStop();
                    Aiming(nearTarget.position);
                    Aiming(standardAimPosition, 0);
                    Aiming(nearTarget.position, 1);
                    yield return Wait(1);
                }
                yield return StoppingAction();
                break;
            case MotionType.SIDE_STEP:
                for(int time = 0; time < interval; time++)
                {
                    if(isDestroied) yield break;
                    var direction = nearTarget.position - position;
                    if(direction.magnitude > gunDistance) Thrust(direction, targetSpeed: maximumSpeed);
                    else ThrustStop();
                    SetBaseAimingAll();
                    yield return Wait(1);
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
    /// <returns></returns>
    protected override IEnumerator MotionAttack(int actionNum)
    {
        nextActionState = ActionPattern.MOVE;
        var motion = actionNum.Normalize<MotionType>();
        var finishMotion = true;
        switch(motion)
        {
            case MotionType.ASSAULTER:
                SetFixedAlignment(nearTarget.position);
                yield return Wait(() => assaulter.canAction);
                assaulter.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => assaulter.canAction);
                break;
            case MotionType.ASSAULTER_STEP:
                SetFixedAlignment(nearTarget.position);
                yield return Wait(() => assaulter.canAction);
                assaulter.Action(Weapon.ActionType.NOMAL);
                yield return StoppingAction();
                yield return Wait(() => assaulter.canAction);
                break;
            case MotionType.GRENADE:
                SetFixedAlignment(1);
                yield return Wait(() => grenade.canAction);
                grenade.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => grenade.canAction);
                break;
            case MotionType.GRENADE_BIG:
                {
                    yield return Wait(() => grenade.canAction);
                    grenade.Action(Weapon.ActionType.SINK);
                    var targetPosition = nearTarget.position;
                    SetFixedAlignment(targetPosition);
                    while(grenade.onFollowThrough)
                    {
                        Aiming(targetPosition);
                        Aiming(standardAimPosition, 0);
                        Aiming(targetPosition, 1);
                        Thrust(-siteAlignment);
                        yield return Wait(1);
                    }
                    yield return StoppingAction();
                    yield return Wait(() => grenade.canAction);
                }
                break;
            case MotionType.SIDE_STEP:
                {
                    var direction = new[] { 90f, -90f }.SelectRandom();
                    for(int time = 0; time < interval; time++)
                    {
                        if(isDestroied) yield break;
                        var directionTweak = (Vector2)(direction.ToRotation() * (position - nearTarget.position));
                        var destination = (position + directionTweak).Within(fieldLowerLeft, fieldUpperRight);
                        Thrust(destination - position, reactPower, maximumSpeed);
                        Aiming(nearTarget.position);
                        SetBaseAimingAll();
                        yield return Wait(1);
                    }
                    yield return StoppingAction();
                    nextActionIndex = (int)new[] { MotionType.ASSAULTER, MotionType.GRENADE }.SelectRandom();
                    nextActionState = ActionPattern.AIMING;
                    finishMotion = false;
                }
                break;
            default:
                break;
        }

        if(onTheWay && finishMotion && actionCount++ >= shipLevel) nextActionState = ActionPattern.ESCAPE;
        for(int time = 0; finishMotion && time < interval; time++)
        {
            Aiming(nearTarget.position);
            SetBaseAimingAll();
            ThrustStop();
            yield return Wait(1);
        }
        yield break;
    }
    int actionCount = 0;
}
