using UnityEngine;
using System.Collections;

/// <summary>
/// ローラーシップクラス
/// </summary>
public class Xewusigume : Npc
{
    protected override IEnumerator motion(int actionNum)
    {
        int interval = 100 - (int)(shipLevel / 10);
        var nearTarget = nowNearTarget;

        switch(nowActionState)
        {
            case ActionPattern.NON_COMBAT:
                nextActionState = ActionPattern.NON_COMBAT;
                exertPower(nowForward, reactPower, (lowerSpeed + maximumSpeed) / 2);
                break;
            case ActionPattern.MOVE:
                nextActionState = ActionPattern.AIMING;
                var direction = nowForward;
                yield return aimingAction(nearTarget.position, interval * 2, () => exertPower(direction, reactPower, maximumSpeed));
                break;
            case ActionPattern.AIMING:
                nextActionState = ActionPattern.ATTACK;
                yield return aimingAction(nearTarget.position, () => exertPower(nowForward, reactPower, lowerSpeed), 1);
                break;
            case ActionPattern.ATTACK:
                nextActionState = ActionPattern.MOVE;
                foreach(var weapon in bodyWeapons)
                {
                    if(weapon == null) continue;
                    weapon.action();
                }
                for(var time = 0; time < interval; time++)
                {
                    stopping();
                    yield return wait(1);
                }
                break;
            default:
                break;
        }

        yield break;
    }
}