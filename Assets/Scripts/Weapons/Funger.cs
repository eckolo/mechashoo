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

    protected override IEnumerator Motion(ActionType actionNum)
    {
        //パーツアクセスのショートカット割り振り
        List<Sword> fung = GetComponent<Things>().getPartsList
            .Select(parts => parts.GetComponent<Sword>()).ToList();

        for (int time = 0; time < timeRequired; time++)
        {
            fung[0].setAngle(180 - easing.quintic.In(180, time, timeRequired - 1));
            fung[1].setAngle(180 + easing.quintic.In(180, time, timeRequired - 1));

            fung[0].defaultSlashSize = easing.cubic.In(defaultSlashSize, time, timeRequired - 1);
            fung[1].defaultSlashSize = easing.cubic.In(defaultSlashSize, time, timeRequired - 1);

            var interval = timeRequired / density;
            if (time % interval == 0)
            {
                fung[0].Action();
                fung[1].Action();
            }

            yield return wait(1);
        }

        fung[0].Action();
        fung[1].Action();
        yield break;
    }
    protected override IEnumerator endMotion()
    {
        //パーツアクセスのショートカット割り振り
        List<Sword> fung = GetComponent<Things>().getPartsList
            .Select(parts => parts.GetComponent<Sword>()).ToList();

        var rewindTimeRequired = timeRequired * 2;
        yield return wait(rewindTimeRequired);
        for (int time = 0; time < rewindTimeRequired; time++)
        {
            fung[0].setAngle(easing.liner.In(180, time, rewindTimeRequired - 1));
            fung[1].setAngle(-easing.liner.In(180, time, rewindTimeRequired - 1));

            yield return wait(1);
        }

        yield break;
    }
}
