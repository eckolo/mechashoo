using UnityEngine;
using System.Collections;

public class Giduwegoji : Npc
{
    Weapon assaulter => allWeapons[0];
    Weapon sword => allWeapons[1];

    enum MotionType { ASSAULTER, SWORD, SWORD_CHARGE }
    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionMove(int actionNum)
    {
        nextActionState = ActionPattern.AIMING;
        yield return HeadingProperDestination(position + GetProperPosition(nearTarget, 90),
            maximumSpeed,
            endDistance: grappleDistance,
            concurrentProcess: () => {
                Aiming(nearTarget.position);
                SetBaseAimingAll();
            });
        nextActionIndex = (int)new[] {
            MotionType.ASSAULTER,
            MotionType.SWORD,
            MotionType.SWORD_CHARGE
        }.SelectRandom(new[] { 5, 5, 1 });
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
                yield return AimingAction(() => nearTarget.position, armIndex: 0, aimingProcess: () => {
                    Aiming(nearTarget.position);
                    Aiming(standardAimPosition, 1);
                    Thrust(nearTarget.position - position);
                });
                yield return StoppingAction();
                break;
            case MotionType.SWORD:
            case MotionType.SWORD_CHARGE:
                yield return StoppingAction();
                yield return HeadingDestination(nearTarget.position, maximumSpeed, grappleDistance, () => {
                    Aiming(nearTarget.position);
                    Aiming(nearTarget.position, 1);
                    Aiming(standardAimPosition, 0);
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
        nextActionState = ActionPattern.MOVE;
        var motion = actionNum.Normalize<MotionType>();
        switch(motion)
        {
            case MotionType.ASSAULTER:
                {
                    yield return StoppingAction();
                    yield return Wait(() => assaulter.canAction);
                    var distance = (nearTarget.position - position).magnitude;
                    assaulter.Action(distance > gunDistance / 2 ? Weapon.ActionType.NOMAL : Weapon.ActionType.SINK);
                    yield return Wait(() => assaulter.canAction);
                }
                break;
            case MotionType.SWORD:
                {
                    yield return Wait(() => sword.canAction);
                    sword.Action(Weapon.ActionType.NOMAL);
                    yield return StoppingAction();
                    yield return Wait(() => sword.canAction);
                }
                break;
            case MotionType.SWORD_CHARGE:
                {
                    yield return Wait(() => sword.canAction);
                    yield return StoppingAction();
                    sword.Action(Weapon.ActionType.SINK);
                    var destination = nearTarget.position;
                    while(!sword.onFollowThrough)
                    {
                        Thrust(destination - position);
                        Aiming(nearTarget.position);
                        Aiming(nearTarget.position, 1);
                        Aiming(standardAimPosition, 0);
                        yield return Wait(1);
                    }
                    yield return StoppingAction();
                    yield return Wait(() => sword.canAction);
                }
                break;
            default:
                break;
        }

        if(onTheWay && actionCount++ >= shipLevel) nextActionState = ActionPattern.ESCAPE;
        for(int time = 0; time < interval; time++)
        {
            Aiming(nearTarget.position);
            SetBaseAimingAll();
            ThrustStop();
            yield return Wait(1);
        }
        yield break;
    }
    int actionCount = 0;
    protected override float grappleDistance => base.grappleDistance * 1.5f;
}