using UnityEngine;
using System.Collections;

public class Sword : Weapon
{
    protected override IEnumerator Motion()
    {
        for (var injectionNum = 0; injectionNum < injectionHoles.Count; injectionNum++)
        {
            var slash = injection(injectionNum);

            slash.setAngle(transform.eulerAngles.z + 45 * (heightPositive ? 1 : -1), followParent().positive);
            slash.setVerosity(slash.transform.rotation * Vector2.right, 10);
        }
        yield break;
    }
}
