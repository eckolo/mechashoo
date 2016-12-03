using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

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
            var interval = Mathf.Max(timeRequired / density, 1);

            yield return swingAction(endPosition: new Vector2(-1, 0.5f),
              timeLimit: timeRequired * 2,
              timeEasing: easing.quadratic.Out,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => setAngle(60 + (easing.quartic.Out(300, time, limit))));

            yield return swingAction(endPosition: Vector2.zero,
              timeLimit: timeRequired,
              timeEasing: easing.exponential.In,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  if((timeRequired - 1 - time) % interval < 1) slash(localTime / limit);
              });

            yield return swingAction(endPosition: new Vector2(-0.5f, -1),
              timeLimit: timeRequired,
              timeEasing: easing.exponential.Out,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => {
                  if((timeRequired - 1 - time) % interval < 1) slash(1 - localTime / limit);
              });

            yield return swingAction(endPosition: Vector2.zero,
              timeLimit: timeRequired * 2,
              timeEasing: easing.quadratic.InOut,
              clockwise: true,
              midstreamProcess: (time, localTime, limit) => setAngle((easing.quartic.In(420, time, limit))));
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

    private IEnumerator swingAction(Vector2 endPosition,
        int timeLimit,
        Func<float, float, float, float> timeEasing,
        bool clockwise,
        UnityAction<int, float, int> midstreamProcess = null)
    {
        Hand tokenHand = transform.parent.GetComponent<Hand>();
        if(tokenHand == null) yield break;

        Parts tokenArm = tokenHand.transform.parent.GetComponent<Parts>() ?? tokenHand;
        var radiusCriteria = (tokenArm.nowLengthVector + tokenHand.nowLengthVector).magnitude;
        var startPosition = correctionVector;

        for(int time = 0; time < timeLimit; time++)
        {
            var limit = timeLimit - 1;
            float localTime = timeEasing(limit, time, limit);

            correctionVector = MathV.Easing.elliptical(startPosition, endPosition * radiusCriteria, localTime, limit, clockwise);
            if(midstreamProcess != null) midstreamProcess(time, localTime, limit);
            yield return wait(1);
        }
        yield break;
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
