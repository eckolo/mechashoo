using UnityEngine;
using System.Collections;

public class Luwucijiqi : Npc
{
    protected override IEnumerator motion(int actionNum)
    {
        int interval = Mathf.FloorToInt(Mathf.Max(100 - shipLevel, 1));
        var nearTarget = nowNearTarget;

        switch(nowActionState)
        {
            case ActionPattern.MOVE:
                nextActionState = ActionPattern.AIMING;
                var wraps = Random.Range(2, 3 + (int)shipLevel);

                for(int wrap = 0; wrap < wraps; wrap++)
                {
                    var nowAngle = (nowSpeed.magnitude > 0 ? nowSpeed : siteAlignment)
                        .correct(position - nearTarget.position, 0.8f)
                        .toAngle();
                    var direction = nowAngle + Random.Range(90, 270);
                    yield return aimingAction(nearTarget.position,
                        interval,
                        () => exertPower(direction, reactPower, maximumSpeed));
                }
                break;
            case ActionPattern.AIMING:
                nextActionState = ActionPattern.ATTACK;
                nextActionIndex = (nearTarget.position.magnitude < arms[1].tipLength).toInt();
                yield return aimingAction(() => nearTarget.position,
                    interval,
                    () => exertPower(nearTarget.position - position, reactPower, lowerSpeed));
                break;
            case ActionPattern.ATTACK:
                nextActionState = ActionPattern.MOVE;

                int armNum = (siteAlignment.magnitude > arms[1].tipLength).toInt();
                arms[armNum].tipHand.actionWeapon(Weapon.ActionType.NOMAL);

                yield return stoppingAction();
                yield return wait(() => arms[armNum].tipHand.takeWeapon.canAction);
                break;
            default:
                break;
        }

        yield break;
    }
}
