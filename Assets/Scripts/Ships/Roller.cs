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

        return (actionNum + 1) % (inScreen() ? maxActionChoices : 2);
    }

    protected override IEnumerator Motion(int actionNum)
    {
        int interval = 100 - (int)(shipLevel / 10);

        switch (actionNum)
        {
            case 1:
                setVerosity(direction(), 0.6f);
                yield return wait(interval);
                break;
            case 2:
                var target = getNearTarget();
                if (target == null) break;
                setVerosity(direction(), 0);
                Vector2 targetVector = target.transform.position - transform.position;
                for (var time = 0; time < interval; time++)
                {
                    var nowDirection = direction() + new Vector2(
                        easing.quadratic.In(targetVector.x, time, interval - 1),
                        easing.quadratic.In(targetVector.y, time, interval - 1));
                    if (nowDirection.x > 0) widthPositive = true;
                    if (nowDirection.x < 0) widthPositive = false;
                    setAngle(nowDirection, widthPositive);
                    yield return null;
                }
                break;
            case 3:
                if (getNearTarget() == null) break;
                foreach (var weaponNum in weaponNumList)
                {
                    if (getParts(weaponNum) != null) getParts(weaponNum).GetComponent<Weapon>().Action();
                }
                yield return wait(interval);
                break;
            default:
                break;
        }

        yield break;
    }

    private Vector2 direction()
    {
        return transform.rotation * Vector2.right * (widthPositive ? 1 : -1);
    }
}