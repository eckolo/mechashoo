using UnityEngine;
using System.Collections;

public class Giduweke : Npc
{
    Weapon nife => allWeapons[0];
    Weapon funger => allWeapons[1];

    enum MotionType { NIFE, FUNGER }
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
        nextActionIndex = ((nearTarget.position - position).magnitude < gunDistance / 2).ToInt();
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
            case MotionType.NIFE:
                yield return StoppingAction();
                yield return HeadingDestination(nearTarget.position, maximumSpeed, grappleDistance, () => {
                    Aiming(nearTarget.position);
                    Aiming(nearTarget.position, 0);
                    Aiming(standardAimPosition, 1);
                });
                break;
            case MotionType.FUNGER:
                yield return AimingAction(() => nearTarget.position, armIndex: 1, aimingProcess: () => {
                    Aiming(nearTarget.position);
                    Aiming(standardAimPosition, 0);
                    Thrust(nearTarget.position - position);
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
        switch(motion)
        {
            case MotionType.NIFE:
                nife.Action(Weapon.ActionType.NOMAL);
                yield return StoppingAction();
                yield return Wait(() => nife.canAction);
                break;
            case MotionType.FUNGER:
                funger.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => funger.canAction);
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
