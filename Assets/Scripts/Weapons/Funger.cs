using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ファンガークラス
/// </summary>
public partial class Funger : Weapon
{
    /// <summary>
    ///斬撃の規模
    /// </summary>
    [SerializeField]
    private float defaultSlashSize = 1;

    /// <summary>
    /// 振り時効果音
    /// </summary>
    [SerializeField]
    protected AudioClip swingSE = null;
    /// <summary>
    /// 噛み合わせ時効果音
    /// </summary>
    [SerializeField]
    protected AudioClip biteSE = null;

    [SerializeField]
    protected enum AttackType
    {
        BITE,
        BIGBITE
    }
    private Dictionary<AttackType, IMotion<Funger>> _motionList = new Dictionary<AttackType, IMotion<Funger>>();
    protected Dictionary<AttackType, IMotion<Funger>> motionList
    {
        get {
            if(_motionList.Count <= 0)
            {
                _motionList.Add(AttackType.BITE, new Bite());
                _motionList.Add(AttackType.BIGBITE, new BigBite());
            }
            return _motionList;
        }
    }

    /// <summary>
    /// 通常時モーション
    /// </summary>
    [SerializeField]
    protected AttackType nomalAttack = AttackType.BITE;
    /// <summary>
    /// Shiftモーション
    /// </summary>
    [SerializeField]
    protected AttackType sinkAttack = AttackType.BITE;
    /// <summary>
    /// 固定時モーション
    /// </summary>
    [SerializeField]
    protected AttackType fixedAttack = AttackType.BITE;
    /// <summary>
    /// NPC限定モーション
    /// </summary>
    [SerializeField]
    protected AttackType npcAttack = AttackType.BITE;

    protected override IEnumerator motion(int actionNum)
    {
        yield return motionList[getAttackType(nowAction)].mainMotion(this);
        yield break;
    }
    protected override IEnumerator endMotion(int actionNum)
    {
        yield return motionList[getAttackType(nowAction)].endMotion(this);
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
            default: return AttackType.BITE;
        }
    }

    /// <summary>
    /// 噛み付き動作
    /// </summary>
    /// <param name="timePar">所要時間比率</param>
    /// <param name="power">斬撃威力（サイズ）比率</param>
    /// <returns></returns>
    protected IEnumerator engage(float timePar = 1, float power = 1)
    {
        soundSE(swingSE);
        var limit = (int)(timeRequired * timePar);
        var startAngle1 = fung1.nowLocalAngle;
        var startAngle2 = fung2.nowLocalAngle;
        for(int time = 0; time < limit; time++)
        {
            fung1.setAngle(startAngle1 - Easing.quintic.In(startAngle1, time, limit - 1));
            fung2.setAngle(startAngle2 + Easing.quintic.In(360 - startAngle2, time, limit - 1));
            yield return wait(1);
        }

        soundSE(biteSE);
        fung1.defaultSlashSize = defaultSlashSize * power;
        fung2.defaultSlashSize = defaultSlashSize * power;
        fung1.action(nowAction);
        fung2.action(nowAction);

        yield break;
    }
    /// <summary>
    /// 噛み付き状態からの戻り動作
    /// </summary>
    /// <param name="timePar">所要時間比率</param>
    /// <returns></returns>
    protected IEnumerator reengage(float timePar = 1)
    {
        var limit = (int)(timeRequired * 2 * timePar);
        yield return wait(limit);
        for(int time = 0; time < limit; time++)
        {
            fung1.setAngle(Easing.liner.In(fung1.defAngle, time, limit - 1));
            fung2.setAngle(-Easing.liner.In(fung2.defAngle, time, limit - 1));

            yield return wait(1);
        }

        yield break;
    }
    protected Sword fung1 => fungs.First();
    protected Sword fung2 => fungs.Last();
    protected List<Sword> fungs
    {
        get {
            if(!_fungs.Any()) _fungs = GetComponent<Things>().getPartsList
                    .Select(parts => parts.GetComponent<Sword>())
                    .Take(2)
                    .ToList();
            return _fungs;
        }
    }
    List<Sword> _fungs = new List<Sword>();
}
