using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 小型蟹機クラス
/// </summary>
public class Fusefizo : Npc
{
    /// <summary>
    /// 移動時行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionMove(int actionNum)
    {
        var destination = new[] { -90, 0, 90 }.SelectRandom(new[] { 3, 2, 3 });

        nextActionState = ActionPattern.AIMING;
        var targetPosition = nearTarget.position - position;
        yield return AimingAction(() => nearTarget.position,
            interval * 2,
            aimingProcess: () => Thrust(GetProperPosition(targetPosition, destination), reactPower, maximumSpeed));
        nextActionIndex = !onTheWay ? new[] { 0, 1 }.SelectRandom(new[] { 2, 3 }) : 1;

        yield break;
    }
    /// <summary>
    /// 照準操作行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionAiming(int actionNum)
    {
        var destination = new[] { -90, 0, 90 }.SelectRandom(new[] { 3, 2, 3 });

        nextActionState = ActionPattern.ATTACK;
        var alreadyAttack = false;
        yield return AimingAction(() => nearTarget.position,
            !onTheWay ? interval : interval * 2,
            aimingProcess: () => {
                var speed = actionNum != 0 ? (lowerSpeed + maximumSpeed) / 2 : maximumSpeed;
                Thrust(GetProperPosition(nearTarget, destination), reactPower, speed);
                if(speed >= maximumSpeed) return;
                if((nearTarget.position - position).magnitude < properDistance)
                {
                    if(!alreadyAttack)
                    {
                        PinchAttack();
                        alreadyAttack = true;
                    }
                }
                else if(alreadyAttack) alreadyAttack = false;
            });

        yield break;
    }
    /// <summary>
    /// 攻撃行動
    /// </summary>
    /// <param name="actionNum">行動パターン識別番号</param>
    /// <returns></returns>
    protected override IEnumerator MotionAttack(int actionNum)
    {
        var destination = new[] { -90, 0, 90 }.SelectRandom(new[] { 3, 2, 3 });
        nextActionState = !onTheWay ? ActionPattern.MOVE : ActionPattern.ESCAPE;
        if(actionNum != 0)
        {
            yield return Wait(() => allWeapons.Any(weapon => weapon.canAction));
            SwingAttack();
            var alreadyAttack = false;
            yield return AimingAction(() => nearTarget.position, interval, aimingProcess: () => {
                Thrust(GetProperPosition(nearTarget, destination), reactPower, (lowerSpeed + maximumSpeed) / 2);
                if((nearTarget.position - position).magnitude < properDistance)
                {
                    if(!alreadyAttack)
                    {
                        PinchAttack();
                        alreadyAttack = true;
                    }
                }
                else if(alreadyAttack) alreadyAttack = false;
            });
            yield return Wait(() => !allWeapons.Any(weapon => !weapon.canAction));
        }
        else
        {
            yield return StoppingAction(lowerSpeed);
            PinchAttack();
            yield return Wait(interval, () => !allWeapons.Any(weapon => !weapon.canAction));
        }

        yield break;
    }
    /// <summary>
    /// 挟み攻撃
    /// </summary>
    void PinchAttack()
    {
        foreach(var handWeapon in handWeapons)
        {
            if(handWeapon.canAction)
            {
                handWeapon.Action(Weapon.ActionType.NOMAL);
                break;
            }
        }
    }
    /// <summary>
    /// 殴り攻撃
    /// </summary>
    void SwingAttack()
    {
        foreach(var handWeapon in handWeapons)
        {
            if(handWeapon.canAction)
            {
                handWeapon.Action(Weapon.ActionType.SINK);
                break;
            }
        }
    }
}