using UnityEngine;
using System.Collections;

/// <summary>
/// 小型蟹機クラス
/// </summary>
public class Fusefizo : Npc
{
    protected override IEnumerator motion(int actionNum)
    {
        int interval = Mathf.FloorToInt(Mathf.Max(100 - shipLevel, 1));
        var nearTarget = nowNearTarget;

        switch(nowActionState)
        {
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