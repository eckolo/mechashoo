using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

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
        LARGE_SPIN,
        LONG_SLEEVED
    }
    private Dictionary<AttackType, IMotion<Sword>> _motionList = new Dictionary<AttackType, IMotion<Sword>>();
    protected Dictionary<AttackType, IMotion<Sword>> motionList
    {
        get {
            if(!_motionList.Any())
            {
                _motionList.Add(AttackType.SINGLE, new OneShot());
                _motionList.Add(AttackType.NIFE, new Nife());
                _motionList.Add(AttackType.SPEAR, new Spear());
                _motionList.Add(AttackType.SPIN, new Spin());
                _motionList.Add(AttackType.LARGE_SPIN, new LargeSpin());
                _motionList.Add(AttackType.LONG_SLEEVED, new LongSleeved());
            }
            return _motionList;
        }
    }

    /// <summary>
    /// 弾丸密度
    /// </summary>
    protected override int density
        => Mathf.CeilToInt(base.density * GetAttackType(nowAction).density);
    /// <summary>
    /// 1モーションの所要時間
    /// </summary>
    protected override int timeRequired
        => Mathf.RoundToInt(base.timeRequired * GetAttackType(nowAction).timeTweak);
    /// <summary>
    /// 回転数
    /// </summary>
    protected int turnoverRate => GetAttackType(nowAction).turnoverRate;

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
    public virtual int timeRequiredPrior
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
    public virtual int timeRequiredARest
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

    [SerializeField]
    public float defaultSlashSize = 1;

    /// <summary>
    /// 行動回数
    /// </summary>
    protected int fireNum
        => Mathf.Max(onTypeInjections.Max(injection => injection.burst), 1);

    protected override IEnumerator Motion(int actionNum)
    {
        var parameter = GetAttackType(nowAction);
        yield return motionList[parameter.type].MainMotion(this, parameter.forward);
        yield break;
    }
    protected override IEnumerator EndMotion(int actionNum)
    {
        if(nextAction == nowAction) yield break;
        var parameter = GetAttackType(nowAction);
        yield return motionList[parameter.type].EndMotion(this, parameter.forward);
        yield break;
    }

    MotionParameter GetAttackType(ActionType action)
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
    public void Slash(float slashSize = 1)
    {
        foreach(var injection in onTypeInjections)
        {
            var finalSize = slashSize * defaultSlashSize * (1 + (injection.hole - selfConnection).magnitude);

            var slash = Inject(injection).GetComponent<Slash>();
            if(slash == null) continue;

            slash.SetParamate(finalSize, GetAttackType(nowAction).power);
        }
    }
}
