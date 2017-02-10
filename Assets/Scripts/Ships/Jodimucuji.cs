using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Npc.ActionPattern;

public class Jodimucuji : Npc
{
    //0:grenade
    //1:assaulter
    //2:laser
    protected override IEnumerator motion(int actionNum)
    {
        int interval = Mathf.FloorToInt(Mathf.Max(100 - shipLevel, 1));
        var moderateSpeed = (lowerSpeed + maximumSpeed) / 2;
        var nearTarget = nowNearTarget;

        switch(nowActionState)
        {
            case NON_COMBAT:
                nextActionState = NON_COMBAT;
                exertPower(nowForward, reactPower, moderateSpeed);
                break;
            case MOVE:
                nextActionState = AIMING;
                yield return aimingAction(() => nearTarget.position, interval * 2, () => {
                    var target = nearTarget.position - position;
                    var degree = getProperPosition(target);
                    var destination = target - siteAlignment;
                    exertPower((destination + degree) / 2, reactPower, moderateSpeed);
                });
                nextActionIndex = Random.Range(0, allWeapons.Count);
                break;
            case AIMING:
                nextActionState = ATTACK;
                switch(actionNum)
                {
                    case 0:
                        yield return aimingAction(nearTarget.position, () => nowSpeed.magnitude > 0, () => stopping());
                        break;
                    case 1:
                        yield return aimingAction(() => getDeviationTarget(nearTarget), () => nowSpeed.magnitude > 0, () => stopping());
                        break;
                    case 2:
                        var _destination = new Vector2(position.x, getDeviationTarget(nearTarget).y);
                        yield return headingDestination(_destination, maximumSpeed);
                        break;
                    default:
                        break;
                }
                break;
            case ATTACK:
                allWeapons[actionNum].action(Weapon.ActionType.NOMAL);
                var continuous = actionNum == 1
                    && new List<bool> { true, false }.selectRandom(new List<int> { 2, 1 });
                if(continuous)
                {
                    var limit = Random.Range(1, shipLevel + 1);
                    var speed = Mathf.Min(lowerSpeed * limit, maximumSpeed);
                    for(int index = 0; index < limit && nearTarget.isAlive; index++)
                    {
                        var weapon = allWeapons[actionNum];
                        yield return aimingAction(() => getProperPosition(getDeviationTarget(nearTarget) - position),
                            interval / 2,
                            () => exertPower(nearTarget.position - position, reactPower, speed));
                        yield return aimingAction(getProperPosition(getDeviationTarget(nearTarget) - position),
                            () => !weapon.canAction,
                            () => stopping());
                        weapon.action(Weapon.ActionType.NOMAL);
                    }
                }
                if(actionNum == 2)
                {
                    nextActionState = new List<ActionPattern> { AIMING, MOVE }.selectRandom(new List<int> { 1, 2 });
                    nextActionIndex = Random.Range(0, allWeapons.Count(weapon => weapon != allWeapons[actionNum]));
                    yield return stopping();
                    yield return aimingAction(() => nearTarget.position, interval);
                }
                else
                {
                    nextActionState = MOVE;
                    yield return wait(interval);
                }
                break;
            default:
                break;
        }

        yield break;
    }
}
