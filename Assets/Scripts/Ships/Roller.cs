using UnityEngine;
using System.Collections;

public class Roller : Enemy
{
    protected override int setNextMotion(int actionNum)
    {
        maxActionChoices = 3;

        return (actionNum + 1) % maxActionChoices;
    }

    protected override IEnumerator Motion(int actionNum)
    {
        var interval = 36;

        switch (actionNum)
        {
            case 1:
                var target = getNearTarget();
                if (target == null) break;
                Vector2 targetVector = target.transform.position - transform.position;
                if (targetVector.x > 0) widthPositive = true;
                if (targetVector.x < 0) widthPositive = false;
                setVerosity(targetVector, 0.6f);
                setAngle(targetVector, widthPositive);
                break;
            case 2:
                foreach (var weaponNum in weaponNumList)
                {
                    if (getParts(weaponNum) != null) getParts(weaponNum).GetComponent<Weapon>().Action();
                }
                break;
            default:
                break;
        }

        yield return wait(interval);

        yield break;
    }
}