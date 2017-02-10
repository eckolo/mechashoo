using UnityEngine;
using System.Collections;

public class Luwuxoji : Npc
{
    protected override IEnumerator motion(int actionNum)
    {
        int interval = Mathf.FloorToInt(Mathf.Max(100 - shipLevel, 1));
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
                yield return aimingAction(nearTarget.position, interval * 2, () => {
                    exertPower(destination - position, reactPower, maximumSpeed);
                    destination = destination.correct(nearTarget.position, 0.999f);
                });
                break;
            case ActionPattern.AIMING:
                nextActionState = ActionPattern.ATTACK;
                nextActionIndex = toInt(nearTarget.position.magnitude < arms[1].tipLength);
                yield return aimingAction(() => nearTarget.position, () => nowSpeed.magnitude > 0, () => stopping());
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
