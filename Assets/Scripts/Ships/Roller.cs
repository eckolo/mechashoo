using UnityEngine;
using System.Collections;

/// <summary>
/// ローラーシップクラス
/// </summary>
public class Roller : Npc
{
    public override void Update()
    {
        base.Update();
    }

    protected override int setNextMotion(int actionNum)
    {
        maxActionChoices = 4;

        return (actionNum + 1) % (inField() ? maxActionChoices : 2);
    }

    protected override IEnumerator motion(int actionNum)
    {
        int interval = 100 - (int)(shipLevel / 10);
        var nearTarget = nowNearTarget;

        switch(actionNum)
        {
            case 1:
                setVerosity(nowForward, 0.6f);
                yield return wait(interval);
                break;
            case 2:
                if(nearTarget == null) break;
                if(!captureTarget(nearTarget)) break;
                setVerosity(nowForward, 0);
                var baseAngle = MathA.toAngle(nowForward);
                float targetAngle = MathA.toAngle(nearTarget.transform.position - transform.position);
                for(var time = 0; time < interval; time++)
                {
                    invertWidth(nowForward.x);
                    setAngle(getWidthRealAngle(MathA.correct(baseAngle, targetAngle, easing.quadratic.In(time, interval - 1))));
                    yield return wait(1);
                }
                break;
            case 3:
                if(nearTarget == null) break;
                if(!captureTarget(nearTarget)) break;
                foreach(var weaponSlot in weaponSlots)
                {
                    if(weaponSlot.entity == null) continue;
                    if(getParts(weaponSlot.partsNum) == null) continue;
                    getParts(weaponSlot.partsNum).GetComponent<Weapon>().action();
                }
                yield return wait(interval);
                break;
            default:
                break;
        }

        yield break;
    }
}