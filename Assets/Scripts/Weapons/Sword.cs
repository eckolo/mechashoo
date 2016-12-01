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
        SINGLE,
        NIFE
    }
    /// <summary>
    /// 通常時モーション
    /// </summary>
    [SerializeField]
    protected AttackType nomalAttack = AttackType.SINGLE;
    /// <summary>
    /// Shiftモーション
    /// </summary>
    [SerializeField]
    protected AttackType sinkAttack = AttackType.SINGLE;
    /// <summary>
    /// 固定時モーション
    /// </summary>
    [SerializeField]
    protected AttackType fixedAttack = AttackType.SINGLE;
    /// <summary>
    /// NPC限定モーション
    /// </summary>
    [SerializeField]
    protected AttackType npcAttack = AttackType.SINGLE;

    public float defaultSlashSize = 1;

    protected override IEnumerator motion(ActionType actionNum)
    {
        AttackType motionType = AttackType.SINGLE;
        switch(actionNum)
        {
            case ActionType.NOMAL:
                motionType = nomalAttack;
                break;
            case ActionType.SINK:
                motionType = sinkAttack;
                break;
            case ActionType.FIXED:
                motionType = fixedAttack;
                break;
            case ActionType.NPC:
                motionType = npcAttack;
                break;
            default:
                break;
        }
        switch(motionType)
        {
            case AttackType.SINGLE:
                slash();
                break;
            case AttackType.NIFE:
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
        if(tokenHand != null)
        {
            Parts tokenArm = tokenHand.transform.parent.GetComponent<Parts>() ?? tokenHand;
            var radiusCriteria = (tokenArm.nowLengthVector + tokenHand.nowLengthVector).magnitude;

            var startPosition = correctionVector;
            var endPosition = new Vector2(-1, 0.5f) * radiusCriteria;
            var interval = Mathf.Max(timeRequired / density, 1);
            for(int time = 0; time < timeRequired * 2; time++)
            {
                var limit = timeRequired * 2 - 1;
                float localTimer = easing.quadratic.Out(limit, time, limit);

                setAngle(60 + (easing.quartic.Out(300, time, limit)));
                correctionVector = MathV.Easing.elliptical(startPosition, endPosition, localTimer, limit, true);
                yield return wait(1);
            }
            startPosition = correctionVector;
            endPosition = Vector2.zero;
            for(int time = 0; time < timeRequired; time++)
            {
                var limit = timeRequired - 1;
                float localTimer = easing.exponential.In(limit, time, limit);

                correctionVector = MathV.Easing.elliptical(startPosition, endPosition, localTimer, limit, true);
                if((timeRequired - 1 - time) % interval < 1) slash(localTimer / limit);

                yield return wait(1);
            }
            startPosition = correctionVector;
            endPosition = new Vector2(-0.5f, -1) * radiusCriteria;
            for(int time = 0; time < timeRequired; time++)
            {
                var limit = timeRequired - 1;
                float localTimer = easing.exponential.Out(limit, time, limit);

                correctionVector = MathV.Easing.elliptical(startPosition, endPosition, localTimer, limit, true);
                if((timeRequired - 1 - time) % interval < 1) slash(1 - localTimer / limit);

                yield return wait(1);
            }
            startPosition = correctionVector;
            endPosition = Vector2.zero;
            for(int time = 0; time < timeRequired * 2; time++)
            {
                var limit = timeRequired * 2 - 1;
                float localTimer = easing.quadratic.InOut(limit, time, limit);

                setAngle((easing.quartic.In(420, time, limit)));
                correctionVector = MathV.Easing.elliptical(startPosition, endPosition, localTimer, limit, true);
                yield return wait(1);
            }
        }
        else
        {
            for(int time = 0; time < timeRequired; time++)
            {
                setAngle(60 - (easing.quartic.Out(360, time, timeRequired - 1)));

                int interval = timeRequired / density + 1;
                if((timeRequired - 1 - time) % interval == 0) slash();


                yield return wait(1);
            }
        }
    }

    /// <summary>
    /// 汎用斬撃発生関数
    /// </summary>
    protected void slash(float? slashSize = null)
    {
        foreach(var injection in injections)
        {
            var finalSize = (slashSize ?? 1) * defaultSlashSize * (1 + (injection.hole - selfConnection).magnitude);

            var slash = inject(injection).GetComponent<Slash>();
            if(slash == null) continue;

            slash.setVerosity(slash.transform.rotation * Vector2.right, 10);
            slash.setParamate(finalSize);
        }
    }
}
