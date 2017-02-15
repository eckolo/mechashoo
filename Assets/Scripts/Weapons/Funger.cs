using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ファンガークラス
/// </summary>
public class Funger : Weapon
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

    protected override IEnumerator motion(int actionNum)
    {
        switch(nowAction)
        {
            case ActionType.NOMOTION:
                break;
            case ActionType.NOMAL:
                yield return engage();
                break;
            case ActionType.SINK:
                yield return engage();
                break;
            case ActionType.FIXED:
                yield return engage();
                break;
            case ActionType.NPC:
                yield return engage();
                break;
            default:
                break;
        }
        yield break;
    }
    protected override IEnumerator endMotion(int actionNum)
    {
        switch(nowAction)
        {
            case ActionType.NOMOTION:
                break;
            case ActionType.NOMAL:
                yield return reengage();
                break;
            case ActionType.SINK:
                yield return reengage();
                break;
            case ActionType.FIXED:
                yield return reengage();
                break;
            case ActionType.NPC:
                yield return reengage();
                break;
            default:
                break;
        }
        yield break;
    }

    protected IEnumerator engage()
    {
        soundSE(swingSE);
        var limit = timeRequired;
        var startPosition1 = fung1.nowLocalAngle;
        var startPosition2 = fung2.nowLocalAngle;
        for(int time = 0; time < limit; time++)
        {
            fung1.setAngle(startPosition1 - Easing.quintic.In(startPosition1, time, limit - 1));
            fung2.setAngle(startPosition2 + Easing.quintic.In(startPosition2, time, limit - 1));
            yield return wait(1);
        }

        soundSE(biteSE);
        fung1.defaultSlashSize = defaultSlashSize;
        fung2.defaultSlashSize = defaultSlashSize;
        fung1.action(nowAction);
        fung2.action(nowAction);

        yield break;
    }
    protected IEnumerator reengage()
    {
        var limit = timeRequired * 2;
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
