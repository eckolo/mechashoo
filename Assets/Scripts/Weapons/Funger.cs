using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ファンガークラス
/// </summary>
public class Funger : Weapon
{
    /// <summary>
    ///1モーションの所要時間
    /// </summary>
    [SerializeField]
    private int timeRequired;
    /// <summary>
    ///斬撃の規模
    /// </summary>
    [SerializeField]
    private int maxSlashSize = 1;
    /// <summary>
    ///斬撃の密度
    /// </summary>
    [SerializeField]
    private int density = 1;

    protected override IEnumerator Motion(int actionNum)
    {
        //パーツアクセスのショートカット割り振り
        List<Sword> fung = new List<Sword>();
        for (int partsNum = 0; partsNum < GetComponent<Material>().getPartsNum(); partsNum++)
        {
            fung.Add(GetComponent<Material>().getParts(partsNum).GetComponent<Sword>());
        }

        for (int time = 0; time < timeRequired; time++)
        {
            fung[0].setAngle(180 - easing.cubic.In(180, time, timeRequired - 1));
            fung[1].setAngle(180 + easing.cubic.In(180, time, timeRequired - 1));

            fung[0].slashSize = easing.cubic.In(maxSlashSize, time, timeRequired - 1);
            fung[1].slashSize = easing.cubic.In(maxSlashSize, time, timeRequired - 1);

            var interval = timeRequired / density;
            if ((timeRequired - 1 - time) % interval == 0)
            {
                fung[0].Action();
                fung[1].Action();
            }

            yield return null;
        }

        yield break;
    }
    protected override IEnumerator endMotion()
    {
        //パーツアクセスのショートカット割り振り
        List<Sword> fung = new List<Sword>();
        for (int partsNum = 0; partsNum < GetComponent<Material>().getPartsNum(); partsNum++)
        {
            fung.Add(GetComponent<Material>().getParts(partsNum).GetComponent<Sword>());
        }

        var rewindTimeRequired = timeRequired * 2;
        for (int time = 0; time < rewindTimeRequired; time++)
        {
            fung[0].setAngle(easing.liner.In(180, time, rewindTimeRequired - 1));
            fung[1].setAngle(-easing.liner.In(180, time, rewindTimeRequired - 1));

            yield return null;
        }

        yield break;
    }
}
