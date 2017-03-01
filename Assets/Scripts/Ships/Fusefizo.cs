using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// 小型蟹機クラス
/// </summary>
public class Fusefizo : Npc
{
    protected override IEnumerator motion(int actionNum)
    {
        int interval = Mathf.FloorToInt(Mathf.Max(100 - shipLevel, 1));
        var destination = new[] { -90, 0, 90 }.selectRandom(new[] { 3, 2, 3 });
        var nearTarget = nowNearTarget;

        switch(nowActionState)
        {
            case ActionPattern.MOVE:
                nextActionState = ActionPattern.AIMING;
                var targetPosition = nearTarget.position - position;
                yield return aimingAction(() => nearTarget.position,
                    interval * 2,
                    () => exertPower(getProperPosition(targetPosition, destination), reactPower, maximumSpeed));
                nextActionIndex = new[] { 0, 1 }.selectRandom(new[] { 2, 3 });
                break;
            case ActionPattern.AIMING:
                nextActionState = ActionPattern.ATTACK;
                var alreadyAttack = false;
                if(actionNum != 0) yield return aimingAction(() => nearTarget.position,
                        interval,
                       () => {
                           exertPower(getProperPosition(nearTarget, destination), reactPower, (lowerSpeed + maximumSpeed) / 2);
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
                break;
            case ActionPattern.ATTACK:
                nextActionState = ActionPattern.MOVE;
                yield return stoppingAction(lowerSpeed);
                if(actionNum != 0)
                {
                    SwingAttack();
                    yield return aimingAction(() => nearTarget.position, interval);
                    yield return wait(() => !allWeapons.Any(weapon => !weapon.canAction));
                }
                else
                {
                    PinchAttack();
                    yield return wait(interval, () => !allWeapons.Any(weapon => !weapon.canAction));
                }
                break;
            default:
                break;
        }

        yield break;
    }
    void PinchAttack()
    {
        foreach(var handWeapon in handWeapons)
        {
            if(handWeapon.canAction)
            {
                handWeapon.action(Weapon.ActionType.NOMAL);
                break;
            }
        }
    }
    void SwingAttack()
    {
        foreach(var handWeapon in handWeapons)
        {
            if(handWeapon.canAction)
            {
                handWeapon.action(Weapon.ActionType.SINK);
                break;
            }
        }
    }
}