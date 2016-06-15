using UnityEngine;
using System.Collections;

public class Sword : Weapon
{
    protected override IEnumerator Motion()
    {
        for (var injectionNum = 0; injectionNum < injectionHoles.Count; injectionNum++)
        {
            Debug.Log(injectionNum);
            injection(injectionNum).setAngle(transform.eulerAngles.z + 45 * (heightPositive ? 1 : -1));
        }
        yield break;
    }
}
