using UnityEngine;
using System.Collections;

/// <summary>
/// ローラーシップクラス
/// </summary>
public class Xewusigume : Npc
{
    protected override IEnumerator motion(ActionPattern actionNum)
    {
        int interval = 100 - (int)(shipLevel / 10);
        var nearTarget = nowNearTarget;
        if(!captureTarget(nearTarget)) actionNum = ActionPattern.NON_COMBAT;

        switch(actionNum)
        {
            case ActionPattern.NON_COMBAT:
                nextActionState = ActionPattern.AIMING;
                exertPower(nowForward, reactPower, (lowerSpeed + maximumSpeed) / 2);
                break;
            case ActionPattern.MOVE:
                nextActionState = ActionPattern.AIMING;
                var direction = nowForward;
                for(var time = 0; time < interval * 2; time++)
                {
                    exertPower(direction, reactPower, maximumSpeed);
                    aiming(nearTarget.position);
                    yield return wait(1);
                }
                break;
            case ActionPattern.AIMING:
                nextActionState = ActionPattern.ATTACK;
                yield return aimingAction(nearTarget.position, () => exertPower(nowForward, reactPower, lowerSpeed), 1);
                break;
            case ActionPattern.ATTACK:
                nextActionState = ActionPattern.MOVE;
                foreach(var weaponSlot in weaponSlots)
                {
                    if(weaponSlot.entity == null) continue;
                    if(getParts(weaponSlot.partsNum) == null) continue;
                    getParts(weaponSlot.partsNum).GetComponent<Weapon>().action();
                }
                for(var time = 0; time < interval; time++)
                {
                    exertPower(nowForward, reactPower, 0);
                    yield return wait(1);
                }
                break;
            default:
                break;
        }

        yield break;
    }
}