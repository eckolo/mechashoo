using UnityEngine;
using System.Collections;

/// <summary>
/// 近接タイプの武装クラス
/// </summary>
public class Sword : Weapon
{
    [SerializeField]
    protected enum AttackType
    {
        Single,
        Nife
    }
    /// <summary>
    /// 通常時モーション
    /// </summary>
    [SerializeField]
    protected AttackType nomalAttack = AttackType.Single;
    /// <summary>
    /// Shiftモーション
    /// </summary>
    [SerializeField]
    protected AttackType sinkAttack = AttackType.Single;
    /// <summary>
    /// 固定時モーション
    /// </summary>
    [SerializeField]
    protected AttackType fixedAttack = AttackType.Single;
    /// <summary>
    /// NPC限定モーション
    /// </summary>
    [SerializeField]
    protected AttackType npcAttack = AttackType.Single;

    public float defaultSlashSize = 1;

    protected override IEnumerator Motion(ActionType actionNum)
    {
        AttackType motionType = AttackType.Single;
        switch (actionNum)
        {
            case ActionType.Nomal:
                motionType = nomalAttack;
                break;
            case ActionType.Sink:
                motionType = sinkAttack;
                break;
            case ActionType.Fixed:
                motionType = fixedAttack;
                break;
            case ActionType.Npc:
                motionType = npcAttack;
                break;
            default:
                break;
        }
        switch (motionType)
        {
            case AttackType.Single:
                slash();
                break;
            case AttackType.Nife:
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
            var radiusCriteria = (tokenArm.nowLengthVector + tokenHand.nowLengthVector).magnitude;
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

            var slash = injection(i).GetComponent<Slash>();
            if (slash == null) continue;

            slash.setVerosity(slash.transform.rotation * Vector2.right, 10);
            slash.setParamate(finalSize);
        }
    }
}
