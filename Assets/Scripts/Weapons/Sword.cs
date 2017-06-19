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
        NIFE,
        SPEAR,
        SPIN,
        LARGE_SPIN
    }
    private Dictionary<AttackType, IMotion<Sword>> _motionList = new Dictionary<AttackType, IMotion<Sword>>();
    protected Dictionary<AttackType, IMotion<Sword>> motionList
    {
        get {
            if(_motionList.Count <= 0)
            {
                _motionList.Add(AttackType.SINGLE, new OneShot());
                _motionList.Add(AttackType.NIFE, new Nife());
                _motionList.Add(AttackType.SPEAR, new Spear());
                _motionList.Add(AttackType.SPIN, new Spin());
                _motionList.Add(AttackType.LARGE_SPIN, new LargeSpin());
            }
            return _motionList;
        }
    }
    [Serializable]
    protected class MotionParameter
    {
        public AttackType type = AttackType.SINGLE;
        public bool forward = true;
    }

    /// <summary>
    /// 通常時モーション
    /// </summary>
    [SerializeField]
    protected MotionParameter nomalAttack = new MotionParameter();
    /// <summary>
    /// Shiftモーション
    /// </summary>
    [SerializeField]
    protected MotionParameter sinkAttack = new MotionParameter();
    /// <summary>
    /// 固定時モーション
    /// </summary>
    [SerializeField]
    protected MotionParameter fixedAttack = new MotionParameter();
    /// <summary>
    /// NPC限定モーション
    /// </summary>
    [SerializeField]
    protected MotionParameter npcAttack = new MotionParameter();

    /// <summary>
    /// 動作準備モーション時の必要時間倍率
    /// </summary>
    [SerializeField]
    protected float timeRequiredParPrior = 1;
    /// <summary>
    /// 動作準備モーション時の必要時間
    /// </summary>
    public int timeRequiredPrior
    {
        get {
            return (int)(timeRequired * timeRequiredParPrior);
        }
    }
    /// <summary>
    /// 残身モーション時の必要時間倍率
    /// </summary>
    [SerializeField]
    protected float timeRequiredParARest = 1;
    /// <summary>
    /// 残身モーション時の必要時間
    /// </summary>
    public int timeRequiredARest
    {
        get {
            return (int)(timeRequired * timeRequiredParARest);
        }
    }

    /// <summary>
    /// 振り上げ時効果音
    /// </summary>
    [SerializeField]
    protected AudioClip swingUpSE = null;
    /// <summary>
    /// 振り下ろし時効果音
    /// </summary>
    [SerializeField]
    protected AudioClip swingDownSE = null;

    public float defaultSlashSize = 1;

    protected override IEnumerator motion(int actionNum)
    {
        var parameter = getAttackType(nowAction);
        yield return motionList[parameter.type].mainMotion(this, parameter.forward);
        yield break;
    }
    protected override IEnumerator endMotion(int actionNum)
    {
        if(nextAction == nowAction) yield break;
        var parameter = getAttackType(nowAction);
        yield return motionList[parameter.type].endMotion(this, parameter.forward);
        yield break;
    }

    MotionParameter getAttackType(ActionType action)
    {
        switch(action)
        {
            case ActionType.NOMAL: return nomalAttack;
            case ActionType.SINK: return sinkAttack;
            case ActionType.FIXED: return fixedAttack;
            case ActionType.NPC: return npcAttack;
            default: return new MotionParameter();
        }
    }

    /// <summary>
    /// 汎用斬撃発生関数
    /// </summary>
    public void slash(float slashSize = 1)
    {
        foreach(var injection in onTypeInjections)
        {
            var finalSize = slashSize * defaultSlashSize * (1 + (injection.hole - selfConnection).magnitude);

            var slash = inject(injection).GetComponent<Slash>();
            if(slash == null) continue;

            slash.setParamate(finalSize);
        }
    }
}
