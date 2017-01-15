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
                nextActionState = ActionPattern.AIMING;
                exertPower(nowForward, reactPower, (lowerSpeed + maximumSpeed) / 2);
                break;
            case ActionPattern.MOVE:
                nextActionState = ActionPattern.AIMING;
                for(var time = 0; time < interval * 2; time++)
                {
                    exertPower(nowForward, reactPower, maximumSpeed);
                    yield return wait(1);
                }
                break;
            case ActionPattern.AIMING:
                nextActionState = ActionPattern.ATTACK;
                setVerosity(nowForward, 0);
                yield return aimingAction(nearTarget.position, finishRange: 0.01f);
                break;
            case ActionPattern.ATTACK:
                nextActionState = ActionPattern.MOVE;
                var armLength = armStates[1].entity.nowLengthVector.magnitude;
                var handLength = armStates[1].entity.childParts.nowLengthVector.magnitude;

                int armNum = toInt(siteAlignment.magnitude < armLength + handLength);
                getHand(armStates[armNum]).actionWeapon(Weapon.ActionType.NOMAL);

                yield return wait(interval);
                break;
            default:
                break;
        }

        yield break;
    }
}
