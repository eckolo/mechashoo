using UnityEngine;
using System.Collections;

public class Luwuxoji : Npc
{
    protected override IEnumerator motion(ActionPattern actionNum)
    {
        int interval = 100 - (int)(shipLevel / 10);
        var nearTarget = nowNearTarget;
        if(!captureTarget(nearTarget)) actionNum = ActionPattern.NON_COMBAT;

        switch(actionNum)
        {
            case ActionPattern.NON_COMBAT:
                nextActionState = ActionPattern.MOVE;
                exertPower(nowForward, reactPower, (lowerSpeed + maximumSpeed) / 2);
                break;
            case ActionPattern.MOVE:
                nextActionState = ActionPattern.AIMING;
                var destination = position + siteAlignment;
                for(var time = 0; time < interval * 2; time++)
                {
                    exertPower(destination - position, reactPower, maximumSpeed);
                    aiming(nearTarget.position);

                    destination = MathV.correct(destination, nearTarget.position, 0.999f);
                    yield return wait(1);
                }
                break;
            case ActionPattern.AIMING:
                nextActionState = ActionPattern.ATTACK;
                yield return aimingAction(nearTarget.position, () => stopping());
                break;
            case ActionPattern.ATTACK:
                nextActionState = ActionPattern.MOVE;

                int armNum = toInt(siteAlignment.magnitude < arms[1].tipLength);
                arms[armNum].tipHand.actionWeapon(Weapon.ActionType.NOMAL);

                yield return wait(interval);
                break;
            default:
                break;
        }

        yield break;
    }
}
