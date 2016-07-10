using UnityEngine;
using System.Collections;

/// <summary>
/// 斬撃発生タイプの武装クラス
/// </summary>
public class Sword : Weapon
{
    public float defaultSlashSize = 1;

    protected override IEnumerator Motion(int actionNum)
    {
        switch (actionNum)
        {
            case 0:
                slash();
                break;
            case 1:
                yield return nife();
                break;
            default:
                break;
        }
        yield break;
    }

    /// <summary>
    /// 軽量刃物系モーション
    /// </summary>
    protected IEnumerator nife()
    {
        Hand tokenHand = transform.parent.GetComponent<Hand>();
        if (tokenHand != null)
        {
            Parts tokenArm = tokenHand.transform.parent.GetComponent<Parts>() ?? tokenHand;
            var radiusCriteria = Mathf.Abs(tokenHand.getParentConnection().x)
                + Mathf.Abs(tokenArm.getSelfConnection().x)
                + Mathf.Abs(tokenHand.GetComponent<Hand>().takePosition.x)
                + Mathf.Abs(tokenHand.getSelfConnection().x);
            var HalfRadiusCriteria = radiusCriteria / 2;

            var interval = Mathf.Max(timeRequired / density, 1);
            for (int time = 0; time < timeRequired * 2; time++)
            {
                var limit = timeRequired * 2 - 1;
                float localTimer = easing.quadratic.Out(limit, time, limit);
                setAngle(60 + (easing.quartic.Out(300, time, limit)));
                correctionVector.x = -easing.sinusoidal.Out(radiusCriteria, localTimer, limit);
                correctionVector.y = easing.sinusoidal.In(HalfRadiusCriteria, localTimer, limit);
                yield return null;
            }
            for (int time = 0; time < timeRequired; time++)
            {
                var limit = timeRequired - 1;
                float localTimer = easing.exponential.In(limit, time, limit);
                correctionVector.x = easing.sinusoidal.Out(radiusCriteria, localTimer, limit) - radiusCriteria;
                correctionVector.y = HalfRadiusCriteria - easing.sinusoidal.In(HalfRadiusCriteria, localTimer, limit);

                if ((timeRequired - 1 - time) % interval < 1) slash(localTimer / limit);

                yield return null;
            }
            for (int time = 0; time < timeRequired; time++)
            {
                var limit = timeRequired - 1;
                float localTimer = easing.exponential.Out(limit, time, limit);
                correctionVector.x = -easing.sinusoidal.In(HalfRadiusCriteria, localTimer, limit);
                correctionVector.y = -easing.sinusoidal.Out(radiusCriteria, localTimer, limit);

                if ((timeRequired - 1 - time) % interval < 1) slash(1 - localTimer / limit);

                yield return null;
            }
            for (int time = 0; time < timeRequired * 2; time++)
            {
                var limit = timeRequired * 2 - 1;
                float localTimer = easing.quadratic.InOut(limit, time, limit);
                setAngle((easing.quartic.In(420, time, limit)));
                correctionVector.x = easing.sinusoidal.In(HalfRadiusCriteria, localTimer, limit) - HalfRadiusCriteria;
                correctionVector.y = easing.sinusoidal.Out(radiusCriteria, localTimer, limit) - radiusCriteria;
                yield return null;
            }
        }
        else
        {
            for (int time = 0; time < timeRequired; time++)
            {
                setAngle(60 - (easing.quartic.Out(360, time, timeRequired - 1)));

                int interval = timeRequired / density + 1;
                if ((timeRequired - 1 - time) % interval == 0)
                {
                    slash();
                }

                yield return null;
            }
        }
    }

    /// <summary>
    /// 汎用斬撃発生関数
    /// </summary>
    protected void slash(float? slashSize = null)
    {
        for (var i = 0; i < injectionHoles.Count; i++)
        {
            var finalSize = (slashSize ?? 1) * defaultSlashSize * (1 + (injectionHoles[i] - selfConnection).magnitude);
            var finalFuelCost = finalSize / defaultSlashSize / injectionHoles.Count;

            var slash = injection(i).GetComponent<Slash>();
            if (slash == null) continue;

            slash.setVerosity(slash.transform.rotation * Vector2.right, 10);
            slash.setParamate(finalSize);
        }
    }
}
