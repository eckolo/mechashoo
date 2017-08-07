using UnityEngine;
using System.Collections;

public class Legaseji : Npc
{
    Weapon assaulter => allWeapons[0];
    Weapon rifle => allWeapons[1];

    enum MotionType { ASSAULTER, RIFLE }
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
            MotionType.RIFLE
        }.SelectRandom(new[] { 2, 3 });
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
            case MotionType.RIFLE:
                yield return AimingAction(() => nearTarget.position, armIndex: 1, aimingProcess: () => {
                    Aiming(nearTarget.position);
                    Aiming(standardAimPosition, 0);
                    Thrust(nearTarget.position - position);
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
        var finishMotion = true;
        switch(motion)
        {
            case MotionType.ASSAULTER:
                yield return Wait(() => assaulter.canAction);
                assaulter.Action(Weapon.ActionType.NOMAL);
                yield return Wait(() => assaulter.canAction);
                break;
            case MotionType.RIFLE:
                yield return Wait(() => rifle.canAction);
                yield return StoppingAction();
                rifle.Action(Weapon.ActionType.NOMAL);
                if(new[] { true, false }.SelectRandom(new[] { 5 + (int)shipLevel, 1 }))
                {
                    nextActionIndex = (int)new[] {
                        MotionType.ASSAULTER,
                        MotionType.RIFLE
                    }.SelectRandom(new[] { 3, 1 + (int)shipLevel });
                    nextActionState = ActionPattern.AIMING;
                    finishMotion = false;
                }
                else
                {
                    yield return Wait(() => rifle.canAction);
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
