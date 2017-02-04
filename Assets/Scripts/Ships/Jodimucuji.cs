using UnityEngine;
using System.Collections;

public class Jodimucuji : Npc
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
