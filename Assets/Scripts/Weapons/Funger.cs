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

    protected override IEnumerator motion(ActionType actionNum)
    {
        //パーツアクセスのショートカット割り振り
        var fung = GetComponent<Things>().getPartsList
            .Select(parts => parts.GetComponent<Sword>()).ToList();

        soundSE(swingSE);
        for(int time = 0; time < timeRequired; time++)
        {
            fung[0].setAngle(180 - easing.quintic.In(180, time, timeRequired - 1));
            fung[1].setAngle(180 + easing.quintic.In(180, time, timeRequired - 1));
            yield return wait(1);
        }

        soundSE(biteSE);
        fung[0].defaultSlashSize = defaultSlashSize;
        fung[1].defaultSlashSize = defaultSlashSize;
        fung[0].action();
        fung[1].action();

        yield break;
    }
    protected override IEnumerator endMotion(ActionType action)
    {
        //パーツアクセスのショートカット割り振り
        var fung = GetComponent<Things>().getPartsList
            .Select(parts => parts.GetComponent<Sword>()).ToList();

        var rewindTimeRequired = timeRequired * 2;
        yield return wait(rewindTimeRequired);
        for(int time = 0; time < rewindTimeRequired; time++)
        {
            fung[0].setAngle(easing.liner.In(180, time, rewindTimeRequired - 1));
            fung[1].setAngle(-easing.liner.In(180, time, rewindTimeRequired - 1));

            yield return wait(1);
        }

        yield break;
    }
}
