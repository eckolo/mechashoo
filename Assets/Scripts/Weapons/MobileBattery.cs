using UnityEngine;
using System.Collections;
using System.Linq;

public class MobileBattery : Gun
{
    public override void Start()
    {
        base.Start();
        Action();
    }

    protected override IEnumerator BaseMotion(int actionNum)
    {
        yield return base.BaseMotion(actionNum);
        DestroyMyself();
        yield break;
    }

    protected override IEnumerator Charging()
    {
        StartCoroutine(SetFixedAlignments());

        yield return base.Charging();

        yield break;
    }

    IEnumerator SetFixedAlignments()
    {
        var injection = nowInjections.FirstOrDefault();
        if(injection == null) yield break;

        var deceleration = GetBullet(injection).GetComponent<Shell>()?.Deceleration ?? 0;
        var setPosition = position;
        var nextSetCountDown = 0;
        for(var speed = injection.initialVelocity; speed > 0; speed -= deceleration)
        {
            setPosition += nowAngle.ToVector(speed).Rescaling(baseMas);
            if(nextSetCountDown-- <= 0)
            {
                var effect = Instantiate(sys.baseObjects.baseAlertAlignmentSprite);
                effect.nowParent = sysPanel.transform;
                effect.position = setPosition;
                nextSetCountDown = 10;
            }
            yield return Wait(1);
        }
        yield break;
    }
}
