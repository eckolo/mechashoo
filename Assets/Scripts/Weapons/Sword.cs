using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

/// <summary>
/// 近接タイプの武装クラス
/// </summary>
public partial class Sword : Weapon
{
    [SerializeField]
    protected enum AttackType
    {
        SINGLE,
        NIFE
    }
    private Dictionary<AttackType, PublicAction<bool>> _motionList = new Dictionary<AttackType, PublicAction<bool>>();
    protected Dictionary<AttackType, PublicAction<bool>> motionList
    {
        get
        {
            if(_motionList.Count <= 0)
            {
                _motionList.Add(AttackType.SINGLE, oneShot);
                _motionList.Add(AttackType.NIFE, nife);
            }
            return _motionList;
        }
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

    protected override IEnumerator motion(ActionType action)
    {
        yield return motionList[getAttackType(action)](true);
        yield break;
    }
    protected override IEnumerator endMotion(ActionType action)
    {
        if(nextAction == action) yield break;
        yield return motionList[getAttackType(action)](false);
        yield break;
    }

    AttackType getAttackType(ActionType action)
    {
        switch(action)
        {
            case ActionType.NOMAL: return nomalAttack;
            case ActionType.SINK: return sinkAttack;
            case ActionType.FIXED: return fixedAttack;
            case ActionType.NPC: return npcAttack;
            default: return AttackType.SINGLE;
        }
    }
    /// <summary>
    /// 単発系モーション
    /// </summary>
    protected IEnumerator oneShot(bool main)
    {
        slash();
        yield break;
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
