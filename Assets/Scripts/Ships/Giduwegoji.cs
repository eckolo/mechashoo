using UnityEngine;
using System.Collections;

public class Giduwegoji : Npc
{
    Weapon assaulter => allWeapons[0];
    Weapon nife => allWeapons[1];

    enum MotionType { ASSAULTER, NIFE }
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
            var digree = GetProperPosition(nearTarget, 90);
            var speed = baseDegree.ToVector(digree) + digree;
            Thrust(speed, reactPower, maximumSpeed);
            SetBaseAimingAll();
        });
        nextActionIndex = ((nearTarget.position - position).magnitude < gunDistance).ToInt();
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
            case MotionType.NIFE:
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
                assaulter.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => assaulter.canAction);
                break;
            case MotionType.NIFE:
                nife.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => nife.onAttack);
                yield return StoppingAction();
                yield return Wait(() => nife.canAction);
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
}