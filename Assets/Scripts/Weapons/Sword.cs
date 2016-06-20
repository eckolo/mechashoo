using UnityEngine;
using System.Collections;

/// <summary>
/// 斬撃発生タイプの武装クラス
/// </summary>
public class Sword : Weapon
{
    public float slashSize = 1;

    protected override IEnumerator Motion(int actionNum)
    {
        for (var injectionNum = 0; injectionNum < injectionHoles.Count; injectionNum++)
        {
            var slash = injection(injectionNum).GetComponent<Slash>();
            if (slash == null) continue;

            slash.setAngle(transform.eulerAngles.z + 45 * (heightPositive ? 1 : -1), followParent().widthPositive);
            slash.setVerosity(slash.transform.rotation * Vector2.right, 10);
            slash.setParamate(slashSize);
        }
        yield break;
    }
}
